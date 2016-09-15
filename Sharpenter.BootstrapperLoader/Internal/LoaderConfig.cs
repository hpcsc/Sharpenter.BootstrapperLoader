using System;
using System.Collections.Generic;
using System.IO;
using Sharpenter.BootstrapperLoader.Helpers;

namespace Sharpenter.BootstrapperLoader.Internal
{
    internal class LoaderConfig
    {
        private const string BootstrapperDefaultClassName = "Bootstrapper";
        private const string ConfigureContainerDefaultMethodName = "ConfigureContainer";
        private const string ConfigureDefaultMethodName = "Configure";

        private readonly Func<bool> AlwaysCall = () => true;

        internal ICreateObject InstanceCreator { get; set; }
        internal string BootstrapperClassName { get; set; }
        internal string ConfigureContainerMethodName { get; set; }
        internal Dictionary<string, Func<bool>> ConfigureMethods { get; set; }
        internal IAssemblyProvider AssemblyProvider { get; set; }

        internal LoaderConfig()
        {
            BootstrapperClassName = BootstrapperDefaultClassName;
            ConfigureContainerMethodName = ConfigureContainerDefaultMethodName;
            ConfigureMethods = new Dictionary<string, Func<bool>>
            {
                {ConfigureDefaultMethodName, AlwaysCall}
            };

            InstanceCreator = new ActivatorCreator();
            AssemblyProvider = new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "*.dll");
        }

        internal void AddConfigureMethod(string methodName)
        {
            if (ConfigureMethods.ContainsKey(methodName))
                throw new ArgumentException($"Duplication configureation for method '{methodName}' detected");

            ConfigureMethods[methodName] = AlwaysCall;
        }

        internal void UpdateMethodCallCondition(string name, Func<bool> condition)
        {
            if (!ConfigureMethods.ContainsKey(name))
                throw new ArgumentException($"Configuration for method '{name} not found");

            ConfigureMethods[name] = condition;
        }

        internal void ClearMethodConfigurations()
        {
            ConfigureMethods.Clear();
        }
    }
}