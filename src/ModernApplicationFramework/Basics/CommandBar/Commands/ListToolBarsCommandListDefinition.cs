﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Definition of the command to show all tool bars as a menu item
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandListDefinition" />
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ListToolBarsCommandListDefinition))]
    public class ListToolBarsCommandListDefinition : ListCommandDefinition
    {
        public override string Name => CommandBarResources.ListToolBarsCommandListDefinition_Name;

        public override string NameUnlocalized =>
            CommandBarResources.ResourceManager.GetString("ListToolBarsCommandListDefinition_Name",
                CultureInfo.InvariantCulture);

        public override CommandBarCategory Category => CommandBarCategories.ViewCategory;
        public override Guid Id => new Guid("{C9D83419-A523-4E38-B2A9-063744C29C8F}");
    }
}