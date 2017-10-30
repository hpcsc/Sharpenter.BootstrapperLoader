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
    
    public class CustomLoadContext : AssemblyLoadContext
    {
        private readonly string _path;

        public CustomLoadContext(string path)
        {
            _path = path;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            bool NamesMatch(RuntimeLibrary runtime)
            {
                return string.Equals(runtime.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase);
            }

            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(_path);
            var dependencyContext = DependencyContext.Load(assembly);
            var runtimeLibrary = dependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
            if (runtimeLibrary == null) return null;
            
            var assemblies = new List<string>();
            var assemblyResolver = CreateCompositeAssemblyResolver();
            var compilationLibrary = RuntimeToCompilationLibrary(runtimeLibrary);
            assemblyResolver.TryResolveAssemblyPaths(compilationLibrary, assemblies);
            return assemblies.Count > 0 ? LoadFromAssemblyPath(assemblies[0]) : null;
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

        private CompositeCompilationAssemblyResolver CreateCompositeAssemblyResolver()
        {
            return new CompositeCompilationAssemblyResolver
            (
                new ICompilationAssemblyResolver[]
                {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(_path)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                }
            );
        }
    }
}

#endif