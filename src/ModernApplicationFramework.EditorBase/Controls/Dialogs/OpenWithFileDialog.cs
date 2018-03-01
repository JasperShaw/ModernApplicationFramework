using ModernApplicationFramework.Controls.Dialogs.Native;

namespace ModernApplicationFramework.EditorBase.Controls.Dialogs
{
    public class OpenWithFileDialog : CustomNativeOpenFileDialog
    {
        public override string CustomButtonText => "Open with...";
        public override string DefaultButtonText => "Open";
    }
}
