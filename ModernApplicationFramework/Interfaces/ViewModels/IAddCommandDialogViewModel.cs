using System.Collections.Generic;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Interfaces.ViewModels
{
	public interface IAddCommandDialogViewModel : IScreen
	{
		ICommand OkClickCommand { get; }

		IEnumerable<CommandCategory> Categories { get; }

		IEnumerable<DefinitionBase> AllCommandDefinitions { get; }

		IEnumerable<CommandBarItemDefinition> Items { get; set; }

		CommandCategory SelectedCategory { get; set; }

		CommandBarItemDefinition SelectedItem { get; set; }

		void UpdateItems();
	}
}