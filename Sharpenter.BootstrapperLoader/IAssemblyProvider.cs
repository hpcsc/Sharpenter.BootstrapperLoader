using System.Collections.Generic;
using System.Reflection;

namespace Sharpenter.BootstrapperLoader
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> Find();
    }
}
