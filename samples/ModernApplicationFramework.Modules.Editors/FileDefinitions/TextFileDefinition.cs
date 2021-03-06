﻿using System;
using System.ComponentModel.Composition;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.EditorBase;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.Modules.Editors.FileDefinitions
{
    [Export(typeof(ISupportedFileDefinition))]
    public class TextFileDefinition : SupportedFileDefinition
    {
        public override BitmapSource MediumThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.Modules.Editors;component/Resources/Images/TextFile_32x.png",
            UriKind.RelativeOrAbsolute));

        public override BitmapSource SmallThumbnailImage => new BitmapImage(new Uri(
            "pack://application:,,,/ModernApplicationFramework.Modules.Editors;component/Resources/Images/TextFile_16x.png",
        UriKind.RelativeOrAbsolute));

        public override string Name => FileSupportResources.TextFileDefinitionName;

        public override string TemplateName => "NewTextfile";

        public override int SortOrder => 1;

        public override string ApplicationContext => FileSupportResources.TextFileDefinitionApplicationContext;

        public override string Description => FileSupportResources.TextFileDefinitionDescription;

        public override string FileExtension => ".txt";

        public override Guid DefaultEditor => Guids.SimpleEditorId;

        public override SupportedFileOperation SupportedFileOperation => SupportedFileOperation.OpenCreate;

        [ImportingConstructor]
        private TextFileDefinition(TextFileDefinitionContext textFileDefinitionContext) : base(textFileDefinitionContext)
        {
        }
    }
}