using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class FirstCharCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public FirstCharCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.FirstChar))
        {
        }

        public override string NameUnlocalized => "FirstChar";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{4A1FEE30-7CC9-4012-A658-0A4FAA746005}");
    }
}