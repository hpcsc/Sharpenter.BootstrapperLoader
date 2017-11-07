using System.Linq;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Builder.LoaderBuilderTests
{
    public class CallingWhenMultipleTimes
    {
        private readonly Mock<IAssemblyProvider> _assemblyProviderMock;
        private readonly BootstrapperLoader _loader;

        public CallingWhenMultipleTimes()
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
            _loader = new LoaderBuilder()
                .Use(_assemblyProviderMock.Object)
                .ForClass()
                    .When(() => true)
                        .CallConfigure("Configure1")
                        .CallConfigureContainer("ConfigureContainer1")
                    .And()
                    .When(() => true)
                        .CallConfigure("Configure2")
                        .CallConfigureContainer("ConfigureContainer2")
                    .And()
                    .When(() => true)
                        .CallConfigure("Configure")
                        .CallConfigureContainer("ConfigureContainer")
                .Build();
        }

        [Fact(DisplayName = "Should not clear previous configuration and maintain default methods")]
        public void should_not_clear_previous_configuration()
        {
            Assert.Equal(new[] { "Configure", "Configure1", "Configure2" }, _loader.Config.ConfigureMethods.Keys.ToArray());
            Assert.Equal(new[] { "ConfigureContainer", "ConfigureContainer1", "ConfigureContainer2" }, _loader.Config.ConfigureContainerMethods.Keys.ToArray());
        }
    }
}