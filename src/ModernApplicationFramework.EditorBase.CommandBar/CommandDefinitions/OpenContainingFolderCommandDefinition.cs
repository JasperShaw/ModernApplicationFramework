using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class OpenContainingFolderCommandDefinition : CommandDefinition<IOpenContainingFolderCommand>
    {
        public override string NameUnlocalized => "Open Containing Folder";
        public override string Text => CommandsResources.OpenContainingFolderCommandText;
        public override string ToolTip => Text;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{1E11050B-F441-4C18-8A0E-B6C46D4265DE}");
    }
}
