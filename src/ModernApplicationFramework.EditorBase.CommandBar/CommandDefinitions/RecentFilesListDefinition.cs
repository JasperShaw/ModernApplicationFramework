using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(RecentFilesListDefinition))]
    public class RecentFilesListDefinition : CommandListDefinition
    {
        public override string Name => CommandsResources.RecentFileListCommand;

        public override string NameUnlocalized =>
            CommandsResources.ResourceManager.GetString("RecentFileListCommand", CultureInfo.InvariantCulture);
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{FFF71602-7C5D-4689-898A-E79D9F1D1AE2}");
    }
}
