namespace Sharpenter.BootstrapperLoader.Tests.Helpers
{
    public interface ISomeInterface1 { }
    public interface ISomeInterface2 { }

    public class SomeClass
    {
        public virtual void Configure(ISomeInterface1 dependency1, ISomeInterface2 dependency2)
        {

        }
    }
}