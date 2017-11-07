using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WhenConditionReturnsTrue
{
    public class WhenMethodNameConventionIsAdded
    {
        private readonly BootstrapperLoader _subject;
        private readonly ContainerBuilder _containerBuilder;
        private readonly Mock<SeventhBootstrapper> _bootstrapperMock;

        public WhenMethodNameConventionIsAdded()
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();  
            
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<SeventhBootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("SeventhBootstrapper")
                    .When(() => true)
                        .AddMethodNameConvention("Development")
                .Build();
        }

        [Fact(DisplayName = "Should invoke configure method by convention with all registered dependencies")]
        public void should_invoke_configure_method_by_convention_with_all_registered_dependencies()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);

            _bootstrapperMock.Verify(
                b =>
                    b.ConfigureDevelopment(Moq.It.Is<IFirstDependency>(v => v is FirstDependency),
                        Moq.It.Is<ISecondDependency>(v => v is SecondDependency)));
        }
        
        [Fact(DisplayName = "Should not invoke default configure method")]
        public void should_not_invoke_default_configure_method()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);
            
            _bootstrapperMock.Verify(
                b => b.Configure(Moq.It.IsAny<IFirstDependency>(), Moq.It.IsAny<ISecondDependency>()),
                Times.Never);
        }
        
        [Fact(DisplayName = "Should invoke configure container method by convention")]
        public void should_invoke_configure_container_method_by_convention()
        {
            _subject.TriggerConfigureContainer(_containerBuilder);

            _bootstrapperMock.Verify(
                b =>
                    b.ConfigureDevelopmentContainer(_containerBuilder));
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