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
        private readonly Func<Type, object> _instanceCreator;
        private IEnumerable<object> _bootstrappers;

        internal BootstrapperLoader(LoaderConfig config, Func<Type, object> instanceCreator)
        {
            _config = config;
            _instanceCreator = instanceCreator;
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
                                  .Select(_instanceCreator));
        }

        public void TriggerConfigureContainer(object container)
        {
            var configureContainerParamTypes = new [] { container.GetType() };
            var configureContainerParam = new[] {container};

            foreach (var bootstrapper in _bootstrappers)
            {
                bootstrapper.GetType()
                    .GetMethod(_config.ConfigureContainerMethodName, configureContainerParamTypes)
                    ?.Invoke(bootstrapper, configureContainerParam);
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
