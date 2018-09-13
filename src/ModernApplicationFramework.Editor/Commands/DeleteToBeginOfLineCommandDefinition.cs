using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class DeleteToBeginOfLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public DeleteToBeginOfLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.DeleteToBeginOfLine))
        {
        }

        public override string NameUnlocalized => "DeleteToBeginOfLine";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{DD73C69E-E5EA-454E-A551-E6DA65E0BB07}");
    }
}