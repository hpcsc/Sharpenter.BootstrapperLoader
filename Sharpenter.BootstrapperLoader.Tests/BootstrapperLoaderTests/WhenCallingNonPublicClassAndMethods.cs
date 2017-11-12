using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public class WhenCallingNonPublicClassAndMethods
    {
        private readonly BootstrapperLoader _subject;
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<NonPublicBootstrapper> _bootstrapperMock;

        public WhenCallingNonPublicClassAndMethods()
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<NonPublicBootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("NonPublicBootstrapper")
                    .AddDefaultMethodNameConvention()
                .Build();
        }
        
        [Fact(DisplayName = "Should invoke correct configure method")]
        public void should_invoke_correct_configure_method()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);

            _bootstrapperMock.Verify(
                b => b.Configure(Moq.It.IsAny<IFirstDependency>()));
        }

        [Fact(DisplayName = "Should invoke correct configure container method")]
        public void should_invoke_correct_configure_container_method()
        {
            _subject.TriggerConfigureContainer(_containerBuilder);

            _bootstrapperMock.Verify(
                b => b.ConfigureContainer(Moq.It.IsAny<ContainerBuilder>()));
        }
        
        [Fact(DisplayName = "Should invoke correct trigger method")]
        public void should_invoke_correct_trigger_method()
        {
            _subject.Trigger("OtherNonPublicMethod", "some parameter");

            _bootstrapperMock.Verify(
                b => b.OtherNonPublicMethod(Moq.It.IsAny<string>()));
        }
    }
}