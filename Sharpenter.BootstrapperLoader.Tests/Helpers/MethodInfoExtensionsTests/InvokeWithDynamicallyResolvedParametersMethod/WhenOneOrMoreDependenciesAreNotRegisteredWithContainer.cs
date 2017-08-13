using Moq;
using System;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using Sharpenter.BootstrapperLoader;
using Xunit;

namespace Sharpenter.BootstrapperLoader.Tests.Helpers.MethodInfoExtensionsTests.InvokeWithDynamicallyResolvedParametersMethod
{
    public class WhenOneOrMoreDependenciesAreNotRegisteredWithContainer
    {
        private Mock<ISomeInterface1> _dependency1Mock;
        private Mock<ISomeInterface2> _dependency2Mock;
        private Mock<SomeClass> _subject;
        private static Exception _exception;

        public WhenOneOrMoreDependenciesAreNotRegisteredWithContainer()
        {
            _dependency1Mock = new Mock<ISomeInterface1>();
            _dependency2Mock = new Mock<ISomeInterface2>();
            _subject = new Mock<SomeClass>();
        }

        [Fact(DisplayName = "Should throw exception when invoking")]
        public void should_throw_exception_when_invoking()
        {
            _exception = Assert.Throws<Exception>(() => _subject.Object
                                                            .GetType()
                                                            .GetMethod("Configure")
                                                            .InvokeWithDynamicallyResolvedParameters(_subject.Object, Resolve));

            Assert.NotNull(_exception);
            Assert.Contains("Could not resolve a service", _exception.Message);
        }

        private object Resolve(Type type)
        {
            if (type == typeof(ISomeInterface1))
            {
                return _dependency1Mock.Object;
            }

            //Simulate failing to resolve dependency from IoC container
            throw new Exception($"Failed to resolve type {type.FullName}");
        }  
    }
}