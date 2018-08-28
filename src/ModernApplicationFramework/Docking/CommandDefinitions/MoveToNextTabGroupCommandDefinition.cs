using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MoveToNextTabGroupCommandDefinition))]
    public sealed class MoveToNextTabGroupCommandDefinition : CommandDefinition<IMoveToNextTabGroupCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("MoveToNextTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.MoveToNextTabGroupCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{5702930A-708C-44EB-9A14-7783FFF3503B}");
    }
}