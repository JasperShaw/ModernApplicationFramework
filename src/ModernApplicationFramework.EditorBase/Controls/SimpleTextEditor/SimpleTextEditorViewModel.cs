using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.EditorBase.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor
{
    [Export(typeof(IEditor))]
    [Export(typeof(SimpleTextEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SimpleTextEditorViewModel : StorableEditor
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
                Document.IsDirty = string.CompareOrdinal(_originalText, value) != 0;
                UpdateDisplayName();
                NotifyOfPropertyChange();
            }
        }

        public override GestureScope GestureScope => GestureScopes.GlobalGestureScope;

        public override Guid EditorId => Guids.SimpleEditorId;

        public override string Name => "Simple TextEditor";

        protected override void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, _text);
            _originalText = _text;
        }

        protected override void LoadFile(IStorableDocument document)
        {
            Document = document;
            if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
                _originalText = File.ReadAllText(document.FilePath);
        }
    }
}