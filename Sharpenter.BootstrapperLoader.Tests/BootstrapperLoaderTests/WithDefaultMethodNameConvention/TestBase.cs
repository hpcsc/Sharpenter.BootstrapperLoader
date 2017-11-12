using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDefaultMethodNameConvention
{
    public class TestBase
    {
        protected readonly BootstrapperLoader Subject;
        protected readonly ContainerBuilder ContainerBuilder;
        protected readonly Mock<Bootstrapper> BootstrapperMock;

        protected TestBase()
        {        
            ContainerBuilder = new ContainerBuilder();
            var testDll = Assembly.GetExecutingAssembly();
            BootstrapperMock = new Mock<Bootstrapper>();
            Subject = new LoaderBuilder()
                    .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                    .UseInstanceCreator(new FakeCreator(BootstrapperMock.Object))
                    .ForClass()
                        .AddDefaultMethodNameConvention()
                    .Build();                
        }
    }
}