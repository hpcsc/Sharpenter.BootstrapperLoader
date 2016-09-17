# BootstrapperLoader
```
A simple library to load and execute bootstrapper classes in referenced dlls by convention
```

[![alt Build Status](https://ci.appveyor.com/api/projects/status/github/hpcsc/Sharpenter.BootstrapperLoader?branch=master&retina=true "Build Status")](https://ci.appveyor.com/project/hpcsc87/Sharpenter-BootstrapperLoader)

## Installation

[![NuGet](https://img.shields.io/nuget/v/Sharpenter.BootstrapperLoader.svg?maxAge=2592000?style=flat-square)](https://www.nuget.org/packages/Sharpenter.BootstrapperLoader/)

```
Install-Package Sharpenter.BootstrapperLoader
```

## Usage Example

This library should be configured and called during the application starts up.

Let's assume following solution structure in a typical ASP.NET/Core layered architecture:
```
MyCoolProject.UI
    - Startup.cs
MyCoolProject.Model
    - ISomeRepository
    - Other model classes
MyCoolProject.Repository
    - SomeRepository: implementation of ISomeRepository
    - EF/NHibernate mapping to database
```

Ideally `UI` project should have reference to only `Model` project, but not `Repository` project, and the dependency direction will be: 

`UI` -> `Model` <- `Repository`

But if your project is using IoC container, the `UI` project will either need to reference `Repository` project to register dependencies to IoC container, or `UI` project will need to reference another `Bootstrapper` project which knows about all projects in the solution.

To solve this dependency issue, the library provides a `BootstrapperLoader` that can load `Bootstrapper` classes in dlls based on convention.

To get started, let's add a `Bootstrapper` class to `MyCoolProject.Repository` that does registration to IoC container by itself (in this case it's `Autofac`):

```
public class Bootstrapper
{
    public void ConfigureContainer(IContainerBuilder builder)
    {
        builder.RegisterType<SomeRepository>().As<ISomeRepository>();
    }

    //Configure can have any number of dependencies injected, as long as those dependencies are already registered with IoC container
    public void Configure(ISomeDependency dependency)
    {
        //Any initialization
    }
}
``` 

In `Startup.cs` in `MyCoolProject.UI`:

```
public class Startup
{
    private BootstrapperLoader _bootstrapperLoader;

    public Startup(IHostingEnvironment env)
    {
        //...

        _bootstrapperLoader = new LoaderBuilder()
                    .Use(new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "MyCoolProject*.dll")) //Look into current directory, grabs all dlls starting with MyCoolProject
                    .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        //Config Autofac to use with ASP.NET Core
        _containerBuilder = new ContainerBuilder();
        //...

        _bootstrapperLoader.Trigger("ConfigureContainer", _containerBuilder);

        //...
    }
     
    public void Configure()
    {
        //Use Resolve() from Autofac container as service locator
        _bootstrapperLoader.TriggerConfigure(_containerBuilder.Build().Resolve);
    }
}
```

## Configuration

During configuration of `BootstrapperLoader`, it's possible to specify class name to look for (default is Bootstrapper):

```
_bootstrapperLoader = new LoaderBuilder()
                    .ForClass()
                        .WithName("SomeBootstrapper")
                    .Build();
```

or pass some parameter to `Bootstrapper` constructor:

```
_bootstrapperLoader = new LoaderBuilder()
                    .ForClass()
                        .HasConstructorParameter<ISomeDependency>(new SomeDependency())
                    .Build();
```

or configure method to look for (default is Configure):

```
_bootstrapperLoader = new LoaderBuilder()
                    .ForClass()
                        .Methods()
                            .Call("SomeConfigureMethod")
                    .Build();
```

or configure method and condition when should that method is called:

```
_bootstrapperLoader = new LoaderBuilder()
                    .ForClass()
                        .Methods()
                            .Call("SomeConfigureMethod").If(() => /*some expression returning boolean*/)
                    .Build();
```

or specify where to load assemblies (default is loading all dlls from current directory):

```
_bootstrapperLoader = new LoaderBuilder()
                        .Use(new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "MyCoolProject*.dll")) //Look into current directory, grabs all dlls starting with MyCoolProject
                        .Build();
```

or combination of above configuration

You can also create new Assembly Provider class, to customize the source of assemblies provided to the loader. At the moment, there are 2 classes provided:
```
- FileSystemAssemblyProvider
- InMemoryAssemblyProvider
```

## Trigger bootstrapper from root project

`BootstrapperLoader` provides 2 methods to trigger methods in sub-projects `Bootstrapper` class:

- `Trigger<TArg>(string methodName, TArg parameter)`

When this method is called, it will look for methods with specified name in `Bootstrapper` classes in sub-projects and invoke those, passing in provided parameter. Most of the time, this method should be used to trigger IoC registration in sub-projects, passing in IoC container from root project as parameter. Although with its general signature, it can be used for other purpose.

- `TriggerConfigure(Func<Type, object> serviceLocator = null)`

This method is triggered when it's the right time to do any non-IoC configuration/initialization (.e.g. AutoMapper setting up). It can be called with or without `serviceLocator` parameter

This method takes `Func<Type, object>` as its parameter to allow `Configure` method in `Bootstrapper` classes to take in any number of dependencies (as long as those dependencies can be resolved using serviceLocator func). It works in the same way with `Startup.Configure` in ASP.NET Core

`Func<Type, object>` is used here to ensure this library is not dependent on any specific IoC container. Most IoC container should support a method with this signature (.e.g. in `Autofac`, it's `Resolve()` method) 

When it's called without `serviceLocator` parameter, it will look for only `Configure()` method (without any parameter) in Bootstrapper classes