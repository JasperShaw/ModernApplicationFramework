using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class OpenLineAboveCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public OpenLineAboveCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.OpenLineAbove))
        {
        }

        public override string NameUnlocalized => "OpenLineAbove";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{2B2596C8-5F92-4E17-BA29-C4EED47231EE}");
    }
}