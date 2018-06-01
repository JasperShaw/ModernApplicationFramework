using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ModernApplicationFramework.Utilities.Tests
{
    [TestClass]
    public class TypeCompatibilityTest
    {
        [TestMethod]
        public void DirectTypeEuqality()
        {
            Assert.IsTrue(typeof(Base).ImplementsOrInharits(typeof(Base)));
        }

        [TestMethod]
        public void DirectInterfaceEuqality()
        {
            Assert.IsTrue(typeof(IBase).ImplementsOrInharits(typeof(IBase)));
        }

        [TestMethod]
        public void BaseClassNotEuqalSubClass()
        {
            Assert.IsFalse(typeof(Base).ImplementsOrInharits(typeof(Sub)));
        }

        [TestMethod]
        public void BaseInterfaceNotEuqalSubInterface()
        {
            Assert.IsFalse(typeof(IBase).ImplementsOrInharits(typeof(ISub)));
        }

        [TestMethod]
        public void SubClassEuqalBaseClass()
        {
            Assert.IsTrue(typeof(Sub).ImplementsOrInharits(typeof(Base)));
        }

        [TestMethod]
        public void SubInterfaceEuqalBaseInterface()
        {
            Assert.IsTrue(typeof(ISub).ImplementsOrInharits(typeof(IBase)));
        }

        [TestMethod]
        public void ClassImplementsOwnInterface()
        {
            Assert.IsTrue(typeof(Sub).ImplementsOrInharits(typeof(ISub)));
        }

        [TestMethod]
        public void ClassImplementsInheritedInterface()
        {
            Assert.IsTrue(typeof(Sub).ImplementsOrInharits(typeof(IBase)));
        }

        [TestMethod]
        public void ClassImplementsInheritedInterfaceLevel2()
        {
            Assert.IsTrue(typeof(SubSub).ImplementsOrInharits(typeof(IBase)));
        }

        [TestMethod]
        public void ClassIsInheritedFromObject()
        {
            Assert.IsTrue(typeof(Base).ImplementsOrInharits(typeof(object)));
        }

        [TestMethod]
        public void InterfaceIsInheritedFromObject()
        {
            Assert.IsTrue(typeof(IBase).ImplementsOrInharits(typeof(object)));
        }

        [TestMethod]
        public void InterfaceIsImplemented()
        {
            Assert.IsFalse(typeof(IBase).ImplementsOrInharits(typeof(Base)));
        }

        [TestMethod]
        public void InterfaceIsImplemented2()
        {
            Assert.IsFalse(typeof(IBase).ImplementsOrInharits(typeof(Sub)));
        }

        [TestMethod]
        public void InterfaceIsNotImplemented()
        {
            Assert.IsFalse(typeof(ISub).ImplementsOrInharits(typeof(Base)));
        }

        [TestMethod]
        public void ClassNotImplementsInterface()
        {
            Assert.IsFalse(typeof(Base).ImplementsOrInharits(typeof(ISub)));
        }

        [TestMethod]
        public void InterfaceNotImplementsInterface()
        {
            Assert.IsFalse(typeof(IBase).ImplementsOrInharits(typeof(IOther)));
        }

        [TestMethod]
        public void InterfaceIsImplemented3()
        {
            Assert.IsFalse(typeof(IBase).ImplementsOrInharits(typeof(SubSub)));
        }

        [TestMethod]
        public void NoEuqality()
        {
            Assert.IsFalse(typeof(Base).ImplementsOrInharits(typeof(Other)));
        }

        [TestMethod]
        public void NullAgaingstNull()
        {
            Assert.IsFalse(((Type)null).ImplementsOrInharits(null));
        }

        [TestMethod]
        public void NullAgaingstAny()
        {
            Assert.IsFalse(((Type)null).ImplementsOrInharits(typeof(Base)));
        }

        [TestMethod]
        public void AnyAgaingstNull()
        {
            Assert.IsFalse(typeof(Base).ImplementsOrInharits(null));
        }
    }
}
