using System.CodeDom;
using Machine.Specifications;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using Sharpenter.BootstrapperLoader.Helpers;
using It = Machine.Specifications.It;

namespace Sharpenter.BootstrapperLoader.Tests.Builder
{
    [Subject(typeof(LoaderBuilder))]
    public class LoaderBuilderTests
    {
        private static Mock<IAssemblyProvider> _assemblyProviderMock;
        private static BootstrapperLoader _loader;

        private Establish context = () =>
        {
            _assemblyProviderMock = new Mock<IAssemblyProvider>();
        };

        public class When_provided_an_assembly_loader
        {
            private Because of = () => _loader = new LoaderBuilder()
                                                        .Use(_assemblyProviderMock.Object)
                                                        .Build();

            private It should_initialize_loader_with_this_assembly_loader =
                () => _loader.Config.AssemblyProvider.ShouldEqual(_assemblyProviderMock.Object);
        }

        public class When_config_class_name_using_builder
        {
            private Because of = () => _loader = new LoaderBuilder()
                                                        .Use(_assemblyProviderMock.Object)
                                                        .ForClass()
                                                            .WithName("SomeBootstrapper")
                                                        .Build();

            private It should_initialize_loader_with_that_class_name =
                () => _loader.Config.BootstrapperClassName.ShouldEqual("SomeBootstrapper");            
        }

        public class When_config_class_constructor_parameters_using_builder
        {
            private Because of = () => _loader = new LoaderBuilder()
                                                        .Use(_assemblyProviderMock.Object)
                                                        .ForClass()
                                                            .HasConstructorParameter("some string")
                                                        .Build();

            private It should_initialize_loader_with_those_constructor_parameters =
                () => _loader.Config.InstanceCreator.ShouldBeOfExactType<ExpressionCreator<string>>();
        }

        public class When_config_configure_container_name_using_builder
        {
            private Because of = () => _loader = new LoaderBuilder()
                                                        .Use(_assemblyProviderMock.Object)
                                                        .ForClass()
                                                            .ConfigureContainerWith("SomeConfigureContainer")
                                                        .Build();

            private It should_initialize_loader_with_that_configure_container_method_name =
                () => _loader.Config.ConfigureContainerMethodName.ShouldEqual("SomeConfigureContainer");            
        }

        public class When_clearing_all_config_method_configurations
        {
            private Because of = () => _loader = new LoaderBuilder()
                                                        .Use(_assemblyProviderMock.Object)
                                                        .ForClass()
                                                            .Methods()
                                                                .ClearAll()
                                                        .Build();

            private It should_not_have_any_method_configurations_in_loader =
                () => _loader.Config.ConfigureMethods.Count.ShouldEqual(0);
        }

        public class When_config_method_without_if_condition
        {
            private Because of = () => _loader = new LoaderBuilder()
                                                        .Use(_assemblyProviderMock.Object)
                                                        .ForClass()
                                                            .Methods()
                                                                .Call("SomeConfigure")
                                                        .Build();

            private It should_always_call_that_method =
                () => _loader.Config.ConfigureMethods["SomeConfigure"]().ShouldBeTrue();
        }
    }
}
