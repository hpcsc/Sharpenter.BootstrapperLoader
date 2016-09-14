using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader.Builder
{
    public class ForClassSyntax : LoaderBuilderBase
    {
        internal ForClassSyntax(LoaderConfig config)
            : base(config)
        {
        }

        public ForClassSyntax WithName(string bootstrapperClassName)
        {
            Config.BootstrapperClassName = bootstrapperClassName;

            return this;
        }

        public ForClassSyntax ConfigureContainerWith(string configureContainerMethodName)
        {
            Config.ConfigureContainerMethodName = configureContainerMethodName;

            return this;
        }

        public MethodsSyntax Methods()
        {
            return new MethodsSyntax(Config);
        }

        public LoaderBuilder And()
        {
            return new LoaderBuilder(Config);
        }
    }
}
