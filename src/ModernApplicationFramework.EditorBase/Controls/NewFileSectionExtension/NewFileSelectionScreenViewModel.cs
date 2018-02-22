using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Commands;
using ModernApplicationFramework.EditorBase.Controls.NewElementDialog;
using ModernApplicationFramework.EditorBase.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls.NewFileSectionExtension
{
    [Export(typeof(NewFileSelectionScreenViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewFileSelectionScreenViewModel : NewElementScreenViewModelBase<NewFileCommandArguments>
    {
        public override bool UsesNameProperty => true;

        public override bool UsesPathProperty => false;

        public override bool CanOpenWith => true;

        public override string NoItemsMessage => "No file templates found";

        public override string NoItemSelectedMessage => "No item selected";

        public override NewFileCommandArguments CreateResult(string name, string path)
        {
            return !(SelectedItem is ISupportedFileDefinition fileArgument)
                ? null
                : new NewFileCommandArguments(name, fileArgument.FileType.FileExtension, fileArgument.PreferredEditor);
        }
    }
}
