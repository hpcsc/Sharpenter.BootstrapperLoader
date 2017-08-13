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
    public class WhenConstructorParameterIsProvided
    {
        private BootstrapperLoader _subject;
        private static Assembly _testDll;

        public WhenConstructorParameterIsProvided()
        {
            _testDll = Assembly.GetExecutingAssembly();
        }

        [Fact(DisplayName = "Should create bootstrapper using constructor with that signature")]
        public void should_create_bootstrapper_using_constructor_with_that_signature()
        {
            _subject = new LoaderBuilder()
                            .Use(new InMemoryAssemblyProvider(() => new[] { _testDll }))
                            .ForClass()
                                .WithName("FourthBootstrapper")
                                .HasConstructorParameter<IFirstDependency>(new FirstDependency())
                            .Build();

            Assert.Equal(1, _subject.Bootstrappers.Count);
            Assert.Contains("BootstrapperLoaderTests.FourthBootstrapper", _subject.Bootstrappers.First().GetType().FullName);
        }
    }
}