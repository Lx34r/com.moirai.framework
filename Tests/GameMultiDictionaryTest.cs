using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class GameMultiDictionaryTest
    {
        [Test]
        public void NewDictionary_CountIsZero()
        {
            var dict = new GameMultiDictionary<string, int>();
            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void Add_SingleKeyValue_CountIsOne()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("fruits", 1);

            Assert.AreEqual(1, dict.Count);
        }

        [Test]
        public void Add_MultipleValuesToSameKey_CountStaysOne()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("fruits", 1);
            dict.Add("fruits", 2);
            dict.Add("fruits", 3);

            Assert.AreEqual(1, dict.Count);
        }

        [Test]
        public void Add_DifferentKeys_CountIncreases()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);

            Assert.AreEqual(2, dict.Count);
        }

        [Test]
        public void Contains_ExistingKey_ReturnsTrue()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 1);

            Assert.IsTrue(dict.Contains("key"));
        }

        [Test]
        public void Contains_MissingKey_ReturnsFalse()
        {
            var dict = new GameMultiDictionary<string, int>();
            Assert.IsFalse(dict.Contains("missing"));
        }

        [Test]
        public void Contains_KeyAndValue_ReturnsTrueForExisting()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 10);
            dict.Add("key", 20);

            Assert.IsTrue(dict.Contains("key", 10));
            Assert.IsTrue(dict.Contains("key", 20));
        }

        [Test]
        public void Contains_KeyAndValue_ReturnsFalseForMissingValue()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 10);

            Assert.IsFalse(dict.Contains("key", 99));
        }

        [Test]
        public void Contains_KeyAndValue_ReturnsFalseForMissingKey()
        {
            var dict = new GameMultiDictionary<string, int>();
            Assert.IsFalse(dict.Contains("missing", 1));
        }

        [Test]
        public void TryGetValue_ExistingKey_ReturnsTrueAndRange()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 1);
            dict.Add("key", 2);

            bool found = dict.TryGetValue("key", out var range);

            Assert.IsTrue(found);
            Assert.IsTrue(range.IsValid);
            Assert.AreEqual(2, range.Count);
        }

        [Test]
        public void TryGetValue_MissingKey_ReturnsFalse()
        {
            var dict = new GameMultiDictionary<string, int>();

            bool found = dict.TryGetValue("missing", out var range);

            Assert.IsFalse(found);
        }

        [Test]
        public void Indexer_ReturnsRangeForKey()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 100);
            dict.Add("key", 200);

            var range = dict["key"];
            Assert.IsTrue(range.IsValid);
            Assert.AreEqual(2, range.Count);
        }

        [Test]
        public void Remove_SingleValue_ReturnsTrue()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 1);
            dict.Add("key", 2);

            bool removed = dict.Remove("key", 1);

            Assert.IsTrue(removed);
            Assert.IsTrue(dict.Contains("key"));
            Assert.IsFalse(dict.Contains("key", 1));
            Assert.IsTrue(dict.Contains("key", 2));
        }

        [Test]
        public void Remove_LastValue_RemovesKey()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 1);

            bool removed = dict.Remove("key", 1);

            Assert.IsTrue(removed);
            Assert.IsFalse(dict.Contains("key"));
            Assert.AreEqual(0, dict.Count);
        }

        [Test]
        public void Remove_NonExistentValue_ReturnsFalse()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 1);

            Assert.IsFalse(dict.Remove("key", 99));
        }

        [Test]
        public void Remove_NonExistentKey_ReturnsFalse()
        {
            var dict = new GameMultiDictionary<string, int>();
            Assert.IsFalse(dict.Remove("missing", 1));
        }

        [Test]
        public void RemoveAll_ExistingKey_ReturnsTrue()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 1);
            dict.Add("key", 2);
            dict.Add("key", 3);

            bool removed = dict.RemoveAll("key");

            Assert.IsTrue(removed);
            Assert.AreEqual(0, dict.Count);
            Assert.IsFalse(dict.Contains("key"));
        }

        [Test]
        public void RemoveAll_NonExistentKey_ReturnsFalse()
        {
            var dict = new GameMultiDictionary<string, int>();
            Assert.IsFalse(dict.RemoveAll("missing"));
        }

        [Test]
        public void Clear_RemovesAllKeysAndValues()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("a", 3);

            dict.Clear();

            Assert.AreEqual(0, dict.Count);
            Assert.IsFalse(dict.Contains("a"));
            Assert.IsFalse(dict.Contains("b"));
        }

        [Test]
        public void Enumerator_IteratesAllKeys()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("a", 1);
            dict.Add("b", 2);
            dict.Add("c", 3);

            int count = 0;
            foreach (var kvp in dict)
            {
                count++;
                Assert.IsTrue(kvp.Value.IsValid);
            }

            Assert.AreEqual(3, count);
        }

        [Test]
        public void RangeEnumerator_IteratesValues()
        {
            var dict = new GameMultiDictionary<string, int>();
            dict.Add("key", 10);
            dict.Add("key", 20);
            dict.Add("key", 30);

            var range = dict["key"];
            int sum = 0;
            foreach (int val in range)
            {
                sum += val;
            }

            Assert.AreEqual(60, sum);
        }
    }
}
