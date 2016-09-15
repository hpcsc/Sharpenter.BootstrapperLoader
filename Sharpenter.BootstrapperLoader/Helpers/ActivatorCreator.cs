using System;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal class ActivatorCreator : IAmInstanceCreator
    {
        public object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
