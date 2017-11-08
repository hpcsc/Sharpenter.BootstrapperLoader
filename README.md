# BootstrapperLoader
```
A simple library to load and execute bootstrapper classes in referenced dlls by convention
```

[![alt Build Status](https://ci.appveyor.com/api/projects/status/github/hpcsc/Sharpenter.BootstrapperLoader?branch=master&retina=true "Build Status")](https://ci.appveyor.com/project/hpcsc87/Sharpenter-BootstrapperLoader)

## CodeProject article

My article on CodeProject on design and implementation of this library: [https://www.codeproject.com/Articles/1162975/Bootstrapper-Loader-for-Layered-Architecture](https://www.codeproject.com/Articles/1162975/Bootstrapper-Loader-for-Layered-Architecture)

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

        _bootstrapperLoader.TriggerConfigureContainer(_containerBuilder);

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

### Default Configuration

By default, `BootstrapperLoader` has following settings:
- use `FileSystemAssemblyProvider` to look for all `*.dll` in current folder (`Directory.GetCurrentDirectory()`)
- `BootstrapperLoader.TriggerConfigure` looks for `Configure()` method in any class with name `Bootstrapper` in dlls found above
- `BootstrapperLoader.TriggerConfigureContainer` looks for `ConfigureContainer()` method in any class with name `Bootstrapper` in dlls found above

### Configure LoaderBuilder

- `WithName("SomeBootstrapper")`: look for class with name `SomeBootstrapper` instead of `Bootstrapper`

    Example:

    ```
    _bootstrapperLoader = new LoaderBuilder()
                        .ForClass()
                            .WithName("SomeBootstrapper")
                        .Build();
    ```
- `HasConstructorParameter<ISomeDependency>()`: when creating `Bootstrapper` instance, use constructor that takes `ISomeDependency` parameter

    Example:

    ```
    _bootstrapperLoader = new LoaderBuilder()
                        .ForClass()
                            .HasConstructorParameter<ISomeDependency>(new SomeDependency())
                        .Build();
    ```

- `When(condition).CallConfigure("SomeConfigure")`: when calling `BootstrapperLoader.TriggerConfigure()`, if `condition` invocation is evaluated to true, call `SomeConfigure()` method in `Bootstrapper` classes in addition to `Configure()`

    Example:

    ```
    _bootstrapperLoader = new LoaderBuilder()
                        .ForClass()
                            .When(env.IsDevelopment)
                                .CallConfigure("SomeConfigure")
                        .Build();
    ```

- `When(condition).CallConfigureContainer("SomeConfigureContainer")`: when calling `BootstrapperLoader.TriggerConfigureContainer()`, if `condition` invocation is evaluated to true, call `SomeConfigureContainer()` method in `Bootstrapper` classes in addition to `ConfigureContainer()`

    Example:

    ```
    _bootstrapperLoader = new LoaderBuilder()
                        .ForClass()
                            .When(env.IsDevelopment)
                                .CallConfigureContainer("SomeConfigureContainer")
                        .Build();
    ```

- `When(condition).AddMethodNameConvention("Development")`: when calling `BootstrapperLoader.TriggerConfigure()`/ `BootstrapperLoader.TriggerConfigureContainer()`, if `condition` invocation is evaluated to true, call `SomeConfigure()`/`SomeConfigureContainer()` method in `Bootstrapper` classes in addition to `Configure()`/`ConfigureContainer()`

    Example:

    ```
    _bootstrapperLoader = new LoaderBuilder()
                        .ForClass()
                            .When(env.IsDevelopment)
                                .AddMethodNameConvention("Development")
                        .Build();
    ```


- `Use()`: specify an alternative assembly provider:

    Example:

    ```
    _bootstrapperLoader = new LoaderBuilder()
                            .Use(new FileSystemAssemblyProvider(Directory.GetCurrentDirectory(), "MyCoolProject*.dll")) //Look into current directory, grabs all dlls starting with MyCoolProject
                            .Build();
    ```

You can also create new Assembly Provider class, to customize the source of assemblies provided to the loader. At the moment, there are 2 classes provided:
```
- FileSystemAssemblyProvider
- InMemoryAssemblyProvider
```

## Trigger bootstrapper from root project

`BootstrapperLoader` provides 3 methods to trigger methods in sub-projects `Bootstrapper` class:

- `TriggerConfigureContainer<TArg>(TArg parameter)`

This method should be used when root project is doing IoC registration. Its parameter is usually IoC container or container builder. This method will look for `ConfigureContainer` method in `Bootstrapper` classes and pass in the parameter, allow `Bootstrapper` classes to register child projects' dependencies to IoC container

- `TriggerConfigure(Func<Type, object> serviceLocator = null)`

This method should be used when it's the right time to do any non-IoC configuration/initialization (.e.g. AutoMapper setting up). It can be called with or without `serviceLocator` parameter

This method takes `Func<Type, object>` as its parameter to allow `Configure` method in `Bootstrapper` classes to take in any number of dependencies (as long as those dependencies can be resolved using serviceLocator func). It works in the same way with `Startup.Configure` in ASP.NET Core

`Func<Type, object>` is used here to ensure this library is not dependent on any specific IoC container. Most IoC container should support a method with this signature (.e.g. in `Autofac`, it's `Resolve()` method)

When it's called without `serviceLocator` parameter, it will look for only `Configure()` method (without any parameter) in `Bootstrapper` classes

- `Trigger<TArg>(string methodName, TArg parameter)`

When this method is called, it will look for methods with specified name in `Bootstrapper` classes in sub-projects and invoke those, passing in provided parameter. This method is for any other situation where your project cannot use above 2 methods.
