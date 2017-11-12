using System.Reflection;
using Autofac;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDifferentClassName
{
    public class TestBase
    {
        protected readonly BootstrapperLoader Subject;
        protected readonly ContainerBuilder ContainerBuilder;
        protected readonly Mock<SomeBootstrapper> BootstrapperMock;

        protected TestBase()
        {
            ContainerBuilder = new ContainerBuilder();
            var testDll = Assembly.GetExecutingAssembly();
            BootstrapperMock = new Mock<SomeBootstrapper>();
            Subject = new LoaderBuilder()
                .UseInstanceCreator(new FakeCreator(BootstrapperMock.Object))
                .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                .ForClass()
                    .WithName("SomeBootstrapper")
                    .AddDefaultMethodNameConvention()
                .Build();
        }
    }
}