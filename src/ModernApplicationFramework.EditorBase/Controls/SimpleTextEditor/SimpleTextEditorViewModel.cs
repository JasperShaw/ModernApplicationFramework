using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor
{
    [Export(typeof(IEditor))]
    [Export(typeof(SimpleTextEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SimpleTextEditorViewModel : Editor.EditorBase
    {
        private string _originalText = string.Empty;

        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text)
                    return;
                _text = value;
                if (!IsReadOnly && Document is IStorableFile storableDocument)
                    storableDocument.IsDirty = string.CompareOrdinal(_originalText, value) != 0;
                UpdateDisplayName();
                NotifyOfPropertyChange();
            }
        }

        public override GestureScope GestureScope => GestureScopes.GlobalGestureScope;

        public override bool CanHandleFile(ISupportedFileDefinition fileDefinition)
        {
            //Accept all supported files
            return fileDefinition != null;
            
        }

        public override Guid EditorId => Guids.SimpleEditorId;

        public override string LocalizedName => "Simple TextEditor";

        public override string Name => "Simple TextEditor";

        protected override void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, _text);
            _originalText = _text;
        }

        protected override void LoadFile(IFile document)
        {
            base.LoadFile(document);
            if (!string.IsNullOrEmpty(document.FullFilePath) && File.Exists(document.FullFilePath))
            {
                _text = File.ReadAllText(document.FullFilePath);
                _originalText = File.ReadAllText(document.FullFilePath);
                NotifyOfPropertyChange(nameof(Text));
            }
        }

        protected override void UpdateDisplayName()
        {
            if (!(Document is IStorableFile storableDocument)) return;
            if (storableDocument.IsDirty)
                DisplayName = Document.FileName + "*";
            else
                DisplayName = Document.FileName;
        }
    }
}