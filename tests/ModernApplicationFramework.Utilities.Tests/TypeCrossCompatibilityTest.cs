using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModernApplicationFramework.Utilities.Tests
{
    [TestClass]
    public class TypeCrossCompatibilityTest
    {
        [TestMethod]
        public void DirectTypeEuqality()
        {
            var a = new Base();
            var b = new Base();
            Assert.IsTrue(a.GetType().CrossCheckTypeCompatibility(b.GetType()));
        }

        [TestMethod]
        public void DirectInterfaceEuqality()
        {
            Assert.IsTrue(typeof(IBase).CrossCheckTypeCompatibility(typeof(IBase)));
        }

        [TestMethod]
        public void BaseClassNotEuqalSubClass()
        {
            Assert.IsTrue(typeof(Base).CrossCheckTypeCompatibility(typeof(Sub)));
        }

        [TestMethod]
        public void BaseInterfaceNotEuqalSubInterface()
        {
            Assert.IsTrue(typeof(IBase).CrossCheckTypeCompatibility(typeof(ISub)));
        }

        [TestMethod]
        public void SubClassEuqalBaseClass()
        {
            Assert.IsTrue(typeof(Sub).CrossCheckTypeCompatibility(typeof(Base)));
        }

        [TestMethod]
        public void SubInterfaceEuqalBaseInterface()
        {
            Assert.IsTrue(typeof(ISub).CrossCheckTypeCompatibility(typeof(IBase)));
        }

        [TestMethod]
        public void ClassImplementsOwnInterface()
        {
            Assert.IsTrue(typeof(Sub).CrossCheckTypeCompatibility(typeof(ISub)));
        }

        [TestMethod]
        public void ClassImplementsInheritedInterface()
        {
            Assert.IsTrue(typeof(Sub).CrossCheckTypeCompatibility(typeof(IBase)));
        }

        [TestMethod]
        public void ClassImplementsInheritedInterfaceLevel2()
        {
            Assert.IsTrue(typeof(SubSub).CrossCheckTypeCompatibility(typeof(IBase)));
        }

        [TestMethod]
        public void ClassIsInheritedFromObject()
        {
            Assert.IsTrue(typeof(Base).CrossCheckTypeCompatibility(typeof(object)));
        }

        [TestMethod]
        public void InterfaceIsInheritedFromObject()
        {
            Assert.IsTrue(typeof(IBase).CrossCheckTypeCompatibility(typeof(object)));
        }

        [TestMethod]
        public void InterfaceIsImplemented()
        {
            Assert.IsTrue(typeof(IBase).CrossCheckTypeCompatibility(typeof(Base)));
        }

        [TestMethod]
        public void InterfaceIsImplemented2()
        {
            Assert.IsTrue(typeof(IBase).CrossCheckTypeCompatibility(typeof(Sub)));
        }

        [TestMethod]
        public void InterfaceIsNotImplemented()
        {
            Assert.IsFalse(typeof(ISub).CrossCheckTypeCompatibility(typeof(Base)));
        }

        [TestMethod]
        public void ClassNotImplementsInterface()
        {
            Assert.IsFalse(typeof(Base).CrossCheckTypeCompatibility(typeof(ISub)));
        }

        [TestMethod]
        public void InterfaceNotImplementsInterface()
        {
            Assert.IsFalse(typeof(IBase).CrossCheckTypeCompatibility(typeof(IOther)));
        }

        [TestMethod]
        public void InterfaceIsImplemented3()
        {
            Assert.IsTrue(typeof(IBase).CrossCheckTypeCompatibility(typeof(SubSub)));
        }

        [TestMethod]
        public void NoEuqality()
        {
            Assert.IsFalse(typeof(Base).CrossCheckTypeCompatibility(typeof(Other)));
        }

        [TestMethod]
        public void NullAgaingstNull()
        {
            Assert.IsTrue(((Type)null).CrossCheckTypeCompatibility(null));
        }

        [TestMethod]
        public void NullAgaingstAny()
        {
            Assert.IsFalse(((Type)null).CrossCheckTypeCompatibility(typeof(Base)));
        }

        [TestMethod]
        public void AnyAgaingstNull()
        {
            Assert.IsFalse(typeof(Base).CrossCheckTypeCompatibility(null));
        }
    }
}
