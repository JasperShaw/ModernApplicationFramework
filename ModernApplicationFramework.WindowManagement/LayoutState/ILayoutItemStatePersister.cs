using System.IO;

namespace ModernApplicationFramework.WindowManagement.LayoutState
{
    internal interface ILayoutItemStatePersister
    {
        void Initialize();
        void LoadFromFile(string filePath, ProcessStateOption option);
        void LoadFromStream(Stream stream, ProcessStateOption option);
        void SaveToFile(string filePath, ProcessStateOption option);
        void SaveToStream(Stream stream, ProcessStateOption option);
    }
}