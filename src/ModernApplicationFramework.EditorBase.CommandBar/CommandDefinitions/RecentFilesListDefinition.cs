﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(RecentFilesListDefinition))]
    public class RecentFilesListDefinition : ListCommandDefinition
    {
        public override string Name => CommandsResources.RecentFileListCommand;

        public override string NameUnlocalized =>
            CommandsResources.ResourceManager.GetString("RecentFileListCommand", CultureInfo.InvariantCulture);
        public override CommandBarCategory Category => CommandBarCategories.FileCategory;
        public override Guid Id => new Guid("{FFF71602-7C5D-4689-898A-E79D9F1D1AE2}");
    }
}
