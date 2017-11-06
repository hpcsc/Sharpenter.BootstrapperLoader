using System;
using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader.Builder
{
    public class WhenSyntax : LoaderBuilderBase
    {
        private readonly Func<bool> _condition;

        internal WhenSyntax(LoaderConfig config, Func<bool> condition)
            : base(config)
        {
            _condition = condition;
            Config.ClearDefaultMethodConfigurations();
        }
        
        public WhenSyntax CallConfigure(string configureMethodName)
        {
            Config.AddConfigureMethod(configureMethodName, _condition);

            return this;
        }

        public ForClassSyntax And()
        {
            return new ForClassSyntax(Config);
        }
    }
}