using Machine.Specifications;
using Moq;
using Sharpenter.BootstrapperLoader.Builder;
using It = Machine.Specifications.It;

namespace Sharpenter.BootstrapperLoader.Tests.Builder
{
    [Subject(typeof(LoaderBuilder))]
    public class LoaderBuilderTests
    {
        private static LoaderBuilder _subject;

        private Establish context = () =>
        {
            _subject = new LoaderBuilder();
        };

        public class UseMethod
        {
            private Establish context = () =>
            {
                _assemblyProviderMock = new Mock<IAssemblyProvider>();                
            };

            private Because of = () => _subject.Use(_assemblyProviderMock.Object);

            private It should_set_config_to_correct_assembly_provider =
                () => _subject.Config.AssemblyProvider.ShouldEqual(_assemblyProviderMock.Object);
            
            private static Mock<IAssemblyProvider> _assemblyProviderMock;
        }

        public class ForClassMethod
        {
            private Because of = () => _classConfig = _subject.ForClass();

            private It should_create_class_config_builder_with_existing_configuration =
                () => _classConfig.Config.ShouldEqual(_subject.Config);

            private static LoaderClassConfigBuilder _classConfig;
        }
    }
}
