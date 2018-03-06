using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;
using File = System.IO.File;

namespace ModernApplicationFramework.Extended.Demo.Modules.MyEditor
{
    [Export(typeof(MyTextEditorViewModel))]
    [Export(typeof(IEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyTextEditorViewModel : EditorBase.Editor.EditorBase
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
                if (!IsReadOnly && Document is IStorableFile storableDocument)
                    storableDocument.IsDirty = string.CompareOrdinal(_originalText, value) != 0;
                UpdateDisplayName();
                NotifyOfPropertyChange();
            }
        }

        public override GestureScope GestureScope => GestureScopes.GlobalGestureScope;

        public override bool CanHandleFile(ISupportedFileDefinition fileDefinition)
        {
            return fileDefinition != null;
        }

        public override Guid EditorId => MyTextEditorId;
        public override string LocalizedName => "My TextEditor";
        public override string Name => "My TextEditor";

        protected override void SaveFile(string filePath)
        {
            File.WriteAllText(filePath, _text);
            _originalText = _text;
        }

        protected override void LoadFile(IFile document)
        {
            base.LoadFile(document);
            if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
            {
                _text = File.ReadAllText(document.FilePath);
                _originalText = File.ReadAllText(document.FilePath);
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