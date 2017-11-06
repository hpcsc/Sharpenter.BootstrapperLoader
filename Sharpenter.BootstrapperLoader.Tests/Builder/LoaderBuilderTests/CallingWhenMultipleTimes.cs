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
                    .And()
                    .When(() => true)
                        .CallConfigure("Configure2")
                    .And()
                    .When(() => true)
                        .CallConfigure("Configure")
                .Build();
        }

        [Fact(DisplayName = "Should not clear previous configuration")]
        public void should_not_clear_previous_configuration()
        {
            Assert.Equal(new string[] { "Configure1", "Configure2", "Configure" }, _loader.Config.ConfigureMethods.Keys.ToArray());
        }
    }
}