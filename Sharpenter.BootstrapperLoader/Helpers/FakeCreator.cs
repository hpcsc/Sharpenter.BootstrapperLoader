﻿using System;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal class FakeCreator : IAmInstanceCreator
    {
        private readonly object _instance;

        public FakeCreator(object instance)
        {
            _instance = instance;
        }

        public object Create(Type type)
        {
            return _instance;
        }
    }
}
