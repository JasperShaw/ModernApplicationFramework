using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public abstract class MafFile : IFile
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get; }

        public string FilePath { get; }

        protected MafFile(string path, string name)
        {
            FileName = name;
            FilePath = path;
        }

        public abstract Task Load(Action action);

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}