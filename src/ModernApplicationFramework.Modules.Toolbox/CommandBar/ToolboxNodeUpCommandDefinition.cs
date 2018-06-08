using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ToolboxNodeUpCommandDefinition))]
    public class ToolboxNodeUpCommandDefinition : CommandDefinition
    {
        private ToolboxViewModel _toolbox;
        public override string NameUnlocalized => "Move up";
        public override string Text => "Move up";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{3543E589-5D75-4CF5-88BC-254A14578C69}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ToolboxNodeUpCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox as ToolboxViewModel;
            Command = new UICommand(MoveNodeUp, CanMoveNodeUp);
        }

        private bool CanMoveNodeUp()
        {
            return true;
        }

        private void MoveNodeUp()
        {
        }
    }
}
