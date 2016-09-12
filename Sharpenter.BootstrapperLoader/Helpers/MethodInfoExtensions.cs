using System;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    public static class MethodInfoExtensions
    {
        public static void InvokeWithDynamicallyResolvedParameters(this MethodInfo configureMethod, object bootstrapper, IServiceLocator serviceLocator)
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
