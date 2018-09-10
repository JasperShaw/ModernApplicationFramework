using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class TopLineExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public TopLineExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.TopLineExt))
        {
        }

        public override string NameUnlocalized => "Top Line Extend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{1974AD78-2A27-4DFC-B940-44BD598F68F4}");
    }
}