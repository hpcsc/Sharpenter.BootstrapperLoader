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

        internal void AddMethodNameConvention(string nameConvention, Func<bool> condition)
        {
            AddConfigureMethod(ConfigureDefaultMethodName + nameConvention, condition);
            AddConfigureContainerMethod(string.Format("Configure{0}Container", nameConvention), condition);
        }
        
        internal void AddConfigureMethod(string methodName, Func<bool> condition)
        {
            ConfigureMethods[methodName] = condition;
        }
        
        internal void AddConfigureContainerMethod(string methodName, Func<bool> condition)
        {
            ConfigureContainerMethods[methodName] = condition;
        }
    }
}