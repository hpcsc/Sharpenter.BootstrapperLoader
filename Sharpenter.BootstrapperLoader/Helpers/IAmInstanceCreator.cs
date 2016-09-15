using System;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal interface IAmInstanceCreator
    {
        object Create(Type type);
    }
}
