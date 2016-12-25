using Sharpenter.BootstrapperLoader.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sharpenter.BootstrapperLoader.Internal
{
    internal class LoaderConfig
    {
        private const string BootstrapperDefaultClassName = "Bootstrapper";        
        private const string ConfigureDefaultMethodName = "Configure";

        private readonly Func<bool> AlwaysCall = () => true;

        internal IAmInstanceCreator InstanceCreator { get; set; }
        internal string BootstrapperClassName { get; set; }
        internal Dictionary<string, Func<bool>> ConfigureMethods { get; set; }
        internal IAssemblyProvider AssemblyProvider { get; set; }

        internal LoaderConfig()
        {
            BootstrapperClassName = BootstrapperDefaultClassName;
            ConfigureMethods = new Dictionary<string, Func<bool>>
            {
                {ConfigureDefaultMethodName, AlwaysCall}
            };

            InstanceCreator = new ExpressionCreator();
            AssemblyProvider = new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "*.dll");
        }

        internal void AddConfigureMethod(string methodName)
        {
            if (ConfigureMethods.ContainsKey(methodName))
                throw new ArgumentException(string.Format("Duplication configureation for method '{0}' detected", methodName));

            ConfigureMethods[methodName] = AlwaysCall;
        }

        internal void UpdateMethodCallCondition(string name, Func<bool> condition)
        {
            if (!ConfigureMethods.ContainsKey(name))
                throw new ArgumentException(string.Format("Configuration for method '{0} not found", name));

            ConfigureMethods[name] = condition;
        }

        internal void ClearMethodConfigurations()
        {
            ConfigureMethods.Clear();
        }
    }
}