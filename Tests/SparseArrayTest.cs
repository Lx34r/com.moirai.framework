using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moirai.Atropos.Collections;

namespace Moirai.Atropos.Tests
{
    public class SparseArrayTest
    {
        [Test]
        public void Constructor_InitializesWithCorrectCapacity()
        {
            var arr = new SparseArray<int>(4, 10);

            Assert.AreEqual(10, arr.Capacity);
            Assert.AreEqual(0, arr.Count);
            Assert.AreEqual(4, arr.NumFreeIndices);
        }

        [Test]
        public void Add_ReturnsIndex_IncrementsCount()
        {
            var arr = new SparseArray<int>(4, 10);
            int idx = arr.Add(42);

            Assert.AreEqual(0, idx);
            Assert.AreEqual(1, arr.Count);
            Assert.AreEqual(42, arr[idx]);
        }

        [Test]
        public void Add_MultipleElements_UseFreeSlots()
        {
            var arr = new SparseArray<int>(4, 10);
            int a = arr.Add(10);
            int b = arr.Add(20);
            int c = arr.Add(30);

            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual(10, arr[a]);
            Assert.AreEqual(20, arr[b]);
            Assert.AreEqual(30, arr[c]);
        }

        [Test]
        public void Add_BeyondInitialLength_ExpandsInternally()
        {
            var arr = new SparseArray<int>(2, 10);
            arr.Add(1);
            arr.Add(2);
            int idx = arr.Add(3);

            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual(3, arr[idx]);
        }

        [Test]
        public void Add_ExceedsCapacity_ThrowsException()
        {
            var arr = new SparseArray<int>(2, 2);
            arr.Add(1);
            arr.Add(2);

            Assert.Throws<ArgumentOutOfRangeException>(() => arr.Add(3));
        }

        [Test]
        public void RemoveAt_DecrementsCount()
        {
            var arr = new SparseArray<int>(4, 10);
            int a = arr.Add(10);
            arr.Add(20);

            arr.RemoveAt(a);

            Assert.AreEqual(1, arr.Count);
            Assert.AreEqual(default(int), arr[a]);
        }

        [Test]
        public void RemoveAt_FreeSlotIsReused()
        {
            var arr = new SparseArray<int>(4, 10);
            int a = arr.Add(10);
            arr.Add(20);
            arr.RemoveAt(a);

            int reused = arr.Add(30);

            Assert.AreEqual(a, reused);
            Assert.AreEqual(30, arr[reused]);
        }

        [Test]
        public void IsAllocated_ReturnsTrueForAllocated()
        {
            var arr = new SparseArray<int>(4, 10);
            int idx = arr.Add(1);

            Assert.IsTrue(arr.IsAllocated(idx));
        }

        [Test]
        public void IsAllocated_ReturnsFalseForUnallocated()
        {
            var arr = new SparseArray<int>(4, 10);
            Assert.IsFalse(arr.IsAllocated(0));
        }

        [Test]
        public void IsAllocated_ReturnsFalseForRemovedIndex()
        {
            var arr = new SparseArray<int>(4, 10);
            int idx = arr.Add(5);
            arr.RemoveAt(idx);

            Assert.IsFalse(arr.IsAllocated(idx));
        }

        [Test]
        public void IsAllocated_NegativeIndex_ReturnsFalse()
        {
            var arr = new SparseArray<int>(4, 10);
            Assert.IsFalse(arr.IsAllocated(-1));
        }

        [Test]
        public void IsAllocated_OutOfBoundsIndex_ReturnsFalse()
        {
            var arr = new SparseArray<int>(4, 10);
            Assert.IsFalse(arr.IsAllocated(100));
        }

        [Test]
        public void Indexer_Set_UpdatesValue()
        {
            var arr = new SparseArray<int>(4, 10);
            int idx = arr.Add(10);
            arr[idx] = 99;

            Assert.AreEqual(99, arr[idx]);
        }

        [Test]
        public void Indexer_Set_UnallocatedIndex_NoOp()
        {
            var arr = new SparseArray<int>(4, 10);
            arr[0] = 42;

            Assert.AreEqual(default(int), arr[0]);
        }

        [Test]
        public void Clear_ResetsCountAndFreeIndices()
        {
            var arr = new SparseArray<int>(4, 10);
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);

            arr.Clear();

            Assert.AreEqual(0, arr.Count);
        }

        [Test]
        public void AddUninitialized_AddsDefaultValue()
        {
            var arr = new SparseArray<string>(4, 10);
            int idx = arr.AddUninitialized();

            Assert.IsTrue(arr.IsAllocated(idx));
            Assert.AreEqual(default(string), arr[idx]);
        }

        [Test]
        public void Enumerator_IteratesOnlyAllocatedElements()
        {
            var arr = new SparseArray<int>(4, 10);
            arr.Add(10);
            int b = arr.Add(20);
            arr.Add(30);
            arr.RemoveAt(b);

            var items = new List<int>();
            foreach (var item in arr)
            {
                items.Add(item);
            }

            Assert.AreEqual(2, items.Count);
            Assert.Contains(10, items);
            Assert.Contains(30, items);
        }

        [Test]
        public void Shrink_RemovesTrailingUnallocatedSlots()
        {
            var arr = new SparseArray<int>(0, 10);
            arr.Add(1);
            arr.Add(2);
            int c = arr.Add(3);
            arr.RemoveAt(c);

            arr.Shrink();

            Assert.AreEqual(2, arr.Count);
        }

        [Test]
        public void Shrink_NoTrailingFree_NoChange()
        {
            var arr = new SparseArray<int>(0, 10);
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);

            int countBefore = arr.Count;
            arr.Shrink();

            Assert.AreEqual(countBefore, arr.Count);
        }
    }
}
