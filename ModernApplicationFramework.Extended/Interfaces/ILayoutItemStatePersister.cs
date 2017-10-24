using System.IO;
using ModernApplicationFramework.Extended.Core.LayoutManagement;

namespace ModernApplicationFramework.Extended.Interfaces
{
    public interface ILayoutItemStatePersister
    {
        void Initialize(IDockingHostViewModel shell, IDockingHost shellView);
        void LoadState();
        void SaveState();


        void SaveToStream(Stream stream, ProcessStateOption option);


        bool HasStateFile { get; }
        void LoadFromStream(Stream stream);



        string StreamToString(Stream stream);
    }
}