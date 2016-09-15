using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    public class InMemoryAssemblyProvider : IAssemblyProvider
    {
        private readonly Func<IEnumerable<Assembly>> _assemblyFactory;

        public InMemoryAssemblyProvider(Func<IEnumerable<Assembly>> assemblyFactory)
        {
            _assemblyFactory = assemblyFactory;
        }

        public IEnumerable<Assembly> Find()
        {
            return _assemblyFactory();
        }
    }
}
