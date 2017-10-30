using System;
using System.Diagnostics;
using AutoMapper;

namespace Sharpenter.BootstrapperLoader.Tests.BootstrapperLoaderTests.DummyAssembly
{
    internal class A {}
    internal class B {}

    public class Bootstrapper
    {
        public virtual void Configure()
        {
             Mapper.Initialize(cfg => cfg.CreateMap<A, B>());            
        }
    }
}
