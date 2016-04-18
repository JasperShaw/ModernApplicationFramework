using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {         
            Assert.IsFalse(ModernApplicationFramework.Core.Utilities.WindowsFileNameHelper.IsValidPath(@"asd"));
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsFalse(ModernApplicationFramework.Core.Utilities.WindowsFileNameHelper.IsValidPath(@"c:/bla"));
        }

        [TestMethod]
        public void TestMethod3()
        {
            Assert.IsFalse(ModernApplicationFramework.Core.Utilities.WindowsFileNameHelper.IsValidPath(@"c:\con\test"));
        }

        [TestMethod]
        public void TestMethod4()
        {
            Assert.IsTrue(ModernApplicationFramework.Core.Utilities.WindowsFileNameHelper.IsValidPath(@"c:\conj\test"));
        }
    }
}
