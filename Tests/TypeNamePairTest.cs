using System;
using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class TypeNamePairTest
    {
        [Test]
        public void Constructor_TypeOnly_NameIsEmpty()
        {
            var pair = new TypeNamePair(typeof(int));

            Assert.AreEqual(typeof(int), pair.Type);
            Assert.AreEqual(string.Empty, pair.Name);
        }

        [Test]
        public void Constructor_TypeAndName_BothSet()
        {
            var pair = new TypeNamePair(typeof(string), "myName");

            Assert.AreEqual(typeof(string), pair.Type);
            Assert.AreEqual("myName", pair.Name);
        }

        [Test]
        public void Constructor_NullName_TreatedAsEmpty()
        {
            var pair = new TypeNamePair(typeof(int), null);
            Assert.AreEqual(string.Empty, pair.Name);
        }

        [Test]
        public void Constructor_NullType_ThrowsGameException()
        {
            Assert.Throws<GameException>(() => new TypeNamePair(null));
        }

        [Test]
        public void Equals_SameTypeAndName_ReturnsTrue()
        {
            var a = new TypeNamePair(typeof(int), "test");
            var b = new TypeNamePair(typeof(int), "test");

            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            var a = new TypeNamePair(typeof(int), "test");
            var b = new TypeNamePair(typeof(string), "test");

            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_DifferentName_ReturnsFalse()
        {
            var a = new TypeNamePair(typeof(int), "aaa");
            var b = new TypeNamePair(typeof(int), "bbb");

            Assert.IsFalse(a.Equals(b));
        }

        [Test]
        public void Equals_ObjectOverload_WorksCorrectly()
        {
            var a = new TypeNamePair(typeof(int), "test");
            object b = new TypeNamePair(typeof(int), "test");

            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void Equals_NonTypeNamePairObject_ReturnsFalse()
        {
            var a = new TypeNamePair(typeof(int));
            Assert.IsFalse(a.Equals("not a TypeNamePair"));
        }

        [Test]
        public void OperatorEquals_MatchingPairs_ReturnsTrue()
        {
            var a = new TypeNamePair(typeof(int), "x");
            var b = new TypeNamePair(typeof(int), "x");

            Assert.IsTrue(a == b);
        }

        [Test]
        public void OperatorNotEquals_DifferentPairs_ReturnsTrue()
        {
            var a = new TypeNamePair(typeof(int), "x");
            var b = new TypeNamePair(typeof(int), "y");

            Assert.IsTrue(a != b);
        }

        [Test]
        public void GetHashCode_SamePairs_SameHash()
        {
            var a = new TypeNamePair(typeof(int), "test");
            var b = new TypeNamePair(typeof(int), "test");

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToString_TypeOnly_ReturnsFullTypeName()
        {
            var pair = new TypeNamePair(typeof(int));

            string result = pair.ToString();
            Assert.AreEqual(typeof(int).FullName, result);
        }

        [Test]
        public void ToString_TypeAndName_ReturnsTypeNameDotName()
        {
            var pair = new TypeNamePair(typeof(int), "myName");

            string result = pair.ToString();
            Assert.AreEqual($"{typeof(int).FullName}.myName", result);
        }
    }
}
