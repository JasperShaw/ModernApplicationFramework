using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class TransposeWordCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public TransposeWordCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.TransposeWord))
        {
        }

        public override string NameUnlocalized => "Transpose Word";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{C49E4545-66E5-4DB1-914A-251FFFBB82DA}");
    }
}