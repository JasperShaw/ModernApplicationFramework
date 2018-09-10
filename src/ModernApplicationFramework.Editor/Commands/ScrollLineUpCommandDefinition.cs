using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ScrollLineUpCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public ScrollLineUpCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.ScrollUp))
        {
        }

        public override string NameUnlocalized => "Scroll Line Up";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{E7995AFA-8BD0-46FE-8D2E-90EED3D4450B}");
    }
}