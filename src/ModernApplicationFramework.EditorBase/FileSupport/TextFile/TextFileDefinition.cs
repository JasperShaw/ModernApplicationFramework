using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport.TextFile
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

        public string Name => FileSupportResources.TextFileDefinitionName;

        public string PresetElementName => "NewTextfile";

        public int SortOrder => 1;

        public string ApplicationContext => FileSupportResources.TextFileDefinitionApplicationContext;

        public string Description => FileSupportResources.TextFileDefinitionDescription;

        public string FileExtension => ".txt";

        public IEnumerable<IFileDefinitionContext> FileContexts { get; }

        public Guid PreferredEditor => Guids.SimpleEditorId;

        public SupportedFileOperation SupportedFileOperation => SupportedFileOperation.OpenCreate;

        [ImportingConstructor]
        private TextFileDefinition(TextFileDefinitionContext textFileDefinitionContext)
        {
            FileContexts = new List<IFileDefinitionContext>{textFileDefinitionContext};
        }
    }
}