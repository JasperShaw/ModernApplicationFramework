using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModernApplicationFramework.Utilities.Tests
{
    [TestClass]
    public class TypeArrayTest
    {
        [TestMethod]
        public void ObjectBaseAcceptsEverything()
        {
            var t = new TypeArray<object>(new[] {typeof(int), typeof(string), typeof(IBase), typeof(Sub)});
            Assert.AreEqual(4, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckBaseInterface()
        {
            var t = new TypeArray<IBase>(new[] { typeof(IBase)});
            Assert.AreEqual(1, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckBaseClass()
        {
            var t = new TypeArray<Base>(new[] { typeof(Base) });
            Assert.AreEqual(1, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckBaseClassAgainstItsInterface()
        {
            var t = new TypeArray<Base>(new[] { typeof(IBase) });
            Assert.AreEqual(0, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckBaseClassAgainstInheritatedClasses()
        {
            var t = new TypeArray<Base>(new[] { typeof(Sub), typeof(SubSub) });
            Assert.AreEqual(2, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckBaseInterfaceAgainstInheritatedClasses()
        {
            var t = new TypeArray<IBase>(new[] { typeof(Sub), typeof(SubSub) });
            Assert.AreEqual(2, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckBaseInterfaceAgainstInheritatedInterfaces()
        {
            var t = new TypeArray<IBase>(new[] { typeof(ISub) });
            Assert.AreEqual(1, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckSubInterfaceAgainstBaseClass()
        {
            var t = new TypeArray<ISub>(new[] { typeof(Base) });
            Assert.AreEqual(0, t.Memebers.Count);
        }

        [TestMethod]
        public void CheckSubInterfaceAgainstBaseInterfaces()
        {
            var t = new TypeArray<ISub>(new[] { typeof(IBase) });
            Assert.AreEqual(0, t.Memebers.Count);
        }

        [TestMethod]
        public void ObjectIsNotAccesptedForClasses()
        {
            var t = new TypeArray<Base>(new[] { typeof(object) });
            Assert.AreEqual(0, t.Memebers.Count);
        }

        [TestMethod]
        public void ObjectIsNotAccesptedForInterfaces()
        {
            var t = new TypeArray<IBase>(new[] { typeof(object) });
            Assert.AreEqual(0, t.Memebers.Count);
        }

        [TestMethod]
        public void ObjectIsAccesptedForClasses()
        {
            var t = new TypeArray<Base>(new[] { typeof(object) }, true);
            Assert.AreEqual(1, t.Memebers.Count);
        }

        [TestMethod]
        public void ObjectIsAccesptedForInterfaces()
        {
            var t = new TypeArray<IBase>(new[] { typeof(object) }, true);
            Assert.AreEqual(1, t.Memebers.Count);
        }

        [TestMethod]
        public void NullNotAllowed()
        {
            var t = new TypeArray<IBase>(new[] { (Type) null });
            Assert.AreEqual(0, t.Memebers.Count);
        }
    }
}
