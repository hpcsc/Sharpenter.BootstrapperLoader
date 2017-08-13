
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Helpers.MethodInfoExtensionsTests.InvokeWithDynamicallyResolvedParametersMethod
{
    public class WhenAllDependenciesAreRegisteredWithContainer
    {
        private Mock<ISomeInterface1> _dependency1Mock;
        private Mock<ISomeInterface2> _dependency2Mock;
        private Mock<SomeClass> _subject;

        public WhenAllDependenciesAreRegisteredWithContainer()
        {
            _dependency1Mock = new Mock<ISomeInterface1>();
            _dependency2Mock = new Mock<ISomeInterface2>();
            _subject = new Mock<SomeClass>();
        }

        [Fact(DisplayName = "Should invoke method with all parameters satisfied by container")]
        public void should_invoke_method_with_all_parameters_satisfied_by_container()
        {
            _subject.Object
                .GetType()
                .GetMethod("Configure")
                .InvokeWithDynamicallyResolvedParameters(_subject.Object,
                    type => type == typeof(ISomeInterface1)
                        ? (object)_dependency1Mock.Object
                        : _dependency2Mock.Object);

            _subject.Verify(s => s.Configure(_dependency1Mock.Object, _dependency2Mock.Object));
        }
    }
}