using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Machine.Specifications;
using Moq;
using Sharpenter.BootstrapperLoader.Internal;
using It = Machine.Specifications.It;

namespace Sharpenter.BootstrapperLoader.Tests
{
    [Subject(typeof(BootstrapperLoader))]
    public class BootstrapperLoaderTests
    {
        public interface IFirstDependency { }
        public interface ISecondDependency { }

        public class FirstDependency : IFirstDependency { }
        public class SecondDependency : ISecondDependency { }

        public class Bootstrapper
        {
            public virtual void ConfigureContainer(ContainerBuilder container)
            {

            }

            public virtual void Configure(IFirstDependency first, ISecondDependency second)
            {
                
            }
        }

        public class SomeBootstrapper
        {
            public virtual void SomeConfigureContainer(ContainerBuilder container)
            {

            }

            public virtual void SomeConfigure(IFirstDependency first, ISecondDependency second)
            {

            }
        }

        public class With_default_method_name_configuration
        {
            private Establish context = () =>
            {
                var testDll = Assembly.GetExecutingAssembly();
                _bootstrapperMock = new Mock<Bootstrapper>();
                _subject = new BootstrapperLoader(new LoaderConfig
                {
                    AssemblyProvider = new InMemoryAssemblyProvider(() => new [] { testDll })
                }, type => _bootstrapperMock.Object);
                _subject.Initialize();

                _containerBuilder = new ContainerBuilder();
            };

            public class When_triggering_configure_container
            {
                private Because of = () => _subject.TriggerConfigureContainer(_containerBuilder);

                private It should_invoke_default_class_and_container_method =
                    () => _bootstrapperMock.Verify(b => b.ConfigureContainer(_containerBuilder));
            }

            public class When_triggering_configure_with_all_dependencies_registered
            {
                private Establish context = () =>
                {
                    _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
                    _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();
                };

                private Because of =
                    () => _subject.TriggerConfigure(new AutofacServiceLocator(_containerBuilder.Build()));

                private It should_invoke_default_class_and_configure_method_with_all_registered_dependencies =
                    () =>
                        _bootstrapperMock.Verify(
                            b =>
                                b.Configure(Moq.It.Is<IFirstDependency>(v => v is FirstDependency),
                                    Moq.It.Is<ISecondDependency>(v => v is SecondDependency)));
            }

            public class When_one_or_more_dependency_is_not_registered
            {
                private Establish context = () =>
                {
                    _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
                };

                private Because of =
                    () =>
                        _exception = Catch.Exception(
                            () => _subject.TriggerConfigure(new AutofacServiceLocator(_containerBuilder.Build())));

                private It should_throw_resolution_exception = () =>
                {
                    _exception.ShouldNotBeNull();
                    _exception.Message.ShouldEqual(
                        "Could not resolve a service of type 'Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests+ISecondDependency' for the parameter 'second' of method 'Configure' on type 'Castle.Proxies.BootstrapperProxy'.");
                };

                private static Exception _exception;
            }

            private static BootstrapperLoader _subject;
            private static ContainerBuilder _containerBuilder;
            private static Mock<Bootstrapper> _bootstrapperMock;
        }

        public class With_different_class_and_method_configuration
        {
            private Establish context = () =>
            {
                var testDll = Assembly.GetExecutingAssembly();
                _bootstrapperMock = new Mock<SomeBootstrapper>();
                _subject = new BootstrapperLoader(new LoaderConfig
                {
                    AssemblyProvider = new InMemoryAssemblyProvider(() => new[] { testDll }),
                    BootstrapperClassName = "SomeBootstrapper",
                    ConfigureContainerMethodName = "SomeConfigureContainer",
                    ConfigureMethods = new Dictionary<string, Func<bool>>
                    {
                        { "SomeConfigure", () => true }
                    }
                }, type => _bootstrapperMock.Object);
                _subject.Initialize();

                _containerBuilder = new ContainerBuilder();
            };

            public class When_triggering_configure_container
            {
                private Because of = () => _subject.TriggerConfigureContainer(_containerBuilder);

                private It should_invoke_correct_class_and_container_method =
                    () => _bootstrapperMock.Verify(b => b.SomeConfigureContainer(_containerBuilder));
            }

            public class When_triggering_configure_with_all_dependencies_registered
            {
                private Establish context = () =>
                {
                    _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
                    _containerBuilder.RegisterType<SecondDependency>().As<ISecondDependency>();
                };

                private Because of =
                    () => _subject.TriggerConfigure(new AutofacServiceLocator(_containerBuilder.Build()));

                private It should_invoke_correct_class_and_configure_method_with_all_registered_dependencies =
                    () =>
                        _bootstrapperMock.Verify(
                            b =>
                                b.SomeConfigure(Moq.It.Is<IFirstDependency>(v => v is FirstDependency),
                                    Moq.It.Is<ISecondDependency>(v => v is SecondDependency)));
            }

            public class When_one_or_more_dependency_is_not_registered
            {
                private Establish context = () =>
                {
                    _containerBuilder.RegisterType<FirstDependency>().As<IFirstDependency>();
                };

                private Because of =
                    () =>
                        _exception = Catch.Exception(
                            () => _subject.TriggerConfigure(new AutofacServiceLocator(_containerBuilder.Build())));

                private It should_throw_resolution_exception = () =>
                {
                    _exception.ShouldNotBeNull();
                    _exception.Message.ShouldEqual(
                        "Could not resolve a service of type 'Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests+ISecondDependency' for the parameter 'second' of method 'SomeConfigure' on type 'Castle.Proxies.SomeBootstrapperProxy'.");
                };

                private static Exception _exception;
            }

            private static BootstrapperLoader _subject;
            private static ContainerBuilder _containerBuilder;
            private static Mock<SomeBootstrapper> _bootstrapperMock;
        }
    }
}
