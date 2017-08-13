using Moq;
using Autofac;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDefaultMethodNameConfiguration
{
    public class TestBase
    {
        protected BootstrapperLoader _subject;
        protected ContainerBuilder _containerBuilder;
        protected Mock<Bootstrapper> _bootstrapperMock;

        public TestBase()
        {        
            _containerBuilder = new ContainerBuilder();
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<Bootstrapper>();
            _subject = new LoaderBuilder()
                    .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                    .UseInstanceCreator(MockInstanceCreator(_bootstrapperMock.Object))
                    .Build();                
        }

        private IAmInstanceCreator MockInstanceCreator(object instance)
        {
            return new FakeCreator(instance);
        }
    }
}