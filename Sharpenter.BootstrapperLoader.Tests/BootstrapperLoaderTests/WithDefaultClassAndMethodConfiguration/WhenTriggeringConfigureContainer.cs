using Autofac;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDefaultClassAndMethodConfiguration
{
    public class WhenTriggeringConfigureContainer : TestBase
    {
        [Fact(DisplayName = "Should invoke default configure container")]
        public void should_invoke_default_configure_container()
        {
            Subject.TriggerConfigureContainer(ContainerBuilder);

            BootstrapperMock.Verify(b => b.ConfigureContainer(Moq.It.IsAny<ContainerBuilder>()));
        }
    }
}