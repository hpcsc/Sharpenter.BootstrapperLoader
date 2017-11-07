using Moq;
using System.Reflection;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public class WhenTriggeringWithMethodNameAndParameter
    {
        private readonly BootstrapperLoader _subject;
        private readonly Mock<SixthBootstrapper> _bootstrapperMock;

        public WhenTriggeringWithMethodNameAndParameter()
        {
            var testDll = Assembly.GetExecutingAssembly();
            _bootstrapperMock = new Mock<SixthBootstrapper>();
            _subject = new LoaderBuilder()
                    .Use(new InMemoryAssemblyProvider(() => new[] { testDll }))
                    .UseInstanceCreator(new FakeCreator(_bootstrapperMock.Object))
                    .Build();
        }

        [Fact(DisplayName = "Should invoke correct method with parameter")]
        public void should_invoke_default_class_and_container_method()
        {
            _subject.Trigger("SomeMethod", "some-parameter");

            _bootstrapperMock.Verify(b => b.SomeMethod("some-parameter"));
        }
    }
}