using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Data view model of the command bar customize dialog
    /// </summary>
    /// <seealso cref="!:Caliburn.Micro.Conductor{Caliburn.Micro.IScreen}.Collection.OneActive" />
    [Export(typeof(CustomizeDialogViewModel))]
    internal sealed class CustomizeDialogViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public CustomizeDialogViewModel()
        {
            DisplayName = Customize_Resources.CustomizeDialog_Title;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            var toolbarsViewModel = IoC.Get<IToolBarsPageViewModel>();
            Items.Add(toolbarsViewModel);
            var commandsPageViewModel = IoC.Get<ICommandsPageViewModel>();
            Items.Add(commandsPageViewModel);
            ActiveItem = toolbarsViewModel;
        }
    }
}