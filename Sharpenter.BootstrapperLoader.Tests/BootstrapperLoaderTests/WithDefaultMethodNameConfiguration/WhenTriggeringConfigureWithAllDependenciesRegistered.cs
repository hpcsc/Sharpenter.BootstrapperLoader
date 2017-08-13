using Moq;
using Autofac;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDefaultMethodNameConfiguration
{
    public class WhenTriggeringConfigureWithAllDependenciesRegistered : TestBase
    {
        public WhenTriggeringConfigureWithAllDependenciesRegistered()
        {
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();
        }

        [Fact(DisplayName = "Should invoke default class and configure method with all registered dependencies")]
        public void should_invoke_default_class_and_configure_method_with_all_registered_dependencies()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);

            _bootstrapperMock.Verify(
                    b =>
                        b.Configure(Moq.It.Is<IFirstDependency>(v => v is FirstDependency),
                            Moq.It.Is<ISecondDependency>(v => v is SecondDependency)));
        }
    }
}