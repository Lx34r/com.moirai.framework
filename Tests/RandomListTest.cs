using System.Collections.Generic;
using NUnit.Framework;
using Moirai.Atropos.Collections;

namespace Moirai.Atropos.Tests
{
    public class RandomListTest
    {
        [Test]
        public void Constructor_Default_EmptyList()
        {
            var list = new RandomList<int>();
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Constructor_WithCapacity_EmptyList()
        {
            var list = new RandomList<int>(10);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void Add_IncreasesCount()
        {
            var list = new RandomList<string>();
            list.Add("a");
            list.Add("b");

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Indexer_ReturnsCorrectItem()
        {
            var list = new RandomList<string>();
            list.Add("first");
            list.Add("second");

            Assert.AreEqual("first", list[0]);
            Assert.AreEqual("second", list[1]);
        }

        [Test]
        public void GetNext_ReturnsItemFromList()
        {
            var list = new RandomList<int>();
            list.Add(10);
            list.Add(20);
            list.Add(30);

            var seen = new HashSet<int>();
            for (int i = 0; i < 50; i++)
            {
                int val = list.GetNext();
                Assert.IsTrue(val == 10 || val == 20 || val == 30);
                seen.Add(val);
            }

            Assert.AreEqual(3, seen.Count, "All items should be selected at least once over 50 iterations");
        }

        [Test]
        public void GetNext_AvoidsDuplicateInSequence()
        {
            var list = new RandomList<int>();
            list.Add(1);
            list.Add(2);

            int last = list.GetNext();
            int sameCount = 0;
            for (int i = 0; i < 20; i++)
            {
                int current = list.GetNext();
                if (current == last) sameCount++;
                last = current;
            }

            Assert.Less(sameCount, 20, "Should not always repeat the same item");
        }

        [Test]
        public void Enumerator_IteratesAllItems()
        {
            var list = new RandomList<string>();
            list.Add("x");
            list.Add("y");
            list.Add("z");

            var items = new List<string>();
            foreach (var item in list)
            {
                items.Add(item);
            }

            Assert.AreEqual(3, items.Count);
            Assert.AreEqual("x", items[0]);
            Assert.AreEqual("y", items[1]);
            Assert.AreEqual("z", items[2]);
        }

        [Test]
        public void Add_WithWeight_IsAccepted()
        {
            var list = new RandomList<string>();
            list.Add("rare", 0.1);
            list.Add("common", 10.0);

            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void GetNext_SingleItem_ReturnsThatItem()
        {
            var list = new RandomList<int>();
            list.Add(42);

            int result = list.GetNext();
            Assert.AreEqual(42, result);
        }
    }
}
