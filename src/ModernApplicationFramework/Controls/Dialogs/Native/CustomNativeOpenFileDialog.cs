using System.ComponentModel;
using Microsoft.Win32;
using ModernApplicationFramework.Native.Shell;

namespace ModernApplicationFramework.Controls.Dialogs.Native
{
    /*
        Microsoft seems not to realize developers might need at some point a proper dialog to choose folders from explorer. 
        They accomplished that in their own tools by using the WindowsAPI but did not create an .NET version. 

        This solution is based on Ookii's source code available here: https://www.nuget.org/packages/Ookii.Dialogs/
    */
    public abstract class CustomNativeOpenFileDialog : NativeOpenFileDialog
    {
        private const int CustomItemId = 0x0003;
        private const int DefaultItemId = 0x0002;
        private const int OpenDropDownId = 0x0001;

        public abstract string CustomButtonText { get; }

        [DefaultValue(false)]
        [Description("A value indicating whether the read-only check box is selected.")]
        [Category("Behavior")]
        public bool CustomSelected { get; set; }

        public abstract string DefaultButtonText { get; }

        [Description("A value indicating whether the dialog box contains a read-only check box.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool IsCustom { get; set; }

        protected CustomNativeOpenFileDialog()
        {
            if (!IsVistaFileDialogSupported)
                DownlevelDialog = new OpenFileDialog();
        }

        public override void Reset()
        {
            base.Reset();
            IsCustom = false;
            CustomSelected = false;
        }

        internal override void ExtendDialogProperties(IFileDialog dialog)
        {
            if (IsCustom)
            {
                var customize = (IFileDialogCustomize) dialog;
                customize.EnableOpenDropDown(OpenDropDownId);
                customize.AddControlItem(OpenDropDownId, DefaultItemId, DefaultButtonText);
                customize.AddControlItem(OpenDropDownId, CustomItemId, CustomButtonText);
            }
        }

        internal override void GetResult(IFileDialog dialog)
        {
            if (IsCustom)
            {
                var customize = (IFileDialogCustomize) dialog;
                customize.GetSelectedControlItem(OpenDropDownId, out var selected);
                CustomSelected = selected == CustomItemId;
            }

            base.GetResult(dialog);
        }
    }
}