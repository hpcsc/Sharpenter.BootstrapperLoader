#if !NET452

namespace Sharpenter.BootstrapperLoader.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using Microsoft.Extensions.DependencyModel;
    using Microsoft.Extensions.DependencyModel.Resolution;
    
    public class CustomAssemblyResolver
    {
        private readonly DependencyContext _dependencyContext;
        private readonly CompositeCompilationAssemblyResolver _assemblyResolver;

        public Assembly Assembly { get; }

        public CustomAssemblyResolver(string path)
        {
            _assemblyResolver = CreateCompositeAssemblyResolver(path);
                        
            Assembly = Assembly.LoadFrom(path);
            _dependencyContext = DependencyContext.Load(Assembly);
            RegisterLoadContextResolving(Assembly);
        }

        private void RegisterLoadContextResolving(Assembly assembly)
        {
            var loadContext = AssemblyLoadContext.GetLoadContext(assembly);
            loadContext.Resolving += OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            bool NamesMatch(RuntimeLibrary runtime)
            {
                return string.Equals(runtime.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase);
            }

            var runtimeLibrary = _dependencyContext?.RuntimeLibraries.FirstOrDefault(NamesMatch);
            return runtimeLibrary == null ? null : Resolve(context, runtimeLibrary);
        }
        
        private Assembly Resolve(AssemblyLoadContext context, RuntimeLibrary runtimeLibrary)
        {
            var assemblies = new List<string>();            
            var compilationLibrary = RuntimeToCompilationLibrary(runtimeLibrary);
            _assemblyResolver.TryResolveAssemblyPaths(compilationLibrary, assemblies);
            return assemblies.Count > 0 ? context.LoadFromAssemblyPath(assemblies[0]) : null;
        }

        private static CompilationLibrary RuntimeToCompilationLibrary(RuntimeLibrary runtimeLibrary)
        {
            return new CompilationLibrary(
                runtimeLibrary.Type,
                runtimeLibrary.Name,
                runtimeLibrary.Version,
                runtimeLibrary.Hash,
                runtimeLibrary.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                runtimeLibrary.Dependencies,
                runtimeLibrary.Serviceable);
        }

        private static CompositeCompilationAssemblyResolver CreateCompositeAssemblyResolver(string path)
        {
            return new CompositeCompilationAssemblyResolver
            (
                new ICompilationAssemblyResolver[]
                {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                }
            );
        }
    }
}

#endif