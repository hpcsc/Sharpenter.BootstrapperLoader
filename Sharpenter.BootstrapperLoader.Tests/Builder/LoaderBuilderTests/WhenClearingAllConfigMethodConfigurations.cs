using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Builder.LoaderBuilderTests
{
    public class WhenClearingAllConfigMethodConfigurations
    {
        private Mock<IAssemblyProvider> _assemblyProviderMock;
        private BootstrapperLoader _loader;

        public WhenClearingAllConfigMethodConfigurations()
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
            _loader = new LoaderBuilder()
                        .Use(_assemblyProviderMock.Object)
                        .ForClass()
                            .Methods()
                                .ClearAll()
                        .Build();
        }

        [Fact(DisplayName = "Should not have any method configurations in loader")]
        public void should_not_have_any_method_configurations_in_loader()
        {
            Assert.Empty(_loader.Config.ConfigureMethods);
        }
    }
}