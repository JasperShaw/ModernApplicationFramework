using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class UnindentCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public UnindentCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.Unindent))
        {
        }

        public override string NameUnlocalized => "Unindent";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{C7405FE9-9137-4F95-8E65-228CF56A3A4D}");
    }
}