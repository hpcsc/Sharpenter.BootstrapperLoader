using System;
using Autofac;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDefaultMethodNameConvention
{
    public class WhenOneOrMoreDependencyIsNotRegistered : TestBase
    {
        private Exception _exception;
        public WhenOneOrMoreDependencyIsNotRegistered()
        {
            ContainerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
        }

        [Fact(DisplayName = "Should throw resolution exception")]
        public void should_throw_resolution_exception()
        {
            _exception = Assert.Throws<Exception>(
                                () => Subject.TriggerConfigure(ContainerBuilder.Build().Resolve));

            Assert.NotNull(_exception);
            Assert.Contains("Could not resolve a service", _exception.Message);
        }
    }
}