using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class IndentCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public IndentCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.OpenLineAbove))
        {
        }

        public override string NameUnlocalized => "Indent";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{8DA69C3E-CBF3-4DF1-9DB2-221007B36573}");
    }
}