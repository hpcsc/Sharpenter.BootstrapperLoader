using Moq;
using Autofac;
using System;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public class WhenTriggeringWithMethodNameAndParameter
    {
        private BootstrapperLoader _subject;
        private ContainerBuilder _containerBuilder;
        private Mock<Bootstrapper> _bootstrapperMock;

        public WhenTriggeringWithMethodNameAndParameter()
        {
            _containerBuilder = new ContainerBuilder();
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<Bootstrapper>();
            _subject = new LoaderBuilder()
                    .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                    .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                    .Build();
        }

        [Fact(DisplayName = "Should invoke default class and container method")]
        public void should_invoke_default_class_and_container_method()
        {
            _subject.Trigger("ConfigureContainer", _containerBuilder);

            _bootstrapperMock.Verify(b => b.ConfigureContainer(_containerBuilder));
        }
    }
}