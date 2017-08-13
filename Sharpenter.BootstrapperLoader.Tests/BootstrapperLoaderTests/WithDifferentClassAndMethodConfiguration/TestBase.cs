using Moq;
using Autofac;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDifferentClassAndMethodConfiguration
{
    public class TestBase
    {
        protected BootstrapperLoader _subject;
        protected ContainerBuilder _containerBuilder;
        protected Mock<SomeBootstrapper> _bootstrapperMock;

        public TestBase()
        {
            _containerBuilder = new ContainerBuilder();
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<SomeBootstrapper>();
            _subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("SomeBootstrapper")
                    .Methods()
                        .ClearAll()
                        .Call("SomeConfigure")
                .Build();
        }
    }
}