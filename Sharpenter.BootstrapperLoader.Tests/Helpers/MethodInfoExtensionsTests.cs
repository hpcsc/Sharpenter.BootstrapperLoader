using System;
using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Sharpenter.BootstrapperLoader.Helpers;
using It = Machine.Specifications.It;

namespace Sharpenter.BootstrapperLoader.Tests.Helpers
{
    [Subject(typeof(MethodInfoExtensions))]
    public class MethodInfoExtensionsTests
    {
        public interface ISomeInterface1 { }
        public interface ISomeInterface2 { }

        public class SomeClass
        {
            public virtual void Configure(ISomeInterface1 dependency1, ISomeInterface2 dependency2)
            {
                
            }
        }

        public class InvokeWithDynamicallyResolvedParametersMethod
        {
            private static Mock<IServiceLocator> _serviceLocatorMock;
            private static Mock<ISomeInterface1> _dependency1Mock;
            private static Mock<ISomeInterface2> _dependency2Mock;
            private static Mock<SomeClass> _subject;

            private Establish context = () =>
            {
                _dependency1Mock = new Mock<ISomeInterface1>();
                _dependency2Mock = new Mock<ISomeInterface2>();
                _serviceLocatorMock = new Mock<IServiceLocator>();
                _subject = new Mock<SomeClass>();
            };

            public class When_all_dependencies_are_registered_with_container
            {
                private Establish context = () =>
                {
                    _serviceLocatorMock.Setup(l => l.GetInstance(typeof(ISomeInterface1)))
                        .Returns(_dependency1Mock.Object);
                    _serviceLocatorMock.Setup(l => l.GetInstance(typeof(ISomeInterface2)))
                        .Returns(_dependency2Mock.Object);
                };

                private Because of =
                    () =>
                        _subject.Object
                                .GetType()
                                .GetMethod("Configure")
                                .InvokeWithDynamicallyResolvedParameters(_subject.Object, _serviceLocatorMock.Object);

                private It should_invoke_method_with_all_parameters_satisfied_by_container =
                    () => _subject.Verify(s => s.Configure(_dependency1Mock.Object, _dependency2Mock.Object));                
            }

            public class When_one_or_more_dependencies_are_not_registered_with_container
            {
                private Establish context = () =>
                {
                    _serviceLocatorMock.Setup(l => l.GetInstance(typeof(ISomeInterface1)))
                        .Returns(_dependency1Mock.Object);
                    _serviceLocatorMock.Setup(l => l.GetInstance(typeof(ISomeInterface2)))
                        .Throws<ActivationException>();
                };

                private Because of = () => _exception = Catch.Exception(() =>
                    _subject.Object
                        .GetType()
                        .GetMethod("Configure")
                        .InvokeWithDynamicallyResolvedParameters(_subject.Object, _serviceLocatorMock.Object));

                private It should_throw_exception_when_invoking = () =>
                {
                    _exception.ShouldNotBeNull();
                    _exception.Message.ShouldEqual(
                        "Could not resolve a service of type 'Sharpenter.BootstrapperLoader.Tests.Helpers.MethodInfoExtensionsTests+ISomeInterface2' for the parameter 'dependency2' of method 'Configure' on type 'Castle.Proxies.SomeClassProxy'.");
                };

                private static Exception _exception;
            }
        }

    }
}
