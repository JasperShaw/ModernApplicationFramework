using System.ComponentModel.Composition;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(AddCommandDialogViewModel))]
    public sealed class AddCommandDialogViewModel : Screen
    {

        public ICommand OkClickCommand => new Command(ExecuteOkClick);


        public AddCommandDialogViewModel()
        {
            DisplayName = "Add Command";
        }

        private void ExecuteOkClick()
        {
            TryClose(true);
        }

    }
}
