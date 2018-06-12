using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SortItemsAlphabeticallyCommandDefinition))]
    public class SortItemsAlphabeticallyCommandDefinition : CommandDefinition
    {
        private readonly IToolbox _toolbox;
        public override string NameUnlocalized => "Sort";
        public override string Text => "Sort";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{A2C9C04A-75EB-44A5-9272-D6B9DEA1D417}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public SortItemsAlphabeticallyCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox;
            Command = new UICommand(Sort, CanSort);
        }

        private void Sort()
        {
            
        }

        private bool CanSort()
        {
            return _toolbox.SelectedCategory != null && _toolbox.SelectedCategory.Items.Count >= 2;
        }
    }
}
