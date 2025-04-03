using Trino.Client.Utils;

namespace Trino.Client.Test
{
    [TestClass]
    public class TrinoFunctionTests
    {
        /// <summary>
        /// We subclass the TrinoFunction class to easily access the output from BuildFunctionStatement()
        /// </summary>
        class TrinoFunctionTestHelper : TrinoFunction
        {
            public TrinoFunctionTestHelper(string catalog, string functionName, IList<object> parameters) : base(catalog, functionName, parameters)
            {
            }

            public string GetStatement()
            {
                return base.BuildFunctionStatement();
            }
        }

        [TestMethod]
        public void TestNoParams()
        {
            var testTrinoFunction = new TrinoFunctionTestHelper("test", "testFunc", []);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("test.testFunc()", statement);
        }

        [TestMethod]
        public void TestOneStringParam()
        {
            var testTrinoFunction = new TrinoFunctionTestHelper("test", "testFunc", ["a"]);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("test.testFunc('a')", statement);
        }

        [TestMethod]
        public void TestTwoStringParams()
        {
            var testTrinoFunction = new TrinoFunctionTestHelper("test", "testFunc", ["a", "b"]);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("test.testFunc('a', 'b')", statement);
        }

        [TestMethod]
        public void TestNullCatalogue()
        {
            var testTrinoFunction = new TrinoFunctionTestHelper(null!, "testFunc", []);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("testFunc()", statement);
        }

        [TestMethod]
        public void TestEmptyCatalogue()
        {
            var testTrinoFunction = new TrinoFunctionTestHelper(string.Empty, "testFunc", []);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("testFunc()", statement);
        }

        [TestMethod]
        public void TestIntegralParams()
        {
            IList<object> args = [(sbyte)1, (byte)2, (short)3, (ushort)4, 5, 6U, 7L, 8UL, (nint)9, (nuint)10];
            var testTrinoFunction = new TrinoFunctionTestHelper(string.Empty, "testFunc", args);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("testFunc(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)", statement);
        }

        [TestMethod]
        public void TestFractionalParams()
        {
            IList<object> args = [1.5f, 2.5d, 3.5m];
            var testTrinoFunction = new TrinoFunctionTestHelper(string.Empty, "testFunc", args);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("testFunc(1.5, 2.5, 3.5)", statement);
        }

        [TestMethod]
        public void TestBooleanParams()
        {
            IList<object> args = [true, false];
            var testTrinoFunction = new TrinoFunctionTestHelper(string.Empty, "testFunc", args);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("testFunc(1, 0)", statement);
        }

        [TestMethod]
        public void TestDateParams()
        {
            IList<object> args = [
                new DateTime(2025, 4, 3),
                new DateTime(2025, 4, 3, 12, 0, 0),
                new DateTimeOffset(2025, 4, 3, 12, 0, 0, new TimeSpan(1, 0, 0))
            ];
            var testTrinoFunction = new TrinoFunctionTestHelper(string.Empty, "testFunc", args);
            var statement = testTrinoFunction.GetStatement();

            Assert.AreEqual("testFunc('2025-04-03T00:00:00', '2025-04-03T12:00:00', '2025-04-03T12:00:00')", statement);
        }
    }
}
