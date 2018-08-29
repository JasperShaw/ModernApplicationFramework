using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Basics.CommandBar.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of the command to open the command bar customize dialog
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.Command.CommandDefinition" />
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(CustomizeMenuCommandDefinition))]
    public sealed class CustomizeMenuCommandDefinition : CommandDefinition<ICustomizeMenuCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            CommandBarResources.ResourceManager.GetString("CustomizeMenuCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string Text => CommandBarResources.CustomizeMenuCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandBarCategories.ViewCategory;
        public override Guid Id => new Guid("{3D393097-6CCB-470C-931D-08096338F31A}");
    }
}