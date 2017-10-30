using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernApplicationFramework.Input;

namespace ModernApplicationFramework.Tests.MultiKeyGesture
{
    [TestClass]
    public class GestureEqualsTests
    {
        [TestMethod]
        public void Test1()
        {
            var k1 = new Input.MultiKeyGesture(Key.F11);
            var k2 = new Input.MultiKeyGesture(Key.F11);

            Assert.IsTrue(k1.Equals(k2));
        }

        [TestMethod]
        public void Test1A()
        {
            var k1 = new Input.MultiKeyGesture(Key.F11);
            var k2 = new Input.MultiKeyGesture(Key.F11);

            Assert.IsTrue(k2.Equals(k1));
        }

        [TestMethod]
        public void Test2()
        {
            var k1 = new Input.MultiKeyGesture(Key.F12);
            var k2 = new Input.MultiKeyGesture(Key.F11);

            Assert.IsFalse(k1.Equals(k2));
        }

        [TestMethod]
        public void Test2A()
        {
            var k1 = new Input.MultiKeyGesture(Key.F12);
            var k2 = new Input.MultiKeyGesture(Key.F11);

            Assert.IsFalse(k2.Equals(k1));
        }

        [TestMethod]
        public void Test3()
        {
            var k1 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt);
            var k2 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt);
            Assert.IsTrue(k1.Equals(k2));
        }

        [TestMethod]
        public void Test3A()
        {
            var k1 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt);
            var k2 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt);
            Assert.IsTrue(k2.Equals(k1));
        }

        [TestMethod]
        public void Test4()
        {
            var k1 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt | ModifierKeys.Control);
            var k2 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt);
            Assert.IsFalse(k1.Equals(k2));
        }

        [TestMethod]
        public void Test4A()
        {
            var k1 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt | ModifierKeys.Control);
            var k2 = new Input.MultiKeyGesture(Key.E, ModifierKeys.Alt);
            Assert.IsFalse(k2.Equals(k1));
        }

        [TestMethod]
        public void Test5()
        {

            var ks1 = new KeySequence(Key.F11);
            var ks2 = new KeySequence(Key.F11);

            var k1 = new Input.MultiKeyGesture(new List<KeySequence>{ks1, ks1});
            var k2 = new Input.MultiKeyGesture(new List<KeySequence>{ks2, ks2});
            Assert.IsTrue(k1.Equals(k2));
        }

        [TestMethod]
        public void Test5A()
        {

            var ks1 = new KeySequence(Key.F11);
            var ks2 = new KeySequence(Key.F11);

            var k1 = new Input.MultiKeyGesture(new List<KeySequence> { ks1, ks1 });
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks2, ks2 });
            Assert.IsTrue(k2.Equals(k1));
        }

        [TestMethod]
        public void Test6()
        {

            var ks2 = new KeySequence(Key.F11);

            var k1 = new Input.MultiKeyGesture(Key.Enter, ModifierKeys.Alt | ModifierKeys.Shift);
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks2, ks2 });
            Assert.IsFalse(k1.Equals(k2));
        }

        [TestMethod]
        public void Test6A()
        {

            var ks2 = new KeySequence(Key.F11);

            var k1 = new Input.MultiKeyGesture(Key.Enter, ModifierKeys.Alt | ModifierKeys.Shift);
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks2, ks2 });
            Assert.IsFalse(k2.Equals(k1));
        }

        [TestMethod]
        public void Test7()
        {

            var ks11 = new KeySequence(ModifierKeys.Control, Key.C);
            var ks12 = new KeySequence(ModifierKeys.Control, Key.B);
            var ks21 = new KeySequence(ModifierKeys.Control, Key.C);
            var ks22 = new KeySequence(ModifierKeys.Control, Key.B);

            var k1 = new Input.MultiKeyGesture(new List<KeySequence> { ks11, ks12 });
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks21, ks22 });
            Assert.IsTrue(k1.Equals(k2));
        }

        [TestMethod]
        public void Test7A()
        {

            var ks11 = new KeySequence(ModifierKeys.Control, Key.C);
            var ks12 = new KeySequence(ModifierKeys.Control, Key.B);
            var ks21 = new KeySequence(ModifierKeys.Control, Key.C);
            var ks22 = new KeySequence(ModifierKeys.Control, Key.B);

            var k1 = new Input.MultiKeyGesture(new List<KeySequence> { ks11, ks12 });
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks21, ks22 });
            Assert.IsTrue(k2.Equals(k1));
        }

        [TestMethod]
        public void Test8()
        {

            var ks11 = new KeySequence(ModifierKeys.Control, Key.A);
            var ks12 = new KeySequence(ModifierKeys.Control, Key.B);
            var ks21 = new KeySequence(ModifierKeys.Control, Key.C);
            var ks22 = new KeySequence(ModifierKeys.Control, Key.B);

            var k1 = new Input.MultiKeyGesture(new List<KeySequence> { ks11, ks12 });
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks21, ks22 });
            Assert.IsFalse(k1.Equals(k2));
        }

        [TestMethod]
        public void Test8A()
        {

            var ks11 = new KeySequence(ModifierKeys.Control, Key.A);
            var ks12 = new KeySequence(ModifierKeys.Control, Key.B);
            var ks21 = new KeySequence(ModifierKeys.Control, Key.C);
            var ks22 = new KeySequence(ModifierKeys.Control, Key.B);

            var k1 = new Input.MultiKeyGesture(new List<KeySequence> { ks11, ks12 });
            var k2 = new Input.MultiKeyGesture(new List<KeySequence> { ks21, ks22 });
            Assert.IsFalse(k2.Equals(k1));
        }
    }
}
