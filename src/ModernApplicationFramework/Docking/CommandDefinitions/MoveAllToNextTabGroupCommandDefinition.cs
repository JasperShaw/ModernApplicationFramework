using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(MoveAllToNextTabGroupCommandDefinition))]
    public sealed class MoveAllToNextTabGroupCommandDefinition : CommandDefinition<IMoveAllToNextTabGroupCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("MoveAllToNextTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.MoveAllToNextTabGroupCommandDefinition_Text;
        public override string ToolTip => null;

        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{4719104D-6589-4899-84E7-5B00F89FA513}");
    }
}