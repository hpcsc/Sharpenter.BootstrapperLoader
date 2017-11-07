using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sharpenter.BootstrapperLoader
{
    public class BootstrapperLoader
    {
        internal List<object> Bootstrappers { get; private set; }

        internal LoaderConfig Config { get; }

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

            Bootstrappers = assembliesWithBootstrapper
                .SelectMany(a => a.GetTypes()
                                  .Where(t => t.Name == Config.BootstrapperClassName)
                                  .Select(Config.InstanceCreator.Create))
                                  .ToList();
        }
        
        public void Trigger<TArg>(string methodName, TArg parameter)
        {
            Bootstrappers.ForEach(bootstrapper =>
                ExecuteIfNotNull(
                    bootstrapper.GetType()
                        .GetMethod(methodName, new[] {typeof(TArg)}),
                    methodInfo => methodInfo.Invoke(bootstrapper, new object[] { parameter }))
            );
        }

        public void TriggerConfigureContainer<TArg>(TArg parameter)
        {
            Bootstrappers.ForEach(bootstrapper =>
            {
                Config.ConfigureContainerMethods
                    .Where(c => c.Value())
                    .ToList()
                    .ForEach(methodConfiguration => 
                        ExecuteIfNotNull(
                            bootstrapper.GetType().GetMethod(methodConfiguration.Key, new[] {typeof(TArg)}),
                            methodInfo => methodInfo.Invoke(bootstrapper, new object[] { parameter }))
                    );
            });
        }

        public void TriggerConfigure(Func<Type, object> serviceLocator = null)
        {
            Bootstrappers.ForEach(bootstrapper =>
            {
                Config.ConfigureMethods
                       .Where(c => c.Value())
                       .ToList()
                       .ForEach(methodConfiguration => 
                                ExecuteIfNotNull(
                                    GetMethodInfoByName(bootstrapper.GetType(), methodConfiguration.Key, serviceLocator),
                                    methodInfo => methodInfo.InvokeWithDynamicallyResolvedParameters(bootstrapper, serviceLocator))
                               );
            });
        }

        private static MethodInfo GetMethodInfoByName(Type bootstrapperType, string methodName, Func<Type, object> serviceLocator)
        {
            return serviceLocator == null
                ? bootstrapperType.GetMethod(methodName, new Type[0])
                : bootstrapperType.GetMethod(methodName);
        }

        private static void ExecuteIfNotNull(MethodInfo methodInfo, Action<MethodInfo> action)
        {
            if (methodInfo != null)
            {
                action(methodInfo);
            }
        }
    }
}
