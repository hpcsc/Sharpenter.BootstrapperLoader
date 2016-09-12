using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader.Builder
{
    public class LoaderClassConfigBuilder : LoaderBuilderBase
    {
        internal LoaderClassConfigBuilder(LoaderConfig config)
            : base(config)
        {
        }

        public LoaderClassConfigBuilder WithName(string bootstrapperClassName)
        {
            Config.BootstrapperClassName = bootstrapperClassName;

            return this;
        }

        public LoaderClassConfigBuilder ConfigureContainerWith(string configureContainerMethodName)
        {
            Config.ConfigureContainerMethodName = configureContainerMethodName;

            return this;
        }

        public LoaderClassConfigBuilder ConfigureWith(string configureMethodName)
        {
            Config.ConfigureMethodName = configureMethodName;

            return this;
        }

        public LoaderBuilder And()
        {
            return new LoaderBuilder(Config);
        }
    }
}
