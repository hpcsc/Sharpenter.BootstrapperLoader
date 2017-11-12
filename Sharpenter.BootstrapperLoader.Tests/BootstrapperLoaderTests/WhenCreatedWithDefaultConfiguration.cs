using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public class WhenCreatedWithDefaultConfiguration
    {
        private readonly BootstrapperLoader _subject;
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<Bootstrapper> _bootstrapperMock;

        public WhenCreatedWithDefaultConfiguration()
        {        
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();
            
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<Bootstrapper>();
            _subject = new LoaderBuilder()
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Build();                
        }
        
        [Fact(DisplayName = "Should not invoke default configure method")]
        public void should_not_invoke_default_configure_method()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);
            
            _bootstrapperMock.Verify(
                b => b.Configure(Moq.It.IsAny<IFirstDependency>(), Moq.It.IsAny<ISecondDependency>()),
                Times.Never);
        }
        
        [Fact(DisplayName = "Should not invoke default configure container method")]
        public void should_not_invoke_default_configure_container_method()
        {
            _subject.TriggerConfigureContainer(_containerBuilder);
            
            _bootstrapperMock.Verify(
                b => b.ConfigureContainer(Moq.It.IsAny<ContainerBuilder>()),
                Times.Never);
        }
    }
}