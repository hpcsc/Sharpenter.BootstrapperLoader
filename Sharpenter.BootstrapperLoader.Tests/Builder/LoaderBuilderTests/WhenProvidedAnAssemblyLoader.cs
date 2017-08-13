using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Builder.LoaderBuilderTests
{
    public class WhenProvidedAnAssemblyLoader
    {
        private Mock<IAssemblyProvider> _assemblyProviderMock;
        private BootstrapperLoader _loader;

        public WhenProvidedAnAssemblyLoader()
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
            _loader = new LoaderBuilder()
                        .Use(_assemblyProviderMock.Object)
                        .Build();
        }

        [Fact(DisplayName = "Should initialize loader with this assembly loader")]
        public void should_initialize_loader_with_this_assembly_loader()
        {
            Assert.Equal(_assemblyProviderMock.Object, _loader.Config.AssemblyProvider);
        }
    }
}