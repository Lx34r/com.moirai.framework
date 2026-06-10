using System;
using NUnit.Framework;

namespace Moirai.Atropos.Tests
{
    public class ConverterUtilityTest
    {
        [SetUp]
        public void SetUp()
        {
            ConverterUtility.ScreenDpi = 0;
        }

        [Test]
        public void GetBytes_Bool_True_ReturnsByte1()
        {
            byte[] bytes = ConverterUtility.GetBytes(true);

            Assert.AreEqual(1, bytes.Length);
            Assert.AreEqual(1, bytes[0]);
        }

        [Test]
        public void GetBytes_Bool_False_ReturnsByte0()
        {
            byte[] bytes = ConverterUtility.GetBytes(false);

            Assert.AreEqual(1, bytes.Length);
            Assert.AreEqual(0, bytes[0]);
        }

        [Test]
        public void GetBytes_Bool_IntoBuffer_WritesAtOffset()
        {
            byte[] buffer = new byte[4];
            ConverterUtility.GetBytes(true, buffer, 2);

            Assert.AreEqual(0, buffer[0]);
            Assert.AreEqual(0, buffer[1]);
            Assert.AreEqual(1, buffer[2]);
            Assert.AreEqual(0, buffer[3]);
        }

        [Test]
        public void GetBytes_Bool_NullBuffer_ThrowsGameException()
        {
            Assert.Throws<GameException>(() => ConverterUtility.GetBytes(true, null, 0));
        }

        [Test]
        public void GetBytes_Bool_InvalidStartIndex_ThrowsGameException()
        {
            byte[] buffer = new byte[1];
            Assert.Throws<GameException>(() => ConverterUtility.GetBytes(true, buffer, 2));
        }

        [Test]
        public void GetBoolean_FromByteArray_ReturnsCorrectValue()
        {
            byte[] trueBytes = { 1 };
            byte[] falseBytes = { 0 };

            Assert.IsTrue(ConverterUtility.GetBoolean(trueBytes));
            Assert.IsFalse(ConverterUtility.GetBoolean(falseBytes));
        }

        [Test]
        public void GetBoolean_WithStartIndex_ReturnsCorrectValue()
        {
            byte[] data = { 0, 0, 1 };
            Assert.IsTrue(ConverterUtility.GetBoolean(data, 2));
        }

        [Test]
        public void GetBytes_Short_RoundTrips()
        {
            short value = 12345;
            byte[] bytes = ConverterUtility.GetBytes(value);

            short result = ConverterUtility.GetInt16(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBytes_Short_NegativeValue_RoundTrips()
        {
            short value = -9876;
            byte[] bytes = ConverterUtility.GetBytes(value);

            short result = ConverterUtility.GetInt16(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBytes_Int_RoundTrips()
        {
            int value = 1234567;
            byte[] bytes = ConverterUtility.GetBytes(value);

            int result = ConverterUtility.GetInt32(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBytes_Long_RoundTrips()
        {
            long value = 123456789012345L;
            byte[] bytes = ConverterUtility.GetBytes(value);

            long result = ConverterUtility.GetInt64(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBytes_Float_RoundTrips()
        {
            float value = 3.14159f;
            byte[] bytes = ConverterUtility.GetBytes(value);

            float result = ConverterUtility.GetSingle(bytes);

            Assert.AreEqual(value, result, 0.0001f);
        }

        [Test]
        public void GetBytes_Double_RoundTrips()
        {
            double value = 2.718281828;
            byte[] bytes = ConverterUtility.GetBytes(value);

            double result = ConverterUtility.GetDouble(bytes);

            Assert.AreEqual(value, result, 0.0000001);
        }

        [Test]
        public void GetBytes_Char_RoundTrips()
        {
            char value = 'Z';
            byte[] bytes = ConverterUtility.GetBytes(value);
            char result = ConverterUtility.GetChar(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetCentimetersFromPixels_ScreenDpiNotSet_Throws()
        {
            Assert.Throws<GameException>(() => ConverterUtility.GetCentimetersFromPixels(100f));
        }

        [Test]
        public void GetPixelsFromCentimeters_ScreenDpiNotSet_Throws()
        {
            Assert.Throws<GameException>(() => ConverterUtility.GetPixelsFromCentimeters(2.54f));
        }

        [Test]
        public void GetInchesFromPixels_ScreenDpiNotSet_Throws()
        {
            Assert.Throws<GameException>(() => ConverterUtility.GetInchesFromPixels(96f));
        }

        [Test]
        public void GetPixelsFromInches_ScreenDpiNotSet_Throws()
        {
            Assert.Throws<GameException>(() => ConverterUtility.GetPixelsFromInches(1f));
        }

        [Test]
        public void PixelConversions_WithValidDpi()
        {
            ConverterUtility.ScreenDpi = 96f;

            float inches = ConverterUtility.GetInchesFromPixels(96f);
            Assert.AreEqual(1f, inches, 0.001f);

            float pixels = ConverterUtility.GetPixelsFromInches(1f);
            Assert.AreEqual(96f, pixels, 0.001f);
        }

        [Test]
        public void CentimeterConversions_WithValidDpi()
        {
            ConverterUtility.ScreenDpi = 96f;

            float cm = ConverterUtility.GetCentimetersFromPixels(96f);
            Assert.AreEqual(2.54f, cm, 0.01f);

            float pixels = ConverterUtility.GetPixelsFromCentimeters(2.54f);
            Assert.AreEqual(96f, pixels, 0.5f);
        }

        [Test]
        public void GetBytes_UShort_RoundTrips()
        {
            ushort value = 60000;
            byte[] bytes = ConverterUtility.GetBytes(value);

            ushort result = ConverterUtility.GetUInt16(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBytes_UInt_RoundTrips()
        {
            uint value = 3000000000U;
            byte[] bytes = ConverterUtility.GetBytes(value);

            uint result = ConverterUtility.GetUInt32(bytes);

            Assert.AreEqual(value, result);
        }

        [Test]
        public void GetBytes_ULong_RoundTrips()
        {
            ulong value = 18000000000000000000UL;
            byte[] bytes = ConverterUtility.GetBytes(value);

            ulong result = ConverterUtility.GetUInt64(bytes);

            Assert.AreEqual(value, result);
        }
    }
}
