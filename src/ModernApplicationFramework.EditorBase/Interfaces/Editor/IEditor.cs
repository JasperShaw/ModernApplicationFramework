using System;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.EditorBase.Interfaces.Editor
{
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