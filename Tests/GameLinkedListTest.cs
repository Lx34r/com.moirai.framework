using System.Collections.Generic;
using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class GameLinkedListTest
    {
        [Test]
        public void NewList_CountIsZero()
        {
            var list = new GameLinkedList<int>();
            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void AddFirst_IncreasesCount()
        {
            var list = new GameLinkedList<int>();
            list.AddFirst(10);
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(10, list.First.Value);
        }

        [Test]
        public void AddLast_AppendsToEnd()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            Assert.AreEqual(1, list.First.Value);
            Assert.AreEqual(3, list.Last.Value);
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void AddAfter_InsertsAfterNode()
        {
            var list = new GameLinkedList<int>();
            var first = list.AddFirst(1);
            list.AddLast(3);
            list.AddAfter(first, 2);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, list.First.Next.Value);
        }

        [Test]
        public void AddBefore_InsertsBeforeNode()
        {
            var list = new GameLinkedList<int>();
            list.AddFirst(1);
            var last = list.AddLast(3);
            list.AddBefore(last, 2);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(2, list.Last.Previous.Value);
        }

        [Test]
        public void Remove_ByValue_ReturnsTrue()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            bool removed = list.Remove(2);

            Assert.IsTrue(removed);
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void Remove_NonExistentValue_ReturnsFalse()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);

            Assert.IsFalse(list.Remove(99));
        }

        [Test]
        public void Remove_ByNode_RemovesCorrectNode()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            var node = list.AddLast(2);
            list.AddLast(3);

            list.Remove(node);

            Assert.AreEqual(2, list.Count);
            Assert.AreEqual(3, list.First.Next.Value);
        }

        [Test]
        public void RemoveFirst_RemovesHeadNode()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);

            list.RemoveFirst();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(2, list.First.Value);
        }

        [Test]
        public void RemoveLast_RemovesTailNode()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);

            list.RemoveLast();

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(1, list.Last.Value);
        }

        [Test]
        public void Contains_ExistingValue_ReturnsTrue()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(10);
            list.AddLast(20);

            Assert.IsTrue(list.Contains(10));
            Assert.IsTrue(list.Contains(20));
        }

        [Test]
        public void Contains_MissingValue_ReturnsFalse()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(10);

            Assert.IsFalse(list.Contains(99));
        }

        [Test]
        public void Find_ReturnsCorrectNode()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(10);
            list.AddLast(20);
            list.AddLast(30);

            var node = list.Find(20);
            Assert.IsNotNull(node);
            Assert.AreEqual(20, node.Value);
        }

        [Test]
        public void FindLast_ReturnsLastOccurrence()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(1);

            var node = list.FindLast(1);
            Assert.IsNotNull(node);
            Assert.AreEqual(list.Last, node);
        }

        [Test]
        public void Clear_RemovesAllNodes()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            list.Clear();

            Assert.AreEqual(0, list.Count);
            Assert.IsNull(list.First);
            Assert.IsNull(list.Last);
        }

        [Test]
        public void CachedNodeCount_IncreasesAfterRemoval()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);

            Assert.AreEqual(0, list.CachedNodeCount);

            list.Remove(1);

            Assert.AreEqual(1, list.CachedNodeCount);
        }

        [Test]
        public void ClearCachedNodes_ResetsCachedCount()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.Remove(1);

            Assert.AreEqual(1, list.CachedNodeCount);

            list.ClearCachedNodes();

            Assert.AreEqual(0, list.CachedNodeCount);
        }

        [Test]
        public void NodeReuse_CachedNodesAreReused()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.Remove(1);

            Assert.AreEqual(1, list.CachedNodeCount);

            list.AddLast(2);

            Assert.AreEqual(0, list.CachedNodeCount);
            Assert.AreEqual(2, list.First.Value);
        }

        [Test]
        public void CopyTo_CopiesAllElements()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(10);
            list.AddLast(20);
            list.AddLast(30);

            var arr = new int[3];
            list.CopyTo(arr, 0);

            Assert.AreEqual(10, arr[0]);
            Assert.AreEqual(20, arr[1]);
            Assert.AreEqual(30, arr[2]);
        }

        [Test]
        public void Enumerator_IteratesInOrder()
        {
            var list = new GameLinkedList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);

            var values = new List<int>();
            foreach (var v in list)
            {
                values.Add(v);
            }

            Assert.AreEqual(3, values.Count);
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void IsReadOnly_ReturnsFalse()
        {
            var list = new GameLinkedList<int>();
            Assert.IsFalse(list.IsReadOnly);
        }
    }
}
