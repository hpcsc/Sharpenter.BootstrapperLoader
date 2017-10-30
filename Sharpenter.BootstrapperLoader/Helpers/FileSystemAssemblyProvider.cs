using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    public class FileSystemAssemblyProvider : IAssemblyProvider
    {
        private readonly string _searchDirectory;
        private readonly string _searchPattern;

        public FileSystemAssemblyProvider(string searchDirectory, string searchPattern)
        {
            _searchDirectory = searchDirectory;
            _searchPattern = searchPattern;
        }

        public IEnumerable<Assembly> Find()
        {
            var dllNames = Directory.GetFiles(_searchDirectory, _searchPattern);
            return dllNames.Select(TryLoadAssembly).Where(a => a != null);
        }

        private static Assembly TryLoadAssembly(string path)
        {
            try
            {
                #if NET462
                return Assembly.LoadFrom(path);
                #else
                var customLoadContext = new CustomLoadContext(path);
                return customLoadContext.LoadFromAssemblyPath(path);
                #endif
            }
            #pragma warning disable 0168
            catch (FileLoadException loadEx)
            { } // The Assembly has already been loaded.
            catch (BadImageFormatException imgEx)
            { } // If a BadImageFormatException exception is thrown, the file is not an assembly.
            #pragma warning restore 0168
            
            return null;
        }
    }
}
