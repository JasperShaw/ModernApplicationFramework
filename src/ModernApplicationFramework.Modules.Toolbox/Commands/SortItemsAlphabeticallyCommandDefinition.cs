using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SortItemsAlphabeticallyCommandDefinition))]
    public class SortItemsAlphabeticallyCommandDefinition : CommandDefinition<ISortItemsAlphabeticallyCommand>
    {
        public override string NameUnlocalized => "Sort";
        public override string Text => "Sort";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{A2C9C04A-75EB-44A5-9272-D6B9DEA1D417}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;
    }

    public interface ISortItemsAlphabeticallyCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ISortItemsAlphabeticallyCommand))]
    internal class SortItemsAlphabeticallyCommand : CommandDefinitionCommand, ISortItemsAlphabeticallyCommand
    {
        private readonly IToolbox _toolbox;

        [ImportingConstructor]
        public SortItemsAlphabeticallyCommand(IToolbox toolbox)
        {
            _toolbox = toolbox;
        }

        protected override bool OnCanExecute(object parameter)
        {
            return _toolbox.SelectedCategory != null && _toolbox.SelectedCategory.Items.Count >= 2;
        }

        protected override void OnExecute(object parameter)
        {
            if (_toolbox.SelectedCategory == null)
                return;
            var selectedNode = _toolbox.SelectedNode;
            var oldLayout = _toolbox.SelectedCategory.Items.ToList();
            var newLayout = oldLayout.OrderBy(x => x.Name);
            _toolbox.SelectedCategory.Items.Clear();
            _toolbox.SelectedCategory.Items.AddRange(newLayout);
            _toolbox.SelectedNode = selectedNode;
        }
    }
}
