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
        private const string ConfigureContainerDefaultMethodName = "ConfigureContainer";

        private bool _useDefaultConfigureMethod = true;
        private bool _useDefaultConfigureContainerMethod = true;

        private readonly Func<bool> _alwaysCall = () => true;

        internal IAmInstanceCreator InstanceCreator { get; set; }
        internal string BootstrapperClassName { get; set; }
        internal Dictionary<string, Func<bool>> ConfigureMethods { get; }
        internal Dictionary<string, Func<bool>> ConfigureContainerMethods { get; }
        internal IAssemblyProvider AssemblyProvider { get; set; }

        internal LoaderConfig()
        {
            BootstrapperClassName = BootstrapperDefaultClassName;
            ConfigureMethods = new Dictionary<string, Func<bool>>
            {
                {ConfigureDefaultMethodName, _alwaysCall}
            };
            
            ConfigureContainerMethods = new Dictionary<string, Func<bool>>
            {
                {ConfigureContainerDefaultMethodName, _alwaysCall}
            };

            InstanceCreator = new ExpressionCreator();
            AssemblyProvider = new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "*.dll");
        }
        
        internal void AddConfigureMethod(string methodName, Func<bool> condition)
        {
            if (ConfigureMethods.ContainsKey(methodName))
                throw new ArgumentException(string.Format("Duplication configuration for method '{0}' detected", methodName));

            ConfigureMethods[methodName] = condition;
        }
        
        internal void AddConfigureContainerMethod(string methodName, Func<bool> condition)
        {
            if (ConfigureContainerMethods.ContainsKey(methodName))
                throw new ArgumentException(string.Format("Duplication configuration for method '{0}' detected", methodName));

            ConfigureContainerMethods[methodName] = condition;
        }

        internal void UpdateMethodCallCondition(string name, Func<bool> condition)
        {
            if (!ConfigureMethods.ContainsKey(name))
                throw new ArgumentException(string.Format("Configuration for method '{0} not found", name));

            ConfigureMethods[name] = condition;
        }

        internal void ClearDefaultConfigureMethods()
        {
            if (!_useDefaultConfigureMethod) return;
            
            ConfigureMethods.Clear();
            _useDefaultConfigureMethod = false;
        }
        
        internal void ClearDefaultConfigureContainerMethods()
        {
            if (!_useDefaultConfigureContainerMethod) return;
            
            ConfigureContainerMethods.Clear();
            _useDefaultConfigureContainerMethod = false;
        }
    }
}