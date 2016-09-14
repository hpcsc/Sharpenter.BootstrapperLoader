using System;
using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader.Builder
{
    public class MethodsSyntax : LoaderBuilderBase
    {
        private string _methodName;

        internal MethodsSyntax(LoaderConfig config)
            : base(config)
        {
        }

        public MethodsSyntax ClearAll()
        {
            Config.ClearMethodConfigurations();

            return this;
        }

        public MethodsSyntax Call(string methodName)
        {
            _methodName = methodName;

            Config.AddConfigureMethod(methodName);

            return this;
        }

        public MethodsSyntax If(Func<bool> condition)
        {
            Config.UpdateMethodCallCondition(_methodName, condition);

            return this;
        }

        public ForClassSyntax And()
        {
            return new ForClassSyntax(Config);
        }
    }
}
