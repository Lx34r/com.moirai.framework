using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class TextUtilityTest
    {
        [SetUp]
        public void SetUp()
        {
            TextUtility.SetFormatHelper(null);
        }

        [Test]
        public void Format_OneArg_FormatsCorrectly()
        {
            string result = TextUtility.Format("Hello {0}", "World");
            Assert.AreEqual("Hello World", result);
        }

        [Test]
        public void Format_TwoArgs_FormatsCorrectly()
        {
            string result = TextUtility.Format("{0} + {1}", 1, 2);
            Assert.AreEqual("1 + 2", result);
        }

        [Test]
        public void Format_ThreeArgs_FormatsCorrectly()
        {
            string result = TextUtility.Format("{0}-{1}-{2}", "a", "b", "c");
            Assert.AreEqual("a-b-c", result);
        }

        [Test]
        public void Format_FourArgs_FormatsCorrectly()
        {
            string result = TextUtility.Format("{0}{1}{2}{3}", 1, 2, 3, 4);
            Assert.AreEqual("1234", result);
        }

        [Test]
        public void Format_FiveArgs_FormatsCorrectly()
        {
            string result = TextUtility.Format("{0}{1}{2}{3}{4}", "a", "b", "c", "d", "e");
            Assert.AreEqual("abcde", result);
        }

        [Test]
        public void Format_NullFormat_ThrowsGameException()
        {
            Assert.Throws<GameException>(() => TextUtility.Format<string>(null, "arg"));
        }

        [Test]
        public void Format_NullFormat_TwoArgs_ThrowsGameException()
        {
            Assert.Throws<GameException>(() => TextUtility.Format<string, string>(null, "a", "b"));
        }

        [Test]
        public void Format_NullFormat_ThreeArgs_ThrowsGameException()
        {
            Assert.Throws<GameException>(() => TextUtility.Format<int, int, int>(null, 1, 2, 3));
        }

        [Test]
        public void Format_IntArgs_FormatsCorrectly()
        {
            string result = TextUtility.Format("Value: {0}", 42);
            Assert.AreEqual("Value: 42", result);
        }

        [Test]
        public void Format_MixedTypes_FormatsCorrectly()
        {
            string result = TextUtility.Format("{0} is {1}", "age", 25);
            Assert.AreEqual("age is 25", result);
        }
    }
}
