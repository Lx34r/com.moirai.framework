using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moirai.Atropos.Collections;

namespace Moirai.Atropos.Tests
{
    public class PriorityQueueTest
    {
        [Test]
        public void Enqueue_SingleItem_CountIsOne()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            Assert.AreEqual(1, pq.Count());
        }

        [Test]
        public void Enqueue_MultipleItems_MaintainsHeapProperty()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            pq.Enqueue(3);
            pq.Enqueue(8);
            pq.Enqueue(1);
            pq.Enqueue(10);

            Assert.IsTrue(pq.IsConsistent());
        }

        [Test]
        public void Dequeue_ReturnsSmallestItem()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            pq.Enqueue(3);
            pq.Enqueue(8);
            pq.Enqueue(1);

            Assert.AreEqual(1, pq.Dequeue());
            Assert.AreEqual(3, pq.Dequeue());
            Assert.AreEqual(5, pq.Dequeue());
            Assert.AreEqual(8, pq.Dequeue());
        }

        [Test]
        public void Dequeue_MaintainsHeapPropertyAfterEachRemoval()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(10);
            pq.Enqueue(4);
            pq.Enqueue(15);
            pq.Enqueue(1);
            pq.Enqueue(7);

            pq.Dequeue();
            Assert.IsTrue(pq.IsConsistent());
            pq.Dequeue();
            Assert.IsTrue(pq.IsConsistent());
        }

        [Test]
        public void Peek_ReturnsSmallestWithoutRemoving()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(9);
            pq.Enqueue(2);
            pq.Enqueue(6);

            Assert.AreEqual(2, pq.Peek());
            Assert.AreEqual(3, pq.Count());
        }

        [Test]
        public void Clear_EmptiesTheQueue()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(1);
            pq.Enqueue(2);
            pq.Clear();

            Assert.AreEqual(0, pq.Count());
        }

        [Test]
        public void Count_EmptyQueue_ReturnsZero()
        {
            var pq = new PriorityQueue<int>();
            Assert.AreEqual(0, pq.Count());
        }

        [Test]
        public void IsConsistent_EmptyQueue_ReturnsTrue()
        {
            var pq = new PriorityQueue<int>();
            Assert.IsTrue(pq.IsConsistent());
        }

        [Test]
        public void GetEnumerator_IteratesAllItems()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(3);
            pq.Enqueue(1);
            pq.Enqueue(2);

            var items = new List<int>();
            foreach (var item in pq)
            {
                items.Add(item);
            }

            Assert.AreEqual(3, items.Count);
        }

        [Test]
        public void ToString_ContainsCountInfo()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            pq.Enqueue(3);

            string s = pq.ToString();
            Assert.IsTrue(s.Contains("count = 2"));
        }

        [Test]
        public void Enqueue_DuplicateValues_AllStored()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(5);
            pq.Enqueue(5);
            pq.Enqueue(5);

            Assert.AreEqual(3, pq.Count());
            Assert.AreEqual(5, pq.Dequeue());
            Assert.AreEqual(5, pq.Dequeue());
            Assert.AreEqual(5, pq.Dequeue());
        }

        [Test]
        public void Enqueue_DescendingOrder_StillMinHeap()
        {
            var pq = new PriorityQueue<int>();
            pq.Enqueue(10);
            pq.Enqueue(9);
            pq.Enqueue(8);
            pq.Enqueue(7);
            pq.Enqueue(6);

            Assert.IsTrue(pq.IsConsistent());
            Assert.AreEqual(6, pq.Peek());
        }

        [Test]
        public void LargeDataSet_MaintainsHeapAndSortedOutput()
        {
            var pq = new PriorityQueue<int>();
            var rng = new System.Random(42);
            for (int i = 0; i < 1000; i++)
            {
                pq.Enqueue(rng.Next(0, 10000));
            }

            Assert.IsTrue(pq.IsConsistent());

            int prev = int.MinValue;
            while (pq.Count() > 0)
            {
                int current = pq.Dequeue();
                Assert.GreaterOrEqual(current, prev);
                prev = current;
            }
        }
    }
}
