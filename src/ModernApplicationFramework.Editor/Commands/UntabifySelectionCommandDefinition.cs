using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class UntabifySelectionCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public UntabifySelectionCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.UntabifySelection))
        {
        }

        public override string NameUnlocalized => "Untabify Selection";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{ED74A3AC-C011-4F78-9995-B25C30192AA6}");
    }
}