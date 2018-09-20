using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Editors.SimpleTextEditor
{
    [Export(typeof(IEditor))]
    [Export(typeof(SimpleTextEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class SimpleTextEditorViewModel : EditorBase.Editor.EditorBase
    {
        private string _originalText = string.Empty;

        private string _text = string.Empty;

        [Browsable(false)]
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
                NotifyOfPropertyChange();
            }
        }

        public override IEnumerable<GestureScope> GestureScopes => new[] {Basics.GestureScopes.GlobalGestureScope};

        protected override string FallbackSaveExtension => IoC.Get<IFileDefinitionManager>()
            .GetDefinitionByFilePath(Document.FileName).FileExtension;

        public override bool CanHandleFile(ISupportedFileDefinition fileDefinition)
        {
            //Accept all supported files
            return fileDefinition != null;           
        }

        public override Guid EditorId => Guids.SimpleEditorId;

        public override string LocalizedName => "Simple TextEditor";

        public override string Name => "Simple TextEditor";

        protected override async void SaveFile()
        {
            using (var stream = new FileStream(Document.FullFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, 4096, true))
                await stream.WriteAsync(Encoding.ASCII.GetBytes(_text), 0, _text.Length);
            _originalText = _text;
        }

        protected override async void LoadFile()
        {
            base.LoadFile();
            if (!string.IsNullOrEmpty(Document.FullFilePath) && File.Exists(Document.FullFilePath))
            {
                using (var reader = File.OpenText(Document.FullFilePath))
                {
                    _text = await reader.ReadToEndAsync();
                    _originalText = _text;
                    NotifyOfPropertyChange(nameof(Text));
                }
            }
        }
    }
}