using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Builder.LoaderBuilderTests
{
    public class WhenConfigClassConstructorParametersUsingBuilder
    {
        private readonly Mock<IAssemblyProvider> _assemblyProviderMock;
        private readonly BootstrapperLoader _loader;

        public WhenConfigClassConstructorParametersUsingBuilder()
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
            _loader = new LoaderBuilder()
                            .Use(_assemblyProviderMock.Object)
                            .ForClass()
                                .HasConstructorParameter("some string")
                            .Build();
        }

        [Fact(DisplayName = "Should initialize loader with those constructor parameters")]
        public void should_initialize_loader_with_those_constructor_parameters()
        {
            Assert.IsType<ExpressionCreator<string>>(_loader.Config.InstanceCreator);
        }
    }
}