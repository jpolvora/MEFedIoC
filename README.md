MEFedIoC
========

MEFedIoC is a project that contains extension methods that will allows you to use MEF as an IoC container.

Install: You can use the NuGet package MEFedIoC
	Install-Package MEFedIoC

Usage:

Like any IoC container, you have to create an instance of a container before registering/resolving.

In MEF, you must create an instance of CompositionContainer class, passing to it the catalogs that contain the types that will be registered/resolved.

The catalogs can be any instance derived from ComposablePartCatalog. MEF comes with AssemblyCatalog, AggregateCatalog and others.

AssemblyCatalog must be constructed with a single assembly associated with it.
AggregateCatalog is a special catalog that you can add many ComposablePartCatalog instances.

Let's create a method that returns a fresh container ready to use:

	using MEFedIoC;
	
	public CompositionContainer GetContainer()
    {
		var multiCatalog = new AggregateCatalog();
		var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
		multiCatalog.Catalogs.Add(catalog);
		
		var container = new CompositionContainer(multiCatalog);
		container.ComposeParts();
		return container;
    }
	
Now we can do registrations.

For this demo, I'll reference these two types for testing: IFoo interface and Foo class implementing IFoo:

    public interface IFoo
    {
        void DoSomething();
    }
	
	public class Foo : IFoo
    {
        public void DoSomething()
        {

        }
    }

We can register base types using factories as Lambda Expressions:

	using MEFedIoC;

	[TestMethod]
	public void TestRegisterFactory()
	{
		var container = GetContainer();

		container.RegisterFunc<IFoo>(() => new Foo());
		var foo = container.Resolve<IFoo>();
		Assert.IsInstanceOfType(foo, typeof(Foo));
	}
	
We can register base types with existing instances:

	[TestMethod]
	public void TestRegisterInstance()
	{
		var container = GetContainer();

		var foo = new Foo();
		container.RegisterInstance<IFoo>(foo);

		var foo2 = container.Resolve<IFoo>();

		Assert.AreSame(foo, foo2);
	}
	
And we can register by passing two generic parameters: The base class and the derived class:

	[TestMethod]
	public void TestRegisterGeneric()
	{
		var container = GetContainer();

		container.Register<IFoo, Foo>();

		var foo = container.Resolve<IFoo>();

		Assert.IsInstanceOfType(foo, typeof(Foo));
	}