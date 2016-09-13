using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader
{
    public class BootstrapperLoader
    {
        private readonly LoaderConfig _config;
        private IEnumerable<object> _bootstrappers;

        internal BootstrapperLoader(LoaderConfig config)
        {
            _config = config;
        }

        internal void Initialize()
        {
            var assemblies = _config.AssemblyProvider.Find();

            var assembliesWithBootstrapper =
                assemblies.Where(
                    a => a.GetTypes()
                          .FirstOrDefault(t => t.Name == _config.BootstrapperClassName) != null);

            _bootstrappers = assembliesWithBootstrapper
                .SelectMany(a => a.GetTypes()
                                  .Where(t => t.Name == _config.BootstrapperClassName)
                                  .Select(Activator.CreateInstance));
        }

        public void TriggerConfigureContainer(object container)
        {
            foreach (var bootstrapper in _bootstrappers)
            {
                bootstrapper.GetType()
                    .GetMethod(_config.ConfigureContainerMethodName, new [] { container.GetType() })
                    ?.Invoke(bootstrapper, new[] { container });
            }
        }

        public void TriggerConfigure(IServiceLocator serviceLocator)
        {
            foreach (var bootstrapper in _bootstrappers)
            {
                bootstrapper
                    .GetType()
                    .GetMethod(_config.ConfigureMethodName)
                    ?.InvokeWithDynamicallyResolvedParameters(bootstrapper, serviceLocator);                
            }
        }
    }
}
