using System;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Controls.Dialogs.Native;
using ModernApplicationFramework.EditorBase.Dialogs.EditorSelectorDialog;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.Dialogs.OpenWithFileDialog
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

        public override Func<bool> CustomExeuteFunc => delegate
        {
            var selectorModel = IoC.Get<OpenFileEditorSelectorViewModel>();
            selectorModel.TargetExtension = IoC.Get<IFileDefinitionManager>()
                .GetDefinitionByExtension(Path.GetExtension(FileNames.FirstOrDefault()));
            if (IoC.Get<IWindowManager>().ShowDialog(selectorModel) != true)
                return false;
            CustomResultData = selectorModel.Result;
            return true;
        };

        public override string EvaluationFailedMessage => DialogResources.OpenFileDialogOpenWithErrorMessage;
    }
}
