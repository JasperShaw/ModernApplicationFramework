using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class DeleteToEndOfLineCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public DeleteToEndOfLineCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.DeleteToEndOfLine))
        {
        }

        public override string NameUnlocalized => "DeleteToEndOfLine";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{941A81D5-68AE-4B19-9287-1A98CD58192C}");
    }
}