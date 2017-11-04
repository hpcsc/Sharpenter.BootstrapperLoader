using Moq;
using Autofac;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public class WhenDependencyAssemblyUsingGlobalNugetStore
    {
        private BootstrapperLoader _subject;

        [Fact(DisplayName = "Should not throw exception during initialization and invocation")]
        public void Should_not_throw_exception_during_initialization_and_invocation()
        {
            var dummyAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), GetDummyAssemblyPath());
            _subject = new LoaderBuilder()
                            .Use(new FileSystemAssemblyProvider(dummyAssemblyPath,
                                                                "Sharpenter.BootstrapperLoader.Tests.DummyAssembly.dll"))
                            .Build();

            _subject.TriggerConfigure();
            Assert.True(true);
        }

        private static string GetDummyAssemblyPath()
        {
            // This test project doesn't have direct reference to DummyAssembly project, to simulate real situation where
            // main project doesn't have direct references to projects with bootstrapper classes
            var configuration = GetBuildConfiguration();
            #if NET452
            return "../../../../Sharpenter.BootstrapperLoader.Tests.DummyAssembly/bin/" + configuration + "/net452";
            #else
            return "../../../../Sharpenter.BootstrapperLoader.Tests.DummyAssembly/bin/" + configuration + "/netstandard2.0";
            #endif
        }

        private static string GetBuildConfiguration()
        {
            #if DEBUG
            return "Debug";
            #else
            return "Release";
            #endif            
        }
    }
}
