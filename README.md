# BootstrapperLoader
```
A simple library to load and execute bootstrapper classes in referenced dlls by convention
```

[![alt Build Status](https://ci.appveyor.com/api/projects/status/github/hpcsc/Sharpenter.BootstrapperLoader?branch=master&retina=true "Build Status")](https://ci.appveyor.com/project/hpcsc87/Sharpenter-BootstrapperLoader)

## Usage

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

    public void Configure()
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
        //Need to pass in wrapper AutofacServiceLocator from `Autofac.Extras.CommonServiceLocator` since the project is using CommonServiceLocator to abstract away all IoC container details
        _bootstrapperLoader.TriggerConfigure(new AutofacServiceLocator(_containerBuilder.Build()));
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

or  configure container method to look for (default is ConfigureContainer):

```
_bootstrapperLoader = new LoaderBuilder()
                    .ForClass()
                        .ConfigureContainerWith("SomeConfigureContainerMethod")
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