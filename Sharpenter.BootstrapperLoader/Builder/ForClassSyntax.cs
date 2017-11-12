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

        public ForClassSyntax AddDefaultMethodNameConvention(Func<bool> condition = null)
        {
            Config.AddDefaultMethodNameConvention(condition);
            
            return this;
        }
        
        public WhenSyntax When(Func<bool> condition)
        {
            return new WhenSyntax(Config, condition);
        }

        public LoaderBuilder And()
        {
            return new LoaderBuilder(Config);
        }
    }
}
