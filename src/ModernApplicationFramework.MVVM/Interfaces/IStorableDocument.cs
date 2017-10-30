using System.Threading.Tasks;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IStorableDocument
    {
        string FileName { get; }

        string FilePath { get; }
        bool IsNew { get; }
        Task Load(string filePath);

        Task New(string fileName);
        Task Save(string filePath);
    }
}