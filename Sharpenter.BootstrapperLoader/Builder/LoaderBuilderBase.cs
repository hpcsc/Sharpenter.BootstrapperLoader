﻿using Sharpenter.BootstrapperLoader.Internal;

namespace Sharpenter.BootstrapperLoader.Builder
{
    public abstract class LoaderBuilderBase
    {
        internal readonly LoaderConfig Config;

        internal LoaderBuilderBase(LoaderConfig config)
        {
            Config = config;
        }

        public BootstrapperLoader Build()
        {
            var loader = new BootstrapperLoader(Config);
            loader.Initialize();

            return loader;
        }
    }
}
