using System;
using Sharpenter.BootstrapperLoader.Helpers;
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

        public ForClassSyntax HasConstructorParameter<TArg>(TArg parameter)
        {
            Config.InstanceCreator = new ExpressionCreator<TArg>(parameter);

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
