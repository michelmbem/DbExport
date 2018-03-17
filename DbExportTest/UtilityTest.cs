using System;
using DbExport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbExportTest
{
    /// <summary>
    ///This is a test class for Utility and is intended
    ///to contain all Utility Unit Tests
    ///</summary>
    [TestClass]
    public class UtilityTest
    {
        private TestContext testContext;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContext; }
            set { testContext = value; }
        }

        #region Additional test attributes

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion

        /// <summary>
        ///A test for GetBytes
        ///</summary>
        [TestMethod]
        public void GetBytesTest()
        {
            var value = @"\\\\banane \\340 gogo\n";
            byte[] expected = {92, 98, 97, 110, 97, 110, 101, 32, 224, 32, 103, 111, 103, 111, 10};
            var actual = Utility.GetBytes(value);

            Assert.AreEqual(BitConverter.ToString(expected), BitConverter.ToString(actual));
        }

        /// <summary>
        ///A test for FromBaseN
        ///</summary>
        [TestMethod]
        public void FromBaseNTest()
        {
            var value = "1010";
            byte n = 2;
            byte expected = 10;
            var actual = Utility.FromBaseN(value, n);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToBaseN
        ///</summary>
        [TestMethod]
        public void ToBaseNTest()
        {
            byte b = 192;
            byte n = 2;
            var expected = "11000000";
            var actual = Utility.ToBaseN(b, n);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for FromBitString
        ///</summary>
        [TestMethod]
        public void FromBitStringTest()
        {
            var value = "11111001011";
            byte[] expected = {7, 203};
            var actual = Utility.FromBitString(value);
            Assert.AreEqual(BitConverter.ToString(expected), BitConverter.ToString(actual));
        }
    }
}