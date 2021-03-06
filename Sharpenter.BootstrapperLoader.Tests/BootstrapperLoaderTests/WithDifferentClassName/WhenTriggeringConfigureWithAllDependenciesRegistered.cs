using Autofac;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDifferentClassName
{
    public class WhenTriggeringConfigureWithAllDependenciesRegistered : TestBase
    {
        public WhenTriggeringConfigureWithAllDependenciesRegistered()
        {
            ContainerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            ContainerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();            
        }

        [Fact(DisplayName = "Should invoke correct class and configure method with all registered dependencies")]
        public void should_invoke_correct_class_and_configure_method_with_all_registered_dependencies()
        {
            Subject.TriggerConfigure(ContainerBuilder.Build().Resolve);

            BootstrapperMock.Verify(
                    b =>
                        b.Configure(Moq.It.Is<IFirstDependency>(v => v is FirstDependency),
                            Moq.It.Is<ISecondDependency>(v => v is SecondDependency)));
        }
    }
}