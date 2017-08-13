using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Builder.LoaderBuilderTests
{
    public class WhenConfigClassNameUsingBuilder
    {
        private Mock<IAssemblyProvider> _assemblyProviderMock;
        private BootstrapperLoader _loader;

        public WhenConfigClassNameUsingBuilder()
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
            _loader = new LoaderBuilder()
                        .Use(_assemblyProviderMock.Object)
                        .ForClass()
                            .WithName("SomeBootstrapper")
                        .Build();
        }

        [Fact(DisplayName = "Should initialize loader with that class name")]
        public void Should_initialize_loader_with_that_class_name()
        {
            Assert.Equal("SomeBootstrapper", _loader.Config.BootstrapperClassName);
        }
    }
}