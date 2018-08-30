using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public abstract class MafFile : IFile
    {
        private string _fileName;
        private string _fullFilePath;
        private bool _isFileNameChanging;
        private bool _isFilePathChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler FileChanged;

        public string FileName
        {
            get => _fileName;
            set
            {
                if (value == _fileName)
                    return;
                _isFileNameChanging = true;
                _fileName = value;
                if (!string.IsNullOrEmpty(FullFilePath) && !_isFilePathChanging)
                    FullFilePath = System.IO.Path.Combine(Path, _fileName);
                _isFileNameChanging = false;
                OnPropertyChanged();
            }
        }

        public string FullFilePath
        {
            get => _fullFilePath;
            protected set
            {
                if (value == _fullFilePath)
                    return;
                _isFilePathChanging = true;
                _fullFilePath = value;
                if (!_isFileNameChanging && !string.IsNullOrEmpty(_fullFilePath))
                {
                    var possibleNewFileName = System.IO.Path.GetFileName(_fullFilePath);
                    if (!possibleNewFileName.Equals(_fileName))
                        FileName = possibleNewFileName;
                }
                _isFilePathChanging = false;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Path));
                FileChanged?.Invoke(this, EventArgs.Empty);
            }
        }

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