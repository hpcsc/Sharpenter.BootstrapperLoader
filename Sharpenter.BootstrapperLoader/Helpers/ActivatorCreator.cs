using System;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal class ActivatorCreator : ICreateObject
    {
        public object Create(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
