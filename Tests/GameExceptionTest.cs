using System;
using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class GameExceptionTest
    {
        [Test]
        public void DefaultConstructor_NoMessage()
        {
            var ex = new GameException();
            Assert.IsNotNull(ex);
            Assert.IsInstanceOf<Exception>(ex);
        }

        [Test]
        public void MessageConstructor_SetsMessage()
        {
            var ex = new GameException("test error");
            Assert.AreEqual("test error", ex.Message);
        }

        [Test]
        public void InnerExceptionConstructor_SetsInnerException()
        {
            var inner = new InvalidOperationException("inner");
            var ex = new GameException("outer", inner);

            Assert.AreEqual("outer", ex.Message);
            Assert.AreSame(inner, ex.InnerException);
        }

        [Test]
        public void CanBeCaughtAsException()
        {
            bool caught = false;
            try
            {
                throw new GameException("test");
            }
            catch (Exception)
            {
                caught = true;
            }

            Assert.IsTrue(caught);
        }

        [Test]
        public void CanBeCaughtAsGameException()
        {
            string message = null;
            try
            {
                throw new GameException("specific");
            }
            catch (GameException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual("specific", message);
        }
    }
}
