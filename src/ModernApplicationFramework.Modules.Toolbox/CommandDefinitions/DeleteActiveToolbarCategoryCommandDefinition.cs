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
    [Export(typeof(DeleteActiveToolbarCategoryCommandDefinition))]
    internal class DeleteActiveToolbarCategoryCommandDefinition : CommandDefinition<IDeleteActiveToolbarCategoryCommand>
    {
        public override string NameUnlocalized => ToolboxResources.ResourceManager.GetString(nameof(ToolboxResources.DeleteCategoryCommand_Name),
            CultureInfo.InvariantCulture);

        public override string Name => ToolboxResources.DeleteCategoryCommand_Name;
        public override string Text => ToolboxResources.DeleteCategoryCommand_Text;
        public override string ToolTip => Text;
        public override CommandBarCategory Category => CommandBarCategories.ToolsCategory;
        public override Guid Id => new Guid("{2A33CF7A-4C10-4FA7-A766-A45F1661F4DF}");
    }
}
