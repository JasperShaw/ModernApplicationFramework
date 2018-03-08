using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Controls.FileChangedDialog;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Settings.Documents;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileChangeService
    {
        private Dictionary<IFile ,FileChangeWatcher> _watchers  = new Dictionary<IFile, FileChangeWatcher>();
        private static FileChangeService _instance;

        private readonly HashSet<IFile> _queue = new HashSet<IFile>();
        private ExternalChangeSettings _settings;

        public static FileChangeService Instance => _instance ?? (_instance = new FileChangeService());

        private ExternalChangeSettings Settings => _settings ?? (_settings = IoC.Get<ExternalChangeSettings>());

        public FileChangeService()
        {
            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Activated += OnMainWindowOnActivated;
            Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Settings.DetectFileChangesOutsideIde)))
            {
                ActivateWatchers(Settings.DetectFileChangesOutsideIde);
            }
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

                if (_settings.AutoloadExternalChanges && file is IStorableFile storableFile && !storableFile.IsDirty)
                {
                    await Application.Current.Dispatcher.BeginInvoke((System.Action)(async () => { await editor.Reload(); }));
                    continue;
                }
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

        public void ActivateWatchers(bool activate)
        {
            if (activate)
                EnableWatchers();
            else
                DisableWatchers();
        }

        private void DisableWatchers()
        {
            foreach (var watcher in _watchers)
                watcher.Value.Enabled = false;
            _queue.Clear();
        }

        private void EnableWatchers()
        {
            foreach (var watcher in _watchers)
                watcher.Value.Enabled = true;
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