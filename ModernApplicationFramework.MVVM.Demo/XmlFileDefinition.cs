using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Demo.Modules.MyEditor;
using ModernApplicationFramework.MVVM.ViewModels;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo
{
    [Export(typeof(ISupportedFileDefinition))]
    public class XmlFileDefinition : ISupportedFileDefinition
    {
        public string Name => "Xml File";
        public string PresetElementName => "NewXmlfile";
        public int SortOrder => 2;
        public string ApplicationContext => "General";
        public string Description => "Opens a plain xml file";

        public Uri IconSource =>
            new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/TextFile_32x.png",
                UriKind.RelativeOrAbsolute);

        public FileType FileType => new FileType("XmlFile", ".xml");
        public Type PrefferedEditor => typeof(MyTextEditorViewModel);
    }
}