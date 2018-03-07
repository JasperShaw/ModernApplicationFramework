using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.EditorBase.Controls.FileChangedDialog;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileChangeWatcher : IDisposable
    {
        static readonly WpfSynchronizeInvoke SynchronizingObject = new WpfSynchronizeInvoke(Application.Current.MainWindow?.Dispatcher);

        private IFile _file;
        private FileSystemWatcher _watcher;
        public bool WasChangedExternally { get; set; }

        public FileChangeWatcher(IFile file)
        {
            _file = file ?? throw new ArgumentNullException(nameof(file));
            if (Application.Current.MainWindow != null)
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
            //if (!WasChangedExternally)
            //{
            //    WasChangedExternally = true;

            //    //if (Application.Current.MainWindow != null && Application.Current.MainWindow.IsActive)
            //    //    InvokeDelayed(delegate { MainWindow_Activated(this, EventArgs.Empty); });
            //}

            FileChangeService.Instance.PushNotification(_file);
        }

        public void Dispose()
        {
            if (_file != null)
            {
                if (Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Activated -= MainWindow_Activated;
                _file = null;
            }
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }


        //private static async void InvokeDelayed(Action action)
        //{
        //    await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
        //    Application.Current.Dispatcher.BeginInvoke(action);
        //}

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            //if (WasChangedExternally)
            //{
            //    WasChangedExternally = false;
            //    if (_file == null)
            //        return;
            //    MessageBox.Show("Changed");
            //}
        }
    }

    public class FileChangeService
    {
        Dictionary<IFile ,FileChangeWatcher> _watchers  = new Dictionary<IFile, FileChangeWatcher>();
        private static FileChangeService _instance;

        private readonly HashSet<IFile> _queue = new HashSet<IFile>();

        public static FileChangeService Instance => _instance ?? (_instance = new FileChangeService());

        public FileChangeService()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Activated += OnMainWindowOnActivated;
        }

        private async void OnMainWindowOnActivated(object sender, EventArgs args)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            var editorProvider = IoC.Get<IEditorProvider>();
            var flag = false;
            foreach (var file in _queue.ToList())
            {
                if (!editorProvider.IsFileOpen(file.FullFilePath, out var editor))
                    await file.Unload();
                if (flag)
                {
                    await Application.Current.Dispatcher.BeginInvoke((System.Action)(async () => { await editor.Reload(); }));
                    continue;
                }
                var dialog = new FileChangeDialogViewModel(file);
                var result = dialog.ShowDialog();
                if (result == ReloadFileDialogResult.NoAll)
                    break;
                if (result == ReloadFileDialogResult.No)
                    continue;
                if (result == ReloadFileDialogResult.Yes)
                    await Application.Current.Dispatcher.BeginInvoke((System.Action) (async () => { await editor.Reload(); }));
                if (result == ReloadFileDialogResult.YesAll)
                {
                    flag = true;
                    await Application.Current.Dispatcher.BeginInvoke((System.Action)(async () => { await editor.Reload(); }));
                }
            }
            _queue.Clear();
        }


        public void AdviseFileChange(IFile file)
        {
            var watcher = ObtainFileWatcher(file);
            if (_watchers == null)
                _watchers = new Dictionary<IFile, FileChangeWatcher>();
            _watchers.Add(file, watcher);
        }

        public void UnadviseFileChange(IFile file)
        {
            if (_watchers.TryGetValue(file, out var watcher))
                watcher.Dispose();
            _watchers.Remove(file);
        }


        public void PushNotification(IFile file)
        {
            _queue.Add(file);
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
    }
}
