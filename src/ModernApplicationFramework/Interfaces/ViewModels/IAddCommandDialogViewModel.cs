using System.Collections.Generic;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.DataSources;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
    /// <summary>
    /// This interface provides the view model for a dialog to add commands to the command bar
    /// </summary>
	public interface IAddCommandDialogViewModel : IScreen
	{
        /// <summary>
        /// The command to execute the changes and close the dialog
        /// </summary>
		ICommand OkClickCommand { get; }

        /// <summary>
        /// A list of all command categories
        /// </summary>
		IEnumerable<CommandBarCategory> Categories { get; }

        /// <summary>
        /// A list of all available commands
        /// </summary>
		IEnumerable<CommandBarItemDefinition> AllCommandDefinitions { get; }

        /// <summary>
        /// A list of all commands fitting to the <see cref="SelectedCategory"/>
        /// </summary>
		IEnumerable<CommandBarItemDataSource> Items { get; set; }

        /// <summary>
        /// The currently selected Category
        /// </summary>
		CommandBarCategory SelectedCategory { get; set; }

        /// <summary>
        /// The currently selected Command
        /// </summary>
		CommandBarItemDataSource SelectedItem { get; set; }

        /// <summary>
        /// Re-fills <see cref="Items"/>
        /// </summary>
		void UpdateItems();
	}
}