using Autofac;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests
{
    public interface IFirstDependency { }
    public interface ISecondDependency { }

    public class FirstDependency : IFirstDependency { }
    public class SecondDependency : ISecondDependency { }

    public class Bootstrapper
    {
        public virtual void ConfigureContainer(ContainerBuilder container) { }

        public virtual void Configure(IFirstDependency first, ISecondDependency second) { }
    }

    public class SomeBootstrapper
    {
        public virtual void ConfigureContainer(ContainerBuilder container) { }

        public virtual void Configure(IFirstDependency first, ISecondDependency second) { }
    }

    public class ThirdBootstrapper
    {
        public virtual void Configure() { }

        public virtual void Configure(IFirstDependency first, ISecondDependency second) { }
    }

    public class FourthBootstrapper
    {
        public FourthBootstrapper(IFirstDependency dependency) { }
    }

    public class FifthBootstrapper
    {
        public virtual void ConfigureContainer(ContainerBuilder container) { }
        
        public virtual void SomeConfigureContainer(ContainerBuilder container) { }

        public virtual void Configure(IFirstDependency first, ISecondDependency second) { }
        
        public virtual void SomeConfigure(IFirstDependency first, ISecondDependency second) { }
    }
    
    public class SixthBootstrapper
    {
        public virtual void SomeMethod(string parameter) { }
    }
    
    public class SeventhBootstrapper
    {
        public virtual void ConfigureContainer(ContainerBuilder container) { }
        public virtual void ConfigureDevelopmentContainer(ContainerBuilder container) { }
        public virtual void Configure(IFirstDependency first, ISecondDependency second) { }
        public virtual void ConfigureDevelopment(IFirstDependency first, ISecondDependency second) { }
    }
}