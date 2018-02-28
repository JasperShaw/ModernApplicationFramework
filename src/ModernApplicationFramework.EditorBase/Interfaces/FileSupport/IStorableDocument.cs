using System.Threading.Tasks;
using Action = System.Action;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IStorableDocument : IDocument
    {
        string FileName { get; }

        string FilePath { get; }

        bool IsDirty { get; set; }

        bool IsNew { get; }

        Task Load(Action action);

        Task Save(Action action);
    }
}