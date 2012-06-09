using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MEFedIoC.Tests
{
    [TestClass]
    public class MefContainerTests
    {
        public IMefContainer GetContainer()
        {
            var multiCatalog = new AggregateCatalog();
            var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            multiCatalog.Catalogs.Add(catalog);
            var container = new MefContainer(multiCatalog);
            return container;
        }

        [TestMethod]
        public void TestRegisterFactory()
        {
            var container = GetContainer();

            container.RegisterFunc<IFoo>(() => new Foo());
            var foo = container.GetService<IFoo>();
            Assert.IsInstanceOfType(foo, typeof(Foo));
        }

        [TestMethod]
        public void TestRegisterInstance()
        {
            var container = GetContainer();

            var foo = new Foo();
            container.RegisterInstance<IFoo>(foo);

            var foo2 = container.Resolve<IFoo>();

            Assert.AreSame(foo, foo2);
        }

        [TestMethod]
        public void TestRegisterGeneric()
        {
            var container = GetContainer();

            container.Register<IFoo, Foo>();

            var foo = container.Resolve<IFoo>();

            Assert.IsInstanceOfType(foo, typeof(Foo));
        }

    }
}
