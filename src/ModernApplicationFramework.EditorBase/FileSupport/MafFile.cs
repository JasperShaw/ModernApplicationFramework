using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public abstract class MafFile : IFile
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string FileName { get; }

        public string FullFilePath { get; }

        public virtual string Path => PathUtilities.GetBaseFilePath(FullFilePath);

        protected MafFile(string path, string name)
        {
            FileName = name;
            FullFilePath = path;
            FileChangeService.Instance.AdviseFileChange(this);
        }

        public abstract Task Load(Action action);

        public virtual Task Unload()
        {
            FileChangeService.Instance.UnadviseFileChange(this);
            return Task.CompletedTask;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}