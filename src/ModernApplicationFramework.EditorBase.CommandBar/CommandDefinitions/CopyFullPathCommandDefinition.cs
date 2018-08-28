using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    public class CopyFullPathCommandDefinition : CommandDefinition<ICopyFullPathCommand>
    {
        public override string NameUnlocalized => "Copy Full Path";
        public override string Text => CommandsResources.CopyFullPathCommandText;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandCategories.FileCategory;
        public override Guid Id => new Guid("{1F2CAB1F-3624-4D2E-9855-4CD6F62F7B13}");
    }
}
