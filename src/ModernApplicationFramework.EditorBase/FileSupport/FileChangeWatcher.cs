using System;
using System.IO;
using System.Windows;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileChangeWatcher : IDisposable
    {
        static readonly WpfSynchronizeInvoke SynchronizingObject = new WpfSynchronizeInvoke(Application.Current.MainWindow?.Dispatcher);

        private IFile _file;
        private FileSystemWatcher _watcher;
        private bool _enabled;


        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (_watcher != null)
                    _watcher.EnableRaisingEvents = value;
            }
        }

        public FileChangeWatcher(IFile file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            if (_watcher != null)
                _watcher.EnableRaisingEvents = false;

            if (string.IsNullOrEmpty(_file.Path))
                return;
            if (!Path.IsPathRooted(_file.Path))
                return;

            try
            {
                if (_watcher == null)
                {
                    _watcher = new FileSystemWatcher {SynchronizingObject = SynchronizingObject};
                    _watcher.Changed += OnFileChangedEvent;
                }

                _watcher.Path = _file.Path;
                _watcher.Filter = _file.FileName;
                _watcher.EnableRaisingEvents = true;
            }
            catch (PlatformNotSupportedException)
            {
                _watcher?.Dispose();
                _watcher = null;
            }
            catch (FileNotFoundException)
            {
                _watcher?.Dispose();
                _watcher = null;
            }
            catch (ArgumentException)
            {
                _watcher?.Dispose();
                _watcher = null;
            }
        }

        private void OnFileChangedEvent(object sender, FileSystemEventArgs e)
        {
            if (_file == null)
                return;
           
            FileChangeService.Instance.PushNotification(_file);
        }

        public void Dispose()
        {
            _file = null;
            if (_watcher == null)
                return;
            _watcher.Dispose();
            _watcher = null;
        }
    }
}
