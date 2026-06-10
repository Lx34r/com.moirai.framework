using System;
using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class MemoryPoolTest
    {
        private class TestMemory : IMemory
        {
            public int Value { get; set; }
            public bool WasCleared { get; private set; }

            public void Clear()
            {
                Value = 0;
                WasCleared = true;
            }
        }

        private class OtherMemory : IMemory
        {
            public void Clear() { }
        }

        [SetUp]
        public void SetUp()
        {
            MemoryPool.ClearAll();
            MemoryPool.EnableStrictCheck = false;
        }

        [TearDown]
        public void TearDown()
        {
            MemoryPool.ClearAll();
            MemoryPool.EnableStrictCheck = false;
        }

        [Test]
        public void Acquire_ReturnsNewInstance()
        {
            var obj = MemoryPool.Acquire<TestMemory>();

            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<TestMemory>(obj);
        }

        [Test]
        public void Release_CallsClearOnObject()
        {
            var obj = MemoryPool.Acquire<TestMemory>();
            obj.Value = 42;

            MemoryPool.Release(obj);

            Assert.IsTrue(obj.WasCleared);
            Assert.AreEqual(0, obj.Value);
        }

        [Test]
        public void Acquire_AfterRelease_ReusesObject()
        {
            var first = MemoryPool.Acquire<TestMemory>();
            MemoryPool.Release(first);

            var second = MemoryPool.Acquire<TestMemory>();

            Assert.AreSame(first, second);
        }

        [Test]
        public void Release_Null_ThrowsException()
        {
            Assert.Throws<Exception>(() => MemoryPool.Release(null));
        }

        [Test]
        public void Count_TracksRegisteredTypes()
        {
            MemoryPool.Acquire<TestMemory>();
            int countAfterFirst = MemoryPool.Count;

            MemoryPool.Acquire<OtherMemory>();
            int countAfterSecond = MemoryPool.Count;

            Assert.AreEqual(1, countAfterFirst);
            Assert.AreEqual(2, countAfterSecond);
        }

        [Test]
        public void ClearAll_RemovesAllCollections()
        {
            MemoryPool.Acquire<TestMemory>();
            MemoryPool.Acquire<OtherMemory>();

            MemoryPool.ClearAll();

            Assert.AreEqual(0, MemoryPool.Count);
        }

        [Test]
        public void Add_PreAllocatesObjects()
        {
            MemoryPool.Add<TestMemory>(3);

            var infos = MemoryPool.GetAllMemoryPoolInfos();
            Assert.AreEqual(1, infos.Length);
            Assert.AreEqual(3, infos[0].UnusedMemoryCount);
            Assert.AreEqual(3, infos[0].AddMemoryCount);
        }

        [Test]
        public void Remove_RemovesPreAllocatedObjects()
        {
            MemoryPool.Add<TestMemory>(5);
            MemoryPool.Remove<TestMemory>(3);

            var infos = MemoryPool.GetAllMemoryPoolInfos();
            Assert.AreEqual(2, infos[0].UnusedMemoryCount);
            Assert.AreEqual(3, infos[0].RemoveMemoryCount);
        }

        [Test]
        public void Remove_MoreThanAvailable_ClampsToAvailable()
        {
            MemoryPool.Add<TestMemory>(2);
            MemoryPool.Remove<TestMemory>(10);

            var infos = MemoryPool.GetAllMemoryPoolInfos();
            Assert.AreEqual(0, infos[0].UnusedMemoryCount);
            Assert.AreEqual(2, infos[0].RemoveMemoryCount);
        }

        [Test]
        public void RemoveAll_ClearsAllFromType()
        {
            MemoryPool.Add<TestMemory>(5);
            MemoryPool.RemoveAll<TestMemory>();

            var infos = MemoryPool.GetAllMemoryPoolInfos();
            Assert.AreEqual(0, infos[0].UnusedMemoryCount);
        }

        [Test]
        public void GetAllMemoryPoolInfos_ReturnsCorrectStats()
        {
            var a = MemoryPool.Acquire<TestMemory>();
            var b = MemoryPool.Acquire<TestMemory>();
            MemoryPool.Release(a);

            var infos = MemoryPool.GetAllMemoryPoolInfos();

            Assert.AreEqual(1, infos.Length);
            Assert.AreEqual(typeof(TestMemory), infos[0].Type);
            Assert.AreEqual(2, infos[0].AcquireMemoryCount);
            Assert.AreEqual(1, infos[0].ReleaseMemoryCount);
            Assert.AreEqual(1, infos[0].UsingMemoryCount);
            Assert.AreEqual(1, infos[0].UnusedMemoryCount);
        }

        [Test]
        public void Acquire_ByType_ReturnsCorrectInstance()
        {
            IMemory obj = MemoryPool.Acquire(typeof(TestMemory));

            Assert.IsNotNull(obj);
            Assert.IsInstanceOf<TestMemory>(obj);
        }

        [Test]
        public void EnableStrictCheck_DuplicateRelease_ThrowsException()
        {
            MemoryPool.EnableStrictCheck = true;

            var obj = MemoryPool.Acquire<TestMemory>();
            MemoryPool.Release(obj);

            Assert.Throws<Exception>(() => MemoryPool.Release(obj));
        }

        [Test]
        public void EnableStrictCheck_InvalidType_ThrowsException()
        {
            MemoryPool.EnableStrictCheck = true;

            Assert.Throws<Exception>(() => MemoryPool.Acquire(typeof(string)));
        }

        [Test]
        public void EnableStrictCheck_AbstractType_ThrowsException()
        {
            MemoryPool.EnableStrictCheck = true;

            Assert.Throws<Exception>(() => MemoryPool.Acquire(typeof(IMemory)));
        }

        [Test]
        public void EnableStrictCheck_NullType_ThrowsException()
        {
            MemoryPool.EnableStrictCheck = true;

            Assert.Throws<Exception>(() => MemoryPool.Acquire(null));
        }
    }
}
