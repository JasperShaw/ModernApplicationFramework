using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Interop;
using ModernApplicationFramework.Native.NativeMethods;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Native.Shell;
using ModernApplicationFramework.Native.Standard;
using IShellItem = ModernApplicationFramework.Native.Shell.IShellItem;
using NativeMethods = ModernApplicationFramework.Native.NativeMethods.NativeMethods;

namespace ModernApplicationFramework.Controls.Dialogs
{
    /// <summary>
    /// Microsoft seems not to realize developers might need at some point a proper dialog to choose folders from explorer. 
    /// They accomplished that in their own tools by using the WindowsAPI but did not create an .NET version. 
    /// 
    /// This solution is based on Ookii's source code available here: https://www.nuget.org/packages/Ookii.Dialogs/
    /// </summary>

    [DefaultEvent("HelpRequest"),
     Designer(
         "System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
         ), DefaultProperty("SelectedPath"), Description("Prompts the user to select a folder.")]
    public sealed class FolderBrowserDialog
    {
        private string _description;
        private string _selectedPath;

        public FolderBrowserDialog()
        {
            Reset();
        }


        /// <summary>
        /// "The descriptive text displayed above the tree view control in the dialog box, or below the list view control in the Vista style dialog."
        /// </summary>
        [Category("Folder Browsing"), DefaultValue(""), Localizable(true), Browsable(true),
         Description(
             "The descriptive text displayed above the tree view control in the dialog box, or below the list view control in the Vista style dialog."
             )]
        public string Description
        {
            get => _description ?? string.Empty;
            set => _description = value;
        }

        /// <summary>
        /// "The root folder where the browsing starts from. This property has no effect if the Vista style dialog is used."
        /// </summary>
        [Localizable(false),
         Description(
             "The root folder where the browsing starts from. This property has no effect if the Vista style dialog is used."
             ), Category("Folder Browsing"), Browsable(true),
         DefaultValue(typeof(Environment.SpecialFolder), "Desktop")]
        public Environment.SpecialFolder RootFolder { get; set; }

        /// <summary>
        /// The path value that was selected
        /// </summary>
        [Browsable(true),
         Editor(
             "System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
             typeof(System.Drawing.Design.UITypeEditor)), Description("The path selected by the user."),
         DefaultValue(""), Localizable(true), Category("Folder Browsing")]
        public string SelectedPath
        {
            get => _selectedPath ?? string.Empty;
            set => _selectedPath = value;
        }

        /// <summary>
        /// A value indicating whether the New Folder button appears in the folder browser dialog box. This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
        /// </summary>
        [Browsable(true), Localizable(false),
         Description(
             "A value indicating whether the New Folder button appears in the folder browser dialog box. This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown."
             ), DefaultValue(true), Category("Folder Browsing")]
        public bool ShowNewFolderButton { get; set; }

        /// <summary>
        /// A value that indicates whether to use the value of the Description property as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
        /// </summary>
        [Category("Folder Browsing"), DefaultValue(false),
         Description(
             "A value that indicates whether to use the value of the Description property as the dialog title for Vista style dialogs. This property has no effect on old style dialogs."
             )]
        public bool UseDescriptionForTitle { get; set; }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            _description = string.Empty;
            UseDescriptionForTitle = false;
            _selectedPath = string.Empty;
            RootFolder = Environment.SpecialFolder.Desktop;
            ShowNewFolderButton = true;
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <returns>Indicator whether the dialog was closed by canceling (<see langword="false"/>) or finishing the action (<see langword="true"/>)</returns>
        public bool? ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <returns>Indicator whether the dialog was closed by canceling (<see langword="false"/>) or finishing the action (<see langword="true"/>)</returns>
        public bool? ShowDialog(System.Windows.Window owner)
        {
            IntPtr ownerHandle = owner == null ? User32.GetActiveWindow() : new WindowInteropHelper(owner).Handle;
            return RunDialog(ownerHandle);
        }

        private bool RunDialog(IntPtr ownerHandle)
        {
            IFileDialog dialog = null;
            try
            {
                dialog = new NativeFileOpenDialog();
                SetDialogProperties(dialog);
                int result = dialog.Show(ownerHandle);
                if (result < 0)
                {
                    if ((uint) (result & 0xFFFF) == (uint) Hresult.ERROR_CANCELLED.Code)
                        return false;
                    else
                        throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(result);
                }
                GetResult(dialog);
                return true;
            }
            finally
            {
                if (dialog != null)
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(dialog);
            }
        }

        private void GetResult(IFileDialog dialog)
        {
            IShellItem item;
            dialog.GetResult(out item);
            item.GetDisplayName(SIGDN.FileSysPath, out _selectedPath);
        }

        private void SetDialogProperties(IFileDialog dialog)
        {
            // Description
            if (!string.IsNullOrEmpty(_description))
            {
                if (UseDescriptionForTitle)
                {
                    dialog.SetTitle(_description);
                }
                else
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    IFileDialogCustomize customize = (IFileDialogCustomize) dialog;
                    customize.AddText(0, _description);
                }
            }

            dialog.SetOptions(Fos.FosPickfolders | Fos.FosForcefilesystem | Fos.FosFilemustexist);

            if (!string.IsNullOrEmpty(_selectedPath))
            {
                string parent = Path.GetDirectoryName(_selectedPath);
                if (parent == null || !Directory.Exists(parent))
                {
                    dialog.SetFileName(_selectedPath);
                }
                else
                {
                    string folder = Path.GetFileName(_selectedPath);
                    dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }
    }
}