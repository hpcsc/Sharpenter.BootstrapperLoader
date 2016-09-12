using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
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

        public void Initialize()
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

        public void ConfigureContainer(object container)
        {
            foreach (var bootstrapper in _bootstrappers)
            {
                bootstrapper.GetType()
                    .GetMethod(_config.ConfigureContainerMethodName, new [] { container.GetType() })?
                    .Invoke(bootstrapper, new[] { container });
            }
        }

        public void Configure(IServiceLocator serviceLocator)
        {
            foreach (var bootstrapper in _bootstrappers)
            {
                InvokeMethodIfAvailable(bootstrapper, _config.ConfigureMethodName, serviceLocator);
            }
        }      

        private static void InvokeMethodIfAvailable(object bootstrapper, string methodName, IServiceLocator serviceLocator)
        {
            var configureMethod = bootstrapper.GetType().GetMethod(methodName);
            if (configureMethod != null)
            {
                InvokeMethodWithDynamicallyResolvedParameters(bootstrapper, configureMethod, serviceLocator);
            }
        }

        private static void InvokeMethodWithDynamicallyResolvedParameters(object bootstrapper, MethodInfo configureMethod, IServiceLocator serviceLocator)
        {
            var parameterInfos = configureMethod.GetParameters();
            var parameters = new object[parameterInfos.Length];
            for (var index = 0; index < parameterInfos.Length; index++)
            {
                var parameterInfo = parameterInfos[index];
                if (parameterInfo.ParameterType == typeof(IServiceLocator))
                {
                    parameters[index] = serviceLocator;
                }
                else
                {
                    try
                    {
                        parameters[index] = serviceLocator.GetInstance(parameterInfo.ParameterType);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(
                            $"Could not resolve a service of type '{parameterInfo.ParameterType.FullName}' for the parameter '{parameterInfo.Name}' of method '{configureMethod.Name}' on type '{configureMethod.DeclaringType.FullName}'.",
                            ex);
                    }
                }
            }

            configureMethod.Invoke(bootstrapper, parameters);
        }
    }
}
