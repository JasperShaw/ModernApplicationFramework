using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Modules.Editors.MyEditor;

namespace ModernApplicationFramework.Modules.Editors.FileDefinitions
{
    [Export(typeof(ISupportedFileDefinition))]
    public class XmlFileDefinition : SupportedFileDefinition
    {
        public override BitmapSource MediumThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.Modules.Editors;component/Resources/Images/XMLFile_32x.png",
            UriKind.RelativeOrAbsolute));

        public override BitmapSource SmallThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.Modules.Editors;component/Resources/Images/XMLFile_16x.png",
            UriKind.RelativeOrAbsolute));

        public override string Name => "Xml File";
        public override string TemplateName => "NewXmlfile";
        public override int SortOrder => 2;
        public override string ApplicationContext => "General";
        public override string Description => "Opens a plain xml file";
        public override string FileExtension => ".xml";
        public override Guid DefaultEditor => MyTextEditorViewModel.MyTextEditorId;
        public override SupportedFileOperation SupportedFileOperation => SupportedFileOperation.OpenCreate;

        [ImportingConstructor]
        private XmlFileDefinition(TextFileDefinitionContext context) :base(context)
        {
        }
    }
}