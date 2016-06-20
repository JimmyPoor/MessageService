using MessageService.Core.Container.Unity;
using NUnit.Framework;
using System;

namespace MessageService.Test.Core
{
    [TestFixture]
    public class UnityContainerTest
    {
        UnityContainer _container;
        MessageService.Core.LifeCycleEnum _lifecycle;
        [SetUp]
        public void ContainerSetup()
        {
            _container = new UnityContainer();
            _lifecycle = MessageService.Core.LifeCycleEnum.Singleton;
        }

        [Test]
        public void Regist_Instance_and_resolve()
        {
            var b = new B() { Name = "kissnana3" };
            _container.RegistInstance<A>(b, _lifecycle);
            var copy = (B)_container.Resolve<A>();
            Assert.AreEqual(b, copy);
            Assert.AreSame(b, copy);
            Assert.AreEqual(b.Name, copy.Name);
        }

        [Test]
        public void Regist_type_resolve_and_contarst_between_resolve_objects()
        {
            _container.Regist<A, B>(_lifecycle);
            var resole_o = (B)_container.Resolve<A>();
            var resole_o2=(B)_container.Resolve<A>();
            Assert.AreNotEqual(resole_o, null);
            Assert.AreNotEqual(resole_o2, null);
            Assert.AreSame(resole_o, resole_o2);
        }


        [Test]
        public void Regist_type_and_resolve_with_consturctor_DI_Auto()
        {
            _container.Regist<A, B>(_lifecycle);
            _container.Regist<C, D>(_lifecycle);

            var c = (D)_container.Resolve<C>(); // will DI automaticlly
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.A);
        }

        [Test]
        public void Regist_type_and_resolve_with_consturctor_DI_Manual()
        {
            _container.Regist<C, D>(_lifecycle);
            _container.Regist<A, B>(_lifecycle);
            A a = new B();
            var c = (D)_container.Resolve<C>(
                new Tuple<string, object>(a.GetType().Name, a)
                );
            Assert.IsNotNull(c);
            Assert.IsNotNull(c.A);
        }

        [Test]
        //[Ignore("will change as throw exception later")]
        public void Throw_when_not_regist_first()
        {
            A a = null;
            D c = null;
            Assert.Catch(() => {
                 a = new B();
                // will throw error  since instance A is not been registed 
                 c = (D)_container.Resolve<C>(
                    new Tuple<string, object>(a.GetType().Name, a)
                    );
            });
            Assert.IsNull(c);
            Assert.Fail("instance type must been registed first");
        }
    }

    public interface A
    {

    }

    public class B : A
    {
        public string Name;
    }


    public interface C { }

    public class D : C
    {
        public A A;
        public D(A a)
        {
            this.A = a;
        }
    }


}
