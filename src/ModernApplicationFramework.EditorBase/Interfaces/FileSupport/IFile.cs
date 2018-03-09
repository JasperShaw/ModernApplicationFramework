using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IFile : INotifyPropertyChanged
    {
        event EventHandler FileChanged;

        string FileName { get; set; }

        string FullFilePath { get; }

        string Path { get; }

        Task Load(Action action);

        Task Unload();
    }
}