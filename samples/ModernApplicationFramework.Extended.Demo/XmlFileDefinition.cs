using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces;

namespace ModernApplicationFramework.Extended.Demo
{
    [Export(typeof(ISupportedFileDefinition))]
    public class XmlFileDefinition : ISupportedFileDefinition
    {
        public BitmapSource MediumThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.Extended.Demo;component/Resources/XMLFile_32x.png",
            UriKind.RelativeOrAbsolute));

        public BitmapSource SmallThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.Extended.Demo;component/Resources/XMLFile_16x.png",
            UriKind.RelativeOrAbsolute));

        public string Name => "Xml File";
        public string PresetElementName => "NewXmlfile";
        public int SortOrder => 2;
        public string ApplicationContext => "General";
        public string Description => "Opens a plain xml file";
        public string FileExtension => ".xml";
        public Type PreferredEditor => typeof(SimpleTextEditorViewModel);
        public SupportedFileOperation SupportedFileOperation => SupportedFileOperation.OpenCreate;
    }
}