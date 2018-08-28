using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CommandBar.Customize.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Data view model of the command bar customize dialog
    /// </summary>
    /// <seealso cref="!:Caliburn.Micro.Conductor{Caliburn.Micro.IScreen}.Collection.OneActive" />
    [Export(typeof(CustomizeDialogViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class CustomizeDialogViewModel : Conductor<IScreen>.Collection.OneActive
    {
        [ImportingConstructor]
        public CustomizeDialogViewModel([ImportMany] ICustomizeDialogScreen[] screens)
        {
            DisplayName = Customize_Resources.CustomizeDialog_Title;
            Items.AddRange(screens.OrderBy(x => x.SortOrder));
            ActivateItem<IToolBarsPageViewModel>();
        }

        public void ActivateItem<T>() where T : ICustomizeDialogScreen
        {
            ActiveItem = Items.OfType<T>().FirstOrDefault();
        }
    }
}