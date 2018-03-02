using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;

namespace ModernApplicationFramework.EditorBase.Tests.OpenSaveFilterBuild
{
    [TestClass]
    public class BuildTests
    {
        [TestMethod]
        public void EmptyFilterTest()
        {
            var fd = new FilterData();
            Assert.AreEqual(fd.ToString(), string.Empty);
        }

        [TestMethod]
        public void EmptyFilterAddAnyAtEndTest()
        {
            var fd = new FilterData {AddFilterAnyFileAtEnd = true};
            Assert.AreEqual(string.Empty, fd.ToString());
        }

        [TestMethod]
        public void EmptyFilterMaxIndexTest()
        {
            var fd = new FilterData { AddFilterAnyFileAtEnd = true };
            Assert.AreEqual(0, fd.MaxIndex);
        }

        [TestMethod]
        public void OneEntryFilterTest()
        {
            var fd = new FilterData();
            fd.AddFilter(new FilterDataEntry("Text", new List<string>{".txt"}));


            Assert.AreEqual("Text (*.txt)|*.txt", fd.Filter);
        }

        [TestMethod]
        public void EmptyFilterAddAnyTest()
        {
            var fd = new FilterData();
            fd.AddFilterAnyFile("Any");
            Assert.AreEqual("Any (*.*)|*.*", fd.ToString());
        }


        [TestMethod]
        public void OneEntryFilterWithAnyAtEndTest()
        {
            var fd = new FilterData {AddFilterAnyFileAtEnd = true};
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt" }));
            Assert.AreEqual("Text (*.txt)|*.txt", fd.Filter);
        }

        [TestMethod]
        public void OneEntryAndAnyFilterWithAnyAtEndTest()
        {
            var fd = new FilterData { AddFilterAnyFileAtEnd = true };
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt" }));
            Assert.AreEqual("Text (*.txt)|*.txt|Any (*.*)|*.*", fd.Filter);
        }

        [TestMethod]
        public void OneEntryAndAnyFilterWithAnyAtRealPositionTest()
        {
            var fd = new FilterData();
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt" }));
            Assert.AreEqual("Any (*.*)|*.*|Text (*.txt)|*.txt", fd.Filter);
        }

        [TestMethod]
        public void OneEntryAndAnyFilterWithAnyAtRealPositionTest2()
        {
            var fd = new FilterData();
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt" }));
            fd.AddFilter(new FilterDataEntry("Xml", new List<string> { ".xml" }));
            Assert.AreEqual("Any (*.*)|*.*|Text (*.txt)|*.txt|Xml (*.xml)|*.xml", fd.Filter);
        }

        [TestMethod]
        public void OneEntryAndAnyFilterWithAnyAtRealPositionTest3()
        {
            var fd = new FilterData();
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt" }));
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Xml", new List<string> { ".xml" }));
            Assert.AreEqual("Text (*.txt)|*.txt|Any (*.*)|*.*|Xml (*.xml)|*.xml", fd.Filter);
        }

        [TestMethod]
        public void OneEntryAndAnyFilterWithAnyAtRealPositionTest4()
        {
            var fd = new FilterData();
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt" }));
            fd.AddFilter(new FilterDataEntry("Xml", new List<string> { ".xml" }));
            fd.AddFilterAnyFile("Any");
            Assert.AreEqual("Text (*.txt)|*.txt|Xml (*.xml)|*.xml|Any (*.*)|*.*", fd.Filter);
        }


        [TestMethod]
        public void MultiEntryFilterTest()
        {
            var fd = new FilterData();
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt", ".ini" }));
            Assert.AreEqual("Text (*.txt, *.ini)|*.txt; *.ini", fd.Filter);
        }

        [TestMethod]
        public void MultiEntryAndAnyFilterWithAnyAtEndTest()
        {
            var fd = new FilterData{AddFilterAnyFileAtEnd = true};
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt", ".ini" }));
            Assert.AreEqual("Text (*.txt, *.ini)|*.txt; *.ini|Any (*.*)|*.*", fd.Filter);
        }

        [TestMethod]
        public void MultiEntryAndAnyFilterWithAnyAtRealPositionTest()
        {
            var fd = new FilterData();
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Text", new List<string> { ".txt", ".ini" }));
            Assert.AreEqual("Any (*.*)|*.*|Text (*.txt, *.ini)|*.txt; *.ini", fd.Filter);
        }

        [TestMethod]
        public void InvalidEntryFilterTest()
        {
            var fd = new FilterData();
            fd.AddFilter(new FilterDataEntry("Text", new List<string>()));
            Assert.AreEqual(string.Empty, fd.Filter);
        }

        [TestMethod]
        public void InvalidEntryAndAnyFilterWithAnyAtRealPositionTest2()
        {
            var fd = new FilterData();
            fd.AddFilterAnyFile("Any");
            fd.AddFilter(new FilterDataEntry("Text", new List<string>()));
            fd.AddFilter(new FilterDataEntry("Xml", new List<string> { ".xml" }));
            Assert.AreEqual("Any (*.*)|*.*|Xml (*.xml)|*.xml", fd.Filter);
        }
    }
}
