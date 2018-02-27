using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.Layout;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls.SimpleTextEditor
{
    [Export(typeof(IEditor))]
    [Export(typeof(SimpleTextEditorViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //Ensures we can create multiple documents at the same type
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

        public override Task LoadFile(IStorableDocument document, string name)
        {
            DisplayName = name;
            Document = document;
            if (!string.IsNullOrEmpty(document.FilePath) && File.Exists(document.FilePath))
                _originalText = File.ReadAllText(document.FilePath);
            return Task.FromResult(true);
        }

        public override Guid EditorId => new Guid("{8DC81487-9A02-4C83-847D-C5869BC6F647}");
        public override string Name => "Simple TextEditor";

        protected override Task SaveFile(string filePath)
        {
            File.WriteAllText(filePath, _text);
            _originalText = _text;
            return Task.FromResult(true);
        }
    }

    public abstract class StorableEditor : KeyBindingLayoutItem, IStorableEditor
    {
        public IStorableDocument Document { get; protected set; }

        public async Task SaveFile()
        {
            var filePath = Path.GetFileName(Document.FilePath);
            await SaveFile(filePath);
            Document.ResetState();
        }

        protected abstract Task SaveFile(string filePath);

        public abstract Task LoadFile(IStorableDocument document, string name);

        public abstract Guid EditorId { get; }
        public abstract string Name { get; }

        public virtual void UpdateDisplayName()
        {
            DisplayName = Document.IsDirty ? Document.FileName + "*" : Document.FileName;
        }
    }

    public interface IStorableEditor : IEditor<IStorableDocument>
    {
        IStorableDocument Document { get; }

        Task SaveFile();
    }

    public interface INormalEditor : IEditor<IDocument>
    {
        IDocument Document { get; }
    }

    public interface IEditor<in T> : IEditor where T : IDocument
    {
        Task LoadFile(T document, string name);
    }

    public interface IEditor : ILayoutItem, ICanHaveInputBindings
    {
        Guid EditorId { get; }

        string Name { get; }

        void UpdateDisplayName();
    }

}