using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(FloatDockedWindowCommandDefinition))]
    public sealed class FloatDockedWindowCommandDefinition : CommandDefinition<IFloatDockedWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("FloatDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.FloatDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{A4C7C240-998D-40CF-9BA0-D9BD0AE2BC1D}");
    }
}