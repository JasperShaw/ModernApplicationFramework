using System.IO;
using ModernApplicationFramework.Extended.Core.LayoutManagement;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ILayoutItemStatePersister
    {
        bool HasStateFile { get; }

        void Initialize(IDockingHostViewModel shell, IDockingHost shellView);

        void SaveToStream(Stream stream, ProcessStateOption option);

        void SaveToFile(string filePath, ProcessStateOption option);

        void SaveToFile(ProcessStateOption option);

        void LoadFromStream(Stream stream, ProcessStateOption option);

        void LoadFromFile(string filePath, ProcessStateOption option);

        void LoadFromFile(ProcessStateOption option);
    }
}