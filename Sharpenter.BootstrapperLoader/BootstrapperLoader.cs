using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader
{
    public class BootstrapperLoader
    {
        private List<object> _bootstrappers;

        internal LoaderConfig Config { get; set; }

        internal BootstrapperLoader(LoaderConfig config)
        {
            Config = config;
        }

        internal void Initialize()
        {
            var assemblies = Config.AssemblyProvider.Find();

            var assembliesWithBootstrapper =
                assemblies.Where(
                    a => a.GetTypes()
                          .FirstOrDefault(t => t.Name == Config.BootstrapperClassName) != null);

            _bootstrappers = assembliesWithBootstrapper
                .SelectMany(a => a.GetTypes()
                                  .Where(t => t.Name == Config.BootstrapperClassName)
                                  .Select(Config.InstanceCreator))
                                  .ToList();
        }

        public void TriggerConfigureContainer(object container)
        {
            var configureContainerParamTypes = new [] { container.GetType() };
            var configureContainerParam = new[] {container};

            _bootstrappers.ForEach(bootstrapper => 
                bootstrapper.GetType()
                            .GetMethod(Config.ConfigureContainerMethodName, configureContainerParamTypes)
                            ?.Invoke(bootstrapper, configureContainerParam));
        }

        public void TriggerConfigure(IServiceLocator serviceLocator)
        {
            _bootstrappers.ForEach(bootstrapper =>
            {
                Config.ConfigureMethods
                       .Where(c => c.Value())
                       .ToList()
                       .ForEach(methodConfiguration => bootstrapper.GetType()
                                                                   .GetMethod(methodConfiguration.Key)
                                                                   ?.InvokeWithDynamicallyResolvedParameters(bootstrapper, serviceLocator));
            });
        }
    }
}
