using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class TabifySelectionCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public TabifySelectionCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.TabifySelection))
        {
        }

        public override string NameUnlocalized => "Tabify Selection";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{934F0608-5E52-4618-91EB-E4BE3CDEF55E}");
    }
}