using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    /// <summary>
    /// Implementation to list all tool bars as a menu item. By clicking on the menu item the tool bars visibility will be toggled
    /// </summary>
    /// <seealso cref="ICommandListHandler{TCommandDefinition}" />
    [Export(typeof(ICommandHandler))]
    public class ShowToolBarListCommandHandler : ICommandListHandler<ListToolBarsCommandListDefinition>
    {
        private readonly IToolBarHostViewModel _toolBarHost;

        [ImportingConstructor]
        public ShowToolBarListCommandHandler(IToolBarHostViewModel shell)
        {
            _toolBarHost = shell;
        }

        public void Populate(Command command, List<CommandDefinitionBase> commands)
        {
            foreach (var toolbarDefinition in _toolBarHost.TopLevelDefinitions)
            {
                var definition =
                    new CommandListHandlerDefinition(toolbarDefinition.Text,
                        new ShowSelectedToolBarCommand(toolbarDefinition));

                if (((ToolbarDefinition) toolbarDefinition).IsVisible)
                    definition.IsChecked = true;
                commands.Add(definition);
            }
        }

        private class ShowSelectedToolBarCommand : CommandDefinitionCommand
        {
            public ShowSelectedToolBarCommand(object args) : base(args)
            {    
            }

            protected override bool OnCanExecute(object parameter)
            {
                return parameter is ToolbarDefinition;
            }

            protected override void OnExecute(object parameter)
            {
                if (!(parameter is ToolbarDefinition toolBarDef))
                    return;
                toolBarDef.IsVisible = !toolBarDef.IsVisible;
            }
        }
    }
}