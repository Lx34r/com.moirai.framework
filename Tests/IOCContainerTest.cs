using NUnit.Framework;
using Moirai.Atropos.Collections;

namespace Moirai.Atropos.Tests
{
    public class IOCContainerTest
    {
        private interface IService { string Name { get; } }

        private class ServiceA : IService { public string Name => "A"; }

        private class ServiceB : IService { public string Name => "B"; }

        private interface IOther { }

        [Test]
        public void Register_And_Resolve_ReturnsSameInstance()
        {
            var container = new IOCContainer();
            var service = new ServiceA();

            container.Register<IService>(service);
            var resolved = container.Resolve<IService>();

            Assert.AreSame(service, resolved);
        }

        [Test]
        public void Resolve_Unregistered_ReturnsNull()
        {
            var container = new IOCContainer();
            var resolved = container.Resolve<IService>();

            Assert.IsNull(resolved);
        }

        [Test]
        public void Register_Override_ReturnsLatest()
        {
            var container = new IOCContainer();
            var first = new ServiceA();
            var second = new ServiceB();

            container.Register<IService>(first);
            container.Register<IService>(second);

            var resolved = container.Resolve<IService>();
            Assert.AreEqual("B", resolved.Name);
        }

        [Test]
        public void Unregister_MatchingInstance_Removes()
        {
            var container = new IOCContainer();
            var service = new ServiceA();

            container.Register<IService>(service);
            container.Unregister<IService>(service);

            Assert.IsNull(container.Resolve<IService>());
        }

        [Test]
        public void Unregister_DifferentInstance_DoesNotRemove()
        {
            var container = new IOCContainer();
            var original = new ServiceA();
            var other = new ServiceA();

            container.Register<IService>(original);
            container.Unregister<IService>(other);

            Assert.AreSame(original, container.Resolve<IService>());
        }

        [Test]
        public void Unregister_NotRegistered_NoError()
        {
            var container = new IOCContainer();
            var service = new ServiceA();

            Assert.DoesNotThrow(() => container.Unregister<IService>(service));
        }

        [Test]
        public void Clear_RemovesAllRegistrations()
        {
            var container = new IOCContainer();
            container.Register<IService>(new ServiceA());
            container.Register<IOther>(null);

            container.Clear();

            Assert.IsNull(container.Resolve<IService>());
            Assert.IsNull(container.Resolve<IOther>());
        }

        [Test]
        public void Register_MultipleTypes_IndependentlyResolved()
        {
            var container = new IOCContainer();
            var serviceA = new ServiceA();

            container.Register<IService>(serviceA);
            container.Register<ServiceA>(serviceA);

            Assert.AreSame(serviceA, container.Resolve<IService>());
            Assert.AreSame(serviceA, container.Resolve<ServiceA>());
        }
    }
}
