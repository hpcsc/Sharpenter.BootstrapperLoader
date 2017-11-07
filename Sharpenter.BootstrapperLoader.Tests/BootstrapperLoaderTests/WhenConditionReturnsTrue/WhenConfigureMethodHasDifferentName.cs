using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WhenConditionReturnsTrue
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
                .When(() => true)
                    .CallConfigure("SomeConfigure")
                .Build();
        }

        [Fact(DisplayName = "Should invoke correct class and configure method with all registered dependencies")]
        public void should_invoke_correct_class_and_configure_method_with_all_registered_dependencies()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);

            _bootstrapperMock.Verify(
                b =>
                    b.SomeConfigure(Moq.It.Is<IFirstDependency>(v => v is FirstDependency),
                        Moq.It.Is<ISecondDependency>(v => v is SecondDependency)));
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