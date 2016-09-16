using System;
using System.Reflection;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    public static class MethodInfoExtensions
    {
        public static void InvokeWithDynamicallyResolvedParameters(this MethodInfo configureMethod, object bootstrapper, Func<Type, object> serviceLocator)
        {
            var parameterInfos = configureMethod.GetParameters();
            var parameters = new object[parameterInfos.Length];
            for (var index = 0; index < parameterInfos.Length; index++)
            {
                var parameterInfo = parameterInfos[index];

                try
                {
                    parameters[index] = serviceLocator(parameterInfo.ParameterType);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Could not resolve a service of type '{parameterInfo.ParameterType.FullName}' for the parameter '{parameterInfo.Name}' of method '{configureMethod.Name}' on type '{configureMethod.DeclaringType.FullName}'.",
                        ex);
                }
            }

            configureMethod.Invoke(bootstrapper, parameters);
        }
    }
}
