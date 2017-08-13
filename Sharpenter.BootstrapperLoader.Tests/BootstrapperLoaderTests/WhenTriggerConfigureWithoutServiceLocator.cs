using Moq;
using Autofac;
using System;
using System.Linq;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public class WhenTriggerConfigureWithoutServiceLocator
    {
        private BootstrapperLoader _subject;
        private Mock<ThirdBootstrapper> _bootstrapperMock;

        public WhenTriggerConfigureWithoutServiceLocator()
        {
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<ThirdBootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("ThirdBootstrapper")
                .Build();
        }

        [Fact(DisplayName = "Should trigger only configure methods without parameters")]
        public void should_trigger_only_configure_methods_without_parameters()
        {
            _subject.TriggerConfigure();

            _bootstrapperMock.Verify(b => b.Configure());
            _bootstrapperMock.Verify(b => b.Configure(Moq.It.IsAny<IFirstDependency>(), Moq.It.IsAny<ISecondDependency>()), Times.Never);
        }
    }
}