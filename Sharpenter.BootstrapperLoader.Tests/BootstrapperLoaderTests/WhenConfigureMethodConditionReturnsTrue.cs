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
    public class WhenConfigureMethodConditionReturnsTrue
    {
        private BootstrapperLoader _subject;
        private ContainerBuilder _containerBuilder;
        private Mock<Bootstrapper> _bootstrapperMock;

        public WhenConfigureMethodConditionReturnsTrue()
        {
            _containerBuilder = new ContainerBuilder();
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
            _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();

            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<Bootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .Methods()
                    .ClearAll()
                    .Call("Configure").If(() => true)
                .Build();
        }

        [Fact(DisplayName = "Should be called when triggering loader")]
        public void should_be_called_when_triggering_loader()
        {
            _subject.TriggerConfigure(_containerBuilder.Build().Resolve);

            _bootstrapperMock.Verify(
                    b => b.Configure(Moq.It.IsAny<IFirstDependency>(), Moq.It.IsAny<ISecondDependency>()));
        }
    }
}