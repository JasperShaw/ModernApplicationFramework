using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ToggleShowAllItemsCommandDefinition))]
    public class ToggleShowAllItemsCommandDefinition : CommandDefinition<IToggleShowAllItemsCommand>
    {
        public override string NameUnlocalized => "Show All";
        public override string Text => "Show All";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{BB1C5EAB-A114-4A06-995C-E311F9DA8C11}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface IToggleShowAllItemsCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(IToggleShowAllItemsCommand))]
    internal class ToggleShowAllItemsCommand : CommandDefinitionCommand, IToggleShowAllItemsCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public ToggleShowAllItemsCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected override void OnExecute(object parameter)
        {
            _toolbox.ShowAllItems = !_toolbox.ShowAllItems;
        }
    }
}
