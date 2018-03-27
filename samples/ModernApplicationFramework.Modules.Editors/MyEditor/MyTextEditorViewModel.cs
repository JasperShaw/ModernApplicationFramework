using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input.Command;
using File = System.IO.File;

namespace ModernApplicationFramework.Modules.Editors.MyEditor
{
    [Export(typeof(MyTextEditorViewModel))]
    [Export(typeof(IEditor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyTextEditorViewModel : EditorBase.Editor.EditorBase
    {
        public static Guid MyTextEditorId = new Guid("{4198960C-B3DA-4319-885B-7F6E13F1FAAF}");

        private string _originalText = string.Empty;

        private string _text;

        [IgnoreProperty(true)]
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

        protected override string FallbackSaveExtension => ".xml";

        public override bool CanHandleFile(ISupportedFileDefinition fileDefinition)
        {
            return fileDefinition != null;
        }

        public override Guid EditorId => MyTextEditorId;
        public override string LocalizedName => "My TextEditor";
        public override string Name => "My TextEditor";

        protected override async void SaveFile()
        {
            using (var stream = new FileStream(Document.FullFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, true))
                await stream.WriteAsync(Encoding.ASCII.GetBytes(_text), 0, _text.Length);
            _originalText = _text;
        }

        protected override void LoadFile()
        {
            base.LoadFile();
            if (!string.IsNullOrEmpty(Document.FullFilePath) && File.Exists(Document.FullFilePath))
            {
                _text = File.ReadAllText(Document.FullFilePath);
                _originalText = File.ReadAllText(Document.FullFilePath);
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