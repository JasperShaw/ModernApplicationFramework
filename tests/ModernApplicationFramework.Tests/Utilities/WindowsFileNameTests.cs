using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModernApplicationFramework.Tests.Utilities
{
    [TestClass]
    public class WindowsFileNameTests
    {
        [TestMethod]
        public void TestMethod1()
        {         
            Assert.IsFalse(Core.Utilities.WindowsFileNameHelper.IsValidPath(@"asd"));
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsFalse(Core.Utilities.WindowsFileNameHelper.IsValidPath(@"c:/bla"));
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.IsFalse(Core.Utilities.WindowsFileNameHelper.IsValidPath(@"c:\con\test"));
        }

        [TestMethod]
        public void TestMethod4()
        {
            Assert.IsTrue(Core.Utilities.WindowsFileNameHelper.IsValidPath(@"c:\conj\test"));
        }
    }
}
