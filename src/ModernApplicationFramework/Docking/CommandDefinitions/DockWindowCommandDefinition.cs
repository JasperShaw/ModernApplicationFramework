using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(DockWindowCommandDefinition))]
    public sealed class DockWindowCommandDefinition : CommandDefinition<IDockWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("DockWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.DockWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandBarCategories.WindowCategory;
        public override Guid Id => new Guid("{5CC255C6-4D81-4B39-AF49-D80FE4C813EC}");
    }
}