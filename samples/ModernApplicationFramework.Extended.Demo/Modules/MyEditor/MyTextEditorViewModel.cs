using System;
using System.ComponentModel.Composition;
using System.IO;
using ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.MyEditor
{
    [Export(typeof(MyTextEditorViewModel))]
    [Export(typeof(IEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyTextEditorViewModel : StorableEditor
    {
        public static Guid MyTextEditorId = new Guid("{4198960C-B3DA-4319-885B-7F6E13F1FAAF}");

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

        public override Guid EditorId => MyTextEditorId;
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