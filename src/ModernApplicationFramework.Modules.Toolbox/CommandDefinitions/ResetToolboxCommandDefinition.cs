﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Modules.Toolbox.Interfaces.Commands;
using ModernApplicationFramework.Modules.Toolbox.Resources;

namespace ModernApplicationFramework.Modules.Toolbox.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    internal class ResetToolboxCommandDefinition : CommandDefinition<IResetToolboxCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.ResetToolboxCommand_Name),
            CultureInfo.InvariantCulture);
        public override string Name => ToolboxResources.ResetToolboxCommand_Name;
        public override string Text => ToolboxResources.ResetToolboxCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.ToolsCategory;
        public override Guid Id => new Guid("{BF0ED4C1-518C-4B30-8FD3-2085A19C63D2}");
    }
}
