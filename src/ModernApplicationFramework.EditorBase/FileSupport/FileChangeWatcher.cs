using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileChangeWatcher : IDisposable
    {
        static readonly WpfSynchronizeInvoke SynchronizingObject = new WpfSynchronizeInvoke(Application.Current.MainWindow?.Dispatcher);

        private IFile _file;
        FileSystemWatcher _watcher;
        private bool _wasChangedExternally = false;

        public FileChangeWatcher(IFile file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
            Application.Current.MainWindow.Activated += MainWindow_Activated;
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
                    _watcher.Created += OnFileChangedEvent;
                    _watcher.Renamed += OnFileChangedEvent;
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
            catch (ArgumentException e)
            {
                _watcher?.Dispose();
                _watcher = null;
            }
        }

        private void OnFileChangedEvent(object sender, FileSystemEventArgs e)
        {
            if (_file == null)
                return;
            if (!_wasChangedExternally)
            {
                _wasChangedExternally = true;
                if (Application.Current.MainWindow.IsActive)
                {
                    InvokeDelayed(delegate { MainWindow_Activated(this, EventArgs.Empty); });
                }    
            }
        }

        public void Dispose()
        {
            if (_file != null)
            {
                _file = null;
            }
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }


        private async void InvokeDelayed(Action action)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            if (_wasChangedExternally)
            {
                _wasChangedExternally = false;
                if (_file == null)
                    return;
                MessageBox.Show("Changed");
            }
        }
    }

    public class FileChangeService
    {
        Dictionary<IFile ,FileChangeWatcher> _watchers  = new Dictionary<IFile, FileChangeWatcher>();
        private static FileChangeService _instance;

        public static FileChangeService Instance => _instance ?? (_instance = new FileChangeService());


        public void AdviseFileChange(IFile file)
        {
            var watcher = ObtainFileWatcher(file);
            if (_watchers == null)
                _watchers = new Dictionary<IFile, FileChangeWatcher>();
            _watchers.Add(file, watcher);
        }

        public void UnadviseFileChange(IFile file)
        {

        }


        private FileChangeWatcher ObtainFileWatcher(IFile file)
        {
            return FindFileWatcher(file) ?? new FileChangeWatcher(file);
        }

        private FileChangeWatcher FindFileWatcher(IFile file)
        {
            if (_watchers == null)
                return null;
            _watchers.TryGetValue(file, out var fileWatcher);
            return fileWatcher;
        }



        //internal static string ValidateAndNormalize(string path, string paramName)
        //{
        //    Validate.IsNotNull(path, paramName);
        //    path = Path.GetFullPath(path);
        //    Validate.IsNotEmpty(path, paramName);
        //    if (path.Length >= 2 && path[path.Length - 1] == 92 && path[path.Length - 2] != 58)
        //        path = path.Substring(0, path.Length - 1);
        //    return path;
        //}

    }
}
