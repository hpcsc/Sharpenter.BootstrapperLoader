using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WhenConditionReturnsFalse
{
    public class WhenConfigureMethodHasDifferentName
    {
        private readonly BootstrapperLoader _subject;
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<FifthBootstrapper> _bootstrapperMock;

        public WhenConfigureMethodHasDifferentName()
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();  
            
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<FifthBootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("FifthBootstrapper")
                    .When(() => false)
                        .CallConfigure("SomeConfigure")
                .Build();
        }

        [Fact(DisplayName = "Should not invoke conditional configure method")]
        public void should_not_invoke_configure_method()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);

            _bootstrapperMock.Verify(
                b => b.SomeConfigure(Moq.It.IsAny<IFirstDependency>(), Moq.It.IsAny<ISecondDependency>()),
                Times.Never);
        }
        
        [Fact(DisplayName = "Should invoke default configure method")]
        public void should_invoke_default_configure_method()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);
            
            _bootstrapperMock.Verify(
                b => b.Configure(Moq.It.IsAny<IFirstDependency>(), Moq.It.IsAny<ISecondDependency>()));
        }
    }
}