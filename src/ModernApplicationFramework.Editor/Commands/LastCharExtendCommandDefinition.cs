using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class LastCharExtendCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public LastCharExtendCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.LastCharExt))
        {
        }

        public override string NameUnlocalized => "LastChar Extend";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{A5DD697B-9639-4693-B82E-37115B9F2D3D}");
    }
}