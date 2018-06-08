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
    [Export(typeof(ToolboxNodeDownCommandDefinition))]
    public class ToolboxNodeDownCommandDefinition : CommandDefinition
    {
        private ToolboxViewModel _toolbox;
        public override string NameUnlocalized => "Move Down";
        public override string Text => "Move Down";
        public override string ToolTip => Text;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{FC1C2BD3-A600-4C0D-BE5A-63DE8EED2EA9}");
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        public override ICommand Command { get; }

        [ImportingConstructor]
        public ToolboxNodeDownCommandDefinition(IToolbox toolbox)
        {
            _toolbox = toolbox as ToolboxViewModel;
            Command = new UICommand(MoveNodeDown, CanMoveNodeDown);
        }

        private bool CanMoveNodeDown()
        {
            return true;
        }

        private void MoveNodeDown()
        {
        }
    }
}
