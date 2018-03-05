using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ModernApplicationFramework.EditorBase.Interfaces.FileSupport
{
    public interface IFile : INotifyPropertyChanged
    {
        string FileName { get; }

        string FilePath { get; }

        Task Load(Action action);
    }
}