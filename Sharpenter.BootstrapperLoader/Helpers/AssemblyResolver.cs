using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Sharpenter.BootstrapperLoader.Helpers
{    
    /// <summary>
    /// This class is from: https://samcragg.wordpress.com/2017/06/30/resolving-assemblies-in-net-core/
    /// </summary>
    internal sealed class AssemblyResolver : IDisposable
    {
        private readonly ICompilationAssemblyResolver _assemblyResolver;
        private readonly DependencyContext _dependencyContext;

        public AssemblyResolver(string path)
        {
            _assemblyResolver = new CompositeCompilationAssemblyResolver
            (
                new ICompilationAssemblyResolver[]
                {
                    new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                }
            );

            AssemblyLoadContext.Default.Resolving += OnResolving;
            Assembly = Assembly.LoadFrom(path);
            _dependencyContext = DependencyContext.Load(Assembly);        
        }

        public Assembly Assembly { get; }

        public void Dispose()
        {
            AssemblyLoadContext.Default.Resolving -= OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            bool NamesMatch(RuntimeLibrary runtime)
            {
                return string.Equals(runtime.Name, name.Name, StringComparison.OrdinalIgnoreCase);
            }

            var library = _dependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
            if (library == null) return null;
            
            var wrapper = new CompilationLibrary(
                library.Type,
                library.Name,
                library.Version,
                library.Hash,
                library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                library.Dependencies,
                library.Serviceable);

            var assemblies = new List<string>();
            _assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
            return assemblies.Count > 0 ? context.LoadFromAssemblyPath(assemblies[0]) : null;
        }
    }    
}