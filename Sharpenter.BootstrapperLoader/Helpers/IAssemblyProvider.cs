using System.Collections.Generic;
using System.Reflection;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> Find();
    }
}
