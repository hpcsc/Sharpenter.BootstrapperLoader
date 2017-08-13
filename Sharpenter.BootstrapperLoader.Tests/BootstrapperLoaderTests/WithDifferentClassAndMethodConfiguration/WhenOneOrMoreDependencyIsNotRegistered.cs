using Autofac;
using System;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.WithDifferentClassAndMethodConfiguration
{
    public class WhenOneOrMoreDependencyIsNotRegistered : TestBase
    {
        private Exception _exception;

        public WhenOneOrMoreDependencyIsNotRegistered()
        {
            _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
        }

        [Fact(DisplayName = "Should throw resolution exception")]
        public void should_throw_resolution_exception()
        {
            _exception = Assert.Throws<Exception>(() => _subject.TriggerConfigure(_containerBuilder.Build().Resolve));

            Assert.NotNull(_exception);
            Assert.Contains("Could not resolve a service", _exception.Message);
        }
    }
}