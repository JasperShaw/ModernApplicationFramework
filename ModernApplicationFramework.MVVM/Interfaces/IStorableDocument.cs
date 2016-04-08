using System.Threading.Tasks;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IStorableDocument
    {
        bool IsNew { get; }

        string FileName { get; }

        string FilePath { get; }

        Task New(string fileName);
        Task Load(string filePath);
        Task Save(string filePath);
    }
}
