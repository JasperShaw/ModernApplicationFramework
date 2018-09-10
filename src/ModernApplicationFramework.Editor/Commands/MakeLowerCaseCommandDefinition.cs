using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class MakeLowerCaseCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public MakeLowerCaseCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.MakeLowerCase))
        {
        }

        public override string NameUnlocalized => "Make Lower Case";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{AB4B96A9-22AF-408C-8B9D-9394D40E7760}");
    }
}