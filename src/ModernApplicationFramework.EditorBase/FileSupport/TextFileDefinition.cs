using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Export(typeof(ISupportedFileDefinition))]
    public class TextFileDefinition : ISupportedFileDefinition
    {
        public BitmapSource MediumThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.EditorBase;component/Resources/Images/TextFile_32x.png",
            UriKind.RelativeOrAbsolute));

        public BitmapSource SmallThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.EditorBase;component/Resources/Images/TextFile_16x.png",
        UriKind.RelativeOrAbsolute));
        public string Name => "Text File";
        public string PresetElementName => "NewTextfile";
        public int SortOrder => 1;
        public string ApplicationContext => "General";
        public string Description => "Opens a plain text file";

        public string FileExtension => ".txt";

        public Guid PreferredEditor => Guids.SimpleEditorId;
        public SupportedFileOperation SupportedFileOperation => SupportedFileOperation.OpenCreate;
    }
}