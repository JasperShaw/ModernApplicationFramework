using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class LastCharCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public LastCharCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.LastChar))
        {
        }

        public override string NameUnlocalized => "LastChar";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{B45784B8-12E0-4C3C-8BB4-911674EFDEE1}");
    }
}