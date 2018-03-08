using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Settings;
using ModernApplicationFramework.EditorBase.Interfaces.Settings.EditorAssociation;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public abstract class SupportedFileDefinition : ISupportedFileDefinition
    {
        private IOpenFileEditorAssociationSettings _settings;

        protected IOpenFileEditorAssociationSettings Settigns =>
            _settings ?? (_settings = IoC.Get<IOpenFileEditorAssociationSettings>());

        public abstract string ApplicationContext { get; }

        public abstract string Description { get; }

        public abstract BitmapSource MediumThumbnailImage { get; }

        public abstract BitmapSource SmallThumbnailImage { get; }
        public abstract string Name { get; }

        public abstract string PresetElementName { get; }
        public abstract int SortOrder { get; }

        public abstract string FileExtension { get; }
        
        public abstract Guid DefaultEditor { get; }

        public abstract SupportedFileOperation SupportedFileOperation { get; }

        public virtual Guid PreferredEditor
        {
            get
            {
                if (Settigns == null)
                    return DefaultEditor;
                return Settigns.GetAssociatedEditor(this).EditorId;
            }
        }

        public IEnumerable<IFileDefinitionContext> FileContexts { get; }

        protected SupportedFileDefinition(IEnumerable<IFileDefinitionContext> contexts)
        {
            FileContexts = new List<IFileDefinitionContext>(contexts);
        }

        protected SupportedFileDefinition(IFileDefinitionContext context)
        {
            FileContexts = new List<IFileDefinitionContext>{context};
        }
    }
}