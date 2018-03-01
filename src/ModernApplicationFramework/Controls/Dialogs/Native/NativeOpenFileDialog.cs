using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Controls.Dialogs.Native
{
    public class NativeOpenFileDialog : NativeFileDialog
    {
        private const int OpenDropDownId = 0x4002;
        private const int OpenItemId = 0x4003;
        private const int ReadOnlyItemId = 0x4004;
        private bool _readOnlyChecked;
        private bool _showReadOnly;

        [DefaultValue(true)]
        [Description(
            "A value indicating whether the dialog box displays a warning if the user specifies a file name that does not exist.")]
        public override bool CheckFileExists
        {
            get => base.CheckFileExists;
            set => base.CheckFileExists = value;
        }

        [Description("A value indicating whether the dialog box allows multiple files to be selected.")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool Multiselect
        {
            get => ((OpenFileDialog) DownlevelDialog)?.Multiselect ?? GetOption(Fos.FosAllowmultiselect);
            set
            {
                if (DownlevelDialog != null)
                    ((OpenFileDialog) DownlevelDialog).Multiselect = value;

                SetOption(Fos.FosAllowmultiselect, value);
            }
        }

        [DefaultValue(false)]
        [Description("A value indicating whether the read-only check box is selected.")]
        [Category("Behavior")]
        public bool ReadOnlyChecked
        {
            get => ((OpenFileDialog) DownlevelDialog)?.ReadOnlyChecked ?? _readOnlyChecked;
            set
            {
                if (DownlevelDialog != null)
                    ((OpenFileDialog) DownlevelDialog).ReadOnlyChecked = value;
                else
                    _readOnlyChecked = value;
            }
        }

        [Description("A value indicating whether the dialog box contains a read-only check box.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool ShowReadOnly
        {
            get => ((OpenFileDialog) DownlevelDialog)?.ShowReadOnly ?? _showReadOnly;
            set
            {
                if (DownlevelDialog != null)
                    ((OpenFileDialog) DownlevelDialog).ShowReadOnly = value;
                else
                    _showReadOnly = value;
            }
        }

        public NativeOpenFileDialog()
        {
            if (!IsVistaFileDialogSupported)
                DownlevelDialog = new OpenFileDialog();
        }

        public Stream OpenFile()
        {
            if (DownlevelDialog != null)
                return ((OpenFileDialog) DownlevelDialog).OpenFile();
            var fileName = FileName;
            return new FileStream(fileName, FileMode.Open, FileAccess.Read);
        }

        public override void Reset()
        {
            base.Reset();
            if (DownlevelDialog != null)
                return;
            CheckFileExists = true;
            _showReadOnly = false;
            _readOnlyChecked = false;
        }

        internal override IFileDialog CreateFileDialog()
        {
            return new NativeFileOpenDialog();
        }

        internal virtual void ExtendDialogProperties(IFileDialog dialog)
        {
            if (_showReadOnly)
            {
                var customize = (IFileDialogCustomize) dialog;
                customize.EnableOpenDropDown(OpenDropDownId);
                customize.AddControlItem(OpenDropDownId, OpenItemId,
                    ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.OpenButton));
                customize.AddControlItem(OpenDropDownId, ReadOnlyItemId,
                    ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.ReadOnly));
            }
        }

        internal override void GetResult(IFileDialog dialog)
        {
            if (Multiselect)
            {
                ((IFileOpenDialog) dialog).GetResults(out var results);
                results.GetCount(out var count);
                var fileNames = new string[count];
                for (uint x = 0; x < count; ++x)
                {
                    results.GetItemAt(x, out var item);
                    item.GetDisplayName(SIGDN.FileSysPath, out var name);
                    fileNames[x] = name;
                }

                FileNamesInternal = fileNames;
            }
            else
            {
                FileNamesInternal = null;
            }

            if (ShowReadOnly)
            {
                var customize = (IFileDialogCustomize) dialog;
                customize.GetSelectedControlItem(OpenDropDownId, out var selected);
                _readOnlyChecked = selected == ReadOnlyItemId;
            }

            base.GetResult(dialog);
        }

        internal sealed override void SetDialogProperties(IFileDialog dialog)
        {
            base.SetDialogProperties(dialog);
            ExtendDialogProperties(dialog);
        }
    }
}