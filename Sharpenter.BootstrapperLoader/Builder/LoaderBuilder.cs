using System;
using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader.Builder
{
    public class LoaderBuilder : LoaderBuilderBase
    {
        public LoaderBuilder() :
            this(new LoaderConfig())
        {            
        }

        internal LoaderBuilder(LoaderConfig config)
            : base(config)
        {
        }

        public LoaderBuilder Use(IAssemblyProvider assemblyProvider)
        {
            Config.AssemblyProvider = assemblyProvider;

            return this;
        }

        public ForClassSyntax ForClass()
        {
            return new ForClassSyntax(Config);
        }

        internal LoaderBuilder UseInstanceCreator(Func<Type, object> instanceCreator)
        {
            Config.InstanceCreator = instanceCreator;

            return this;
        }
    }
}
