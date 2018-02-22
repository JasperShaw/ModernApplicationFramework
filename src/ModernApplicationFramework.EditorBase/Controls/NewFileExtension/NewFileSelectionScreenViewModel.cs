using System.ComponentModel.Composition;
using ModernApplicationFramework.EditorBase.Controls.NewElementDialog;
using ModernApplicationFramework.EditorBase.Core;
using ModernApplicationFramework.EditorBase.Interfaces;

namespace ModernApplicationFramework.EditorBase.Controls.NewFileExtension
{
    [Export(typeof(NewFileSelectionScreenViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewFileSelectionScreenViewModel : NewElementScreenViewModelBase<NewFileCommandArguments>
    {
        public override bool UsesNameProperty => true;

        public override bool UsesPathProperty => false;

        public override bool CanOpenWith => true;

        public override string NoItemsMessage => "Not file templates found";

        public override string NoItemSelectedMessage => "No item selected";

        public override NewFileCommandArguments CreateResult(string name, string path)
        {
            return !(SelectedItem is ISupportedFileDefinition fileArgument)
                ? null
                : new NewFileCommandArguments(name, fileArgument.FileType.FileExtension, fileArgument.PreferredEditor);
        }
    }
}
