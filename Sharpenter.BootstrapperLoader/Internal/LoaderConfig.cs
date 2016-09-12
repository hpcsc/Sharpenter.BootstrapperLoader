using System.IO;

namespace Sharpenter.BootstrapperLoader.Internal
{
    internal class LoaderConfig
    {
        private const string BootstrapperDefaultClassName = "Bootstrapper";
        private const string ConfigureContainerDefaultMethodName = "ConfigureContainer";
        private const string ConfigureDefaultMethodName = "Configure";

        internal string BootstrapperClassName { get; set; }
        internal string ConfigureContainerMethodName { get; set; }
        internal string ConfigureMethodName { get; set; }
        internal IAssemblyProvider AssemblyProvider { get; set; }

        public LoaderConfig()
        {
            BootstrapperClassName = BootstrapperDefaultClassName;
            ConfigureContainerMethodName = ConfigureContainerDefaultMethodName;
            ConfigureMethodName = ConfigureDefaultMethodName;

            AssemblyProvider = new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "*.dll");
        }
    }
}
