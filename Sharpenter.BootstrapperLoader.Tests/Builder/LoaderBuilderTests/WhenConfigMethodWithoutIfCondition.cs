using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Builder.LoaderBuilderTests
{
    public class WhenConfigMethodWithoutIfCondition
    {
        private Mock<IAssemblyProvider> _assemblyProviderMock;
        private BootstrapperLoader _loader;

        public WhenConfigMethodWithoutIfCondition()
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
            _loader = new LoaderBuilder()
                        .Use(_assemblyProviderMock.Object)
                        .ForClass()
                            .Methods()
                                .Call("SomeConfigure")
                        .Build();
        }

        [Fact(DisplayName = "Should always call that method")]
        public void should_always_call_that_method()
        {
            Assert.True(_loader.Config.ConfigureMethods["SomeConfigure"]());
        }
    }
}