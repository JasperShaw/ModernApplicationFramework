using System.Threading.Tasks;

namespace ModernApplicationFramework.EditorBase.Interfaces.Layout
{
    public interface IStorableDocument : IDocument
    {
        string FileName { get; }

        string FilePath { get; }

        bool IsDirty { get; set; }

        bool IsNew { get; }

        Task Load(string filePath);

        Task Save(string filePath);

        void ResetState();
    }
}