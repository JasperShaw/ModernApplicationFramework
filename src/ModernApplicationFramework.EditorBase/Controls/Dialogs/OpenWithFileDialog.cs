using System;
using System.IO;
using ModernApplicationFramework.Controls.Dialogs.Native;

namespace ModernApplicationFramework.EditorBase.Controls.Dialogs
{
    public class OpenWithFileDialog : CustomNativeOpenFileDialog
    {
        public override string CustomButtonText => DialogResources.OpenFileDialogButtonOpenWith;
        public override string DefaultButtonText => DialogResources.OpenFileDialogButtonOpen;

        public override Func<bool> CustomEvaluationFunc => delegate
        {
            var ext = string.Empty;
            foreach (var name in FileNames)
            {
                if (string.IsNullOrEmpty(ext))
                    ext = Path.GetExtension(name);
                else if (ext != Path.GetExtension(name))
                    return false;
            }

            return true;
        };

        public override string EvaluationFailedMessage => DialogResources.OpenFileDialogOpenWithErrorMessage;
    }
}
