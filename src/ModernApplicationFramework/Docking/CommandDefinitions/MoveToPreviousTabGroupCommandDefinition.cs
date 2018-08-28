using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MoveToPreviousTabGroupCommandDefinition))]
    public sealed class MoveToPreviousTabGroupCommandDefinition : CommandDefinition<IMoveToPreviousTabGroupCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("MoveToPreviousTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.MoveToPreviousTabGroupCommandDefinition_Text;
        public override string ToolTip => null;

        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{8E11AADE-8B7D-44C1-815F-8D5D6C3C9644}");
    }
}