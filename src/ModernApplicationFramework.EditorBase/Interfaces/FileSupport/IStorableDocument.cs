using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.FileSupport;
using Action = System.Action;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IStorableDocument : IDocumentBase
    {
        bool IsDirty { get; set; }

        bool IsNew { get; }

        Task Save(Action action);
    }
}