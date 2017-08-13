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
    public class WhenCreatedAndInitialized
    {
        private BootstrapperLoader _subject;
        private static Assembly _testDll;

        public WhenCreatedAndInitialized()
        {
            _testDll = Assembly.GetExecutingAssembly();
        }

        [Fact(DisplayName = "Should create bootstrapper using default constructor")]
        public void should_create_bootstrapper_using_default_constructor()
        {
            _subject = new LoaderBuilder()
                            .Use(new InMemoryAssemblyProvider(() => new[] { _testDll }))
                            .Build();

            Assert.Equal(1, _subject.Bootstrappers.Count);
            Assert.Contains("BootstrapperLoaderTests.Bootstrapper", _subject.Bootstrappers.First().GetType().FullName);
        }
    }
}