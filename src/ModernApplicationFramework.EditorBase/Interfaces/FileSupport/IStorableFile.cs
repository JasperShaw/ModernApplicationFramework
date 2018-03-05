using System.Threading.Tasks;
using Action = System.Action;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IStorableFile : IFile
    {
        bool IsDirty { get; set; }

        bool IsNew { get; }

        Task Save(Action action);
    }
}