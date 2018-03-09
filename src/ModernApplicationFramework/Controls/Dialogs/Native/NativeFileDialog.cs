using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Platform.Structs;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Controls.Dialogs.Native
{
    /*
        Microsoft seems not to realize developers might need at some point a proper dialog to choose folders from explorer. 
        They accomplished that in their own tools by using the WindowsAPI but did not create an .NET version. 

        This solution is based on Ookii's source code available here: https://www.nuget.org/packages/Ookii.Dialogs/
    */
    [DefaultEvent("FileOk")]
    [DefaultProperty("FileName")]
    public abstract class NativeFileDialog
    {
        internal const int HelpButtonId = 0x4001;
        private bool _addExtension;
        private string _defaultExt;

        private FileDialog _downlevelDialog;
        private string[] _fileNames;
        private string _filter;
        private int _filterIndex;
        private string _initialDirectory;
        private Fos _options;
        private Window _owner;
        private string _title;

        [Description("Event raised when the user clicks on the Open or Save button on a file dialog box.")]
        [Category("Action")]
        public event CancelEventHandler FileOk;

        public static bool IsVistaFileDialogSupported => NativeMethods.IsWindowsVistaOrLater;

        [Description(
            "A value indicating whether the dialog box automatically adds an extension to a file name if the user omits the extension.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool AddExtension
        {
            get => DownlevelDialog?.AddExtension ?? _addExtension;
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.AddExtension = value;
                else
                    _addExtension = value;
            }
        }

        [Description(
            "A value indicating whether the dialog box displays a warning if the user specifies a file name that does not exist.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public virtual bool CheckFileExists
        {
            get => DownlevelDialog?.CheckFileExists ?? GetOption(Fos.FosFilemustexist);
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.CheckFileExists = value;
                else
                    SetOption(Fos.FosFilemustexist, value);
            }
        }

        [Description(
            "A value indicating whether the dialog box displays a warning if the user specifies a path that does not exist.")]
        [DefaultValue(true)]
        [Category("Behavior")]
        public bool CheckPathExists
        {
            get => DownlevelDialog?.CheckPathExists ?? GetOption(Fos.FosPathmustexist);
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.CheckPathExists = value;
                else
                    SetOption(Fos.FosPathmustexist, value);
            }
        }

        [Category("Behavior")]
        [DefaultValue("")]
        [Description("The default file name extension.")]
        public string DefaultExt
        {
            get
            {
                if (DownlevelDialog != null)
                    return DownlevelDialog.DefaultExt;
                return _defaultExt ?? string.Empty;
            }
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.DefaultExt = value;
                }
                else
                {
                    if (value != null)
                        if (value.StartsWith(".", StringComparison.CurrentCulture))
                            value = value.Substring(1);
                        else if (value.Length == 0)
                            value = null;

                    _defaultExt = value;
                }
            }
        }

        [Category("Behavior")]
        [Description(
            "A value indicating whether the dialog box returns the location of the file referenced by the shortcut or whether it returns the location of the shortcut (.lnk).")]
        [DefaultValue(true)]
        public bool DereferenceLinks
        {
            get
            {
                if (DownlevelDialog != null)
                    return DownlevelDialog.DereferenceLinks;
                return !GetOption(Fos.FosNodereferencelinks);
            }
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.DereferenceLinks = value;
                else
                    SetOption(Fos.FosNodereferencelinks, !value);
            }
        }

        [DefaultValue("")]
        [Category("Data")]
        [Description("A string containing the file name selected in the file dialog box.")]
        public string FileName
        {
            get
            {
                if (DownlevelDialog != null)
                    return DownlevelDialog.FileName;

                if (_fileNames == null || _fileNames.Length == 0 || string.IsNullOrEmpty(_fileNames[0]))
                    return string.Empty;
                return _fileNames[0];
            }
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.FileName = value;
                _fileNames = new string[1];
                _fileNames[0] = value;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        [Description("The file names of all selected files in the dialog box.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string[] FileNames => DownlevelDialog?.FileNames ?? FileNamesInternal;

        [Description(
            "The current file name filter string, which determines the choices that appear in the \"Save as file type\" or \"Files of type\" box in the dialog box.")]
        [Category("Behavior")]
        [Localizable(true)]
        [DefaultValue("")]
        public string Filter
        {
            get => DownlevelDialog?.Filter ?? _filter;
            set
            {
                if (DownlevelDialog != null)
                {
                    DownlevelDialog.Filter = value;
                }
                else
                {
                    if (value != _filter)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            var filterElements = value.Split('|');
                            if (filterElements.Length % 2 != 0)
                                throw new ArgumentException("Invalid Filter");
                        }
                        else
                        {
                            value = null;
                        }

                        _filter = value;
                    }
                }
            }
        }

        [Description("The index of the filter currently selected in the file dialog box.")]
        [Category("Behavior")]
        [DefaultValue(1)]
        public int FilterIndex
        {
            get => DownlevelDialog?.FilterIndex ?? _filterIndex;
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.FilterIndex = value;
                else
                    _filterIndex = value;
            }
        }

        [Description("The initial directory displayed by the file dialog box.")]
        [DefaultValue("")]
        [Category("Data")]
        public string InitialDirectory
        {
            get
            {
                if (DownlevelDialog != null)
                    return DownlevelDialog.InitialDirectory;

                return _initialDirectory ?? string.Empty;
            }
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.InitialDirectory = value;
                else
                    _initialDirectory = value;
            }
        }

        [DefaultValue(false)]
        [Description("A value indicating whether the dialog box restores the current directory before closing.")]
        [Category("Behavior")]
        public bool RestoreDirectory
        {
            get => DownlevelDialog?.RestoreDirectory ?? GetOption(Fos.FosNochangedir);
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.RestoreDirectory = value;
                else
                    SetOption(Fos.FosNochangedir, value);
            }
        }

        [Description("The file dialog box title.")]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Title
        {
            get
            {
                if (DownlevelDialog != null)
                    return DownlevelDialog.Title;
                return _title ?? string.Empty;
            }
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.Title = value;
                else
                    _title = value;
            }
        }

        [DefaultValue(true)]
        [Category("Behavior")]
        [Description("A value indicating whether the dialog box accepts only valid Win32 file names.")]
        public bool ValidateNames
        {
            get
            {
                if (DownlevelDialog != null)
                    return DownlevelDialog.ValidateNames;
                return !GetOption(Fos.FosNovalidate);
            }
            set
            {
                if (DownlevelDialog != null)
                    DownlevelDialog.ValidateNames = value;
                else
                    SetOption(Fos.FosNovalidate, !value);
            }
        }

        internal string[] FileNamesInternal
        {
            private get
            {
                if (_fileNames == null) return new string[0];
                return (string[]) _fileNames.Clone();
            }
            set { _fileNames = value; }
        }

        [Browsable(false)]
        protected FileDialog DownlevelDialog
        {
            get => _downlevelDialog;
            set
            {
                _downlevelDialog = value;
                if (value != null) value.FileOk += DownlevelDialog_FileOk;
            }
        }

        protected NativeFileDialog()
        {
            Reset();
        }

        public virtual void Reset()
        {
            if (DownlevelDialog != null)
            {
                DownlevelDialog.Reset();
            }
            else
            {
                _fileNames = null;
                _filter = null;
                _filterIndex = 1;
                _addExtension = true;
                _defaultExt = null;
                _options = 0;
                _title = null;
                CheckPathExists = true;
            }
        }

        public bool? ShowDialog()
        {
            var result = ShowDialog(null);
            return result;
        }

        public bool? ShowDialog(Window owner)
        {
            _owner = owner;
            if (DownlevelDialog != null)
                return DownlevelDialog.ShowDialog(owner);
            var ownerHandle = owner == null ? User32.GetActiveWindow() : new WindowInteropHelper(owner).Handle;
            return RunFileDialog(ownerHandle);
        }

        internal abstract IFileDialog CreateFileDialog();

        internal bool DoFileOk(IFileDialog dialog)
        {
            GetResult(dialog);

            var e = new CancelEventArgs();
            OnFileOk(e);
            return !e.Cancel;
        }

        internal bool GetOption(Fos option)
        {
            return (_options & option) != 0;
        }

        protected internal virtual void GetResult(IFileDialog dialog)
        {
            if (GetOption(Fos.FosAllowmultiselect))
                return;
            _fileNames = new string[1];
            dialog.GetResult(out var result);
            result.GetDisplayName(SIGDN.FileSysPath, out _fileNames[0]);
        }

        internal bool PromptUser(string text, MessageBoxButton buttons, MessageBoxImage icon,
            MessageBoxResult defaultResult)
        {
            var caption = string.IsNullOrEmpty(_title)
                ? (this is NativeOpenFileDialog
                    ? ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.Open)
                    : ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.ConfirmSaveAs))
                : _title;
            MessageBoxOptions options = 0;
            if (Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
                options |= MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;
            return MessageBox.Show(_owner, text, caption, buttons, icon, defaultResult, options) ==
                   MessageBoxResult.Yes;
        }

        internal virtual void SetDialogProperties(IFileDialog dialog)
        {
            dialog.Advise(new NativeFileDialogEvents(this), out _);

            // Set the default file name
            if (!(_fileNames == null || _fileNames.Length == 0 || string.IsNullOrEmpty(_fileNames[0])))
            {
                var parent = Path.GetDirectoryName(_fileNames[0]);
                if (parent == null || !Directory.Exists(parent))
                {
                    dialog.SetFileName(_fileNames[0]);
                }
                else
                {
                    var folder = Path.GetFileName(_fileNames[0]);
                    dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }

            // Set the filter
            if (!string.IsNullOrEmpty(_filter))
            {
                var filterElements = _filter.Split('|');
                var filter = new ComdlgFilterspec[filterElements.Length / 2];
                for (var x = 0; x < filterElements.Length; x += 2)
                {
                    filter[x / 2].pszName = filterElements[x];
                    filter[x / 2].pszSpec = filterElements[x + 1];
                }

                dialog.SetFileTypes((uint) filter.Length, filter);

                if (_filterIndex > 0 && _filterIndex <= filter.Length)
                    dialog.SetFileTypeIndex((uint) _filterIndex);
            }

            // Default extension
            if (_addExtension && !string.IsNullOrEmpty(_defaultExt))
                dialog.SetDefaultExtension(_defaultExt);

            // Initial directory
            if (!string.IsNullOrEmpty(_initialDirectory))
            {
                var item = NativeMethods.CreateItemFromParsingName(_initialDirectory);
                dialog.SetDefaultFolder(item);
            }

            if (!string.IsNullOrEmpty(_title)) dialog.SetTitle(_title);

            dialog.SetOptions(_options | Fos.FosForcefilesystem);
        }

        internal void SetOption(Fos option, bool value)
        {
            if (value)
                _options |= option;
            else
                _options &= ~option;
        }

        protected virtual void OnFileOk(CancelEventArgs e)
        {
            var handler = FileOk;
            handler?.Invoke(this, e);
        }

        private void DownlevelDialog_FileOk(object sender, CancelEventArgs e)
        {
            OnFileOk(e);
        }

        private bool RunFileDialog(IntPtr hwndOwner)
        {
            IFileDialog dialog = null;
            try
            {
                dialog = CreateFileDialog();
                SetDialogProperties(dialog);
                var result = dialog.Show(hwndOwner);
                if (result >= 0)
                    return true;
                if ((uint) result == (uint) HResult.ErrorCancelled)
                    return false;
                else
                    throw Marshal.GetExceptionForHR(result);
            }
            finally
            {
                if (dialog != null)
                    Marshal.FinalReleaseComObject(dialog);
            }
        }
    }
}