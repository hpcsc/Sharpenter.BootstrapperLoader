using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WhenSyntax.WhenConditionReturnsFalse
{
    public class WhenConfigureContainerMethodHasDifferentName
    {
        private readonly BootstrapperLoader _subject;
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<FifthBootstrapper> _bootstrapperMock;

        public WhenConfigureContainerMethodHasDifferentName()
        {
            _containerBuilder = new ContainerBuilder();
            
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<FifthBootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("FifthBootstrapper")
                    .When(() => false)
                        .CallConfigureContainer("SomeConfigureContainer")
                .Build();
        }

        [Fact(DisplayName = "Should not invoke conditional configure container method")]
        public void should_not_invoke_configure_container_method()
        {
            _subject.TriggerConfigureContainer(_containerBuilder);

            _bootstrapperMock.Verify(
                b => b.SomeConfigureContainer(Moq.It.IsAny<ContainerBuilder>()),
                Times.Never);
        }
        
        [Fact(DisplayName = "Should not invoke default configure container method")]
        public void should_not_invoke_default_configure_method()
        {
            _subject.TriggerConfigureContainer(_containerBuilder);
            
            _bootstrapperMock.Verify(
                b => b.ConfigureContainer(Moq.It.IsAny<ContainerBuilder>()),
                Times.Never);
        }
    }
}