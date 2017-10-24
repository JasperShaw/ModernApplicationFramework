using System.IO;
using ModernApplicationFramework.Extended.Interfaces;

namespace MordernApplicationFramework.WindowManagement.LayoutState
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