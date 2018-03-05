using System;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.EditorBase.Interfaces.Editor
{
    public interface IEditor : ILayoutItem, ICanHaveInputBindings
    {
        Guid EditorId { get; }

        string Name { get; }

        IDocumentBase Document { get; }

        bool IsReadOnly { get; }

        Task LoadFile(IDocumentBase document, string name);

        Task SaveFile();

        bool CanHandleFile(ISupportedFileDefinition fileDefinition);
    }
}