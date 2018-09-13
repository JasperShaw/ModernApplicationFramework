using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Editor.Commanding;

namespace ModernApplicationFramework.Editor.Commands
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class TransposeCharCommandDefinition : CommandDefinition
    {
        [ImportingConstructor]
        public TransposeCharCommandDefinition() : base(new TextEditCommand(MafConstants.EditorCommands.TransposeChar))
        {
        }

        public override string NameUnlocalized => "Transpose Char";
        public override string Text => NameUnlocalized;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.EditCategory;
        public override Guid Id => new Guid("{CB5E061E-CCD1-4E8C-9809-D6141EA3103F}");
    }
}