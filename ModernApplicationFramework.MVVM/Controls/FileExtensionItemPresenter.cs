using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Core.CommandArguments;
using ModernApplicationFramework.MVVM.Views;

namespace ModernApplicationFramework.MVVM.Controls
{
    public class FileExtensionItemPresenter : NewElementItemPresenter
    {
        public override object CreateResult(string name, string path)
        {
            var fileArgument = SelectedItem as ISupportedFileDefinition;
            return fileArgument == null
                ? null
                : new NewFileCommandArguments(name, fileArgument.FileType.FileExtension, fileArgument.PrefferedEditor);

        }
    }
}
