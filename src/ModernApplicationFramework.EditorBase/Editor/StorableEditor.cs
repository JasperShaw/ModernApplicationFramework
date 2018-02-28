using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Layout;

namespace ModernApplicationFramework.EditorBase.Editor
{
    public abstract class StorableEditor : KeyBindingLayoutItem, IStorableEditor
    {
        public IStorableDocument Document { get; protected set; }

        public async Task SaveFile()
        {
            var filePath = Path.GetFileName(Document.FilePath);
            await Document.Save(() => SaveFile(filePath));
        }

        public async Task LoadFile(IStorableDocument document, string name)
        {
            DisplayName = name;
            Document = document;
            await Document.Load(() => LoadFile(document));
        }

        public abstract Guid EditorId { get; }
        public abstract string Name { get; }

        public virtual void UpdateDisplayName()
        {
            DisplayName = Document.IsDirty ? Document.FileName + "*" : Document.FileName;
        }

        protected abstract void SaveFile(string filePath);

        protected abstract void LoadFile(IStorableDocument document);
    }
}