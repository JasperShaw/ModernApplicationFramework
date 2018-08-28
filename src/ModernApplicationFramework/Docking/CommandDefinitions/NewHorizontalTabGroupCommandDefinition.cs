using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(NewHorizontalTabGroupCommandDefinition))]
    public sealed class NewHorizontalTabGroupCommandDefinition : CommandDefinition<INewHorizontalTabGroupCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("NewHorizontalTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.NewHorizontalTabGroupCommandDefinition_Text;
        public override string ToolTip => null;

        public override ImageMoniker ImageMonikerSource => Monikers.SplitScreenHorizontal;

        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{4860E5BB-2518-4785-B448-9B8CFE85F13F}");
    }
}