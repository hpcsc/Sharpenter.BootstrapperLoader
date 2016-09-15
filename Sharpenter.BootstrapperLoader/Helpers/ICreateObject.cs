using System;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal interface ICreateObject
    {
        object Create(Type type);
    }
}
