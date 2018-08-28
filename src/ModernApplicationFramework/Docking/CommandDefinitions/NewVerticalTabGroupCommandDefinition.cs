using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(NewVerticalTabGroupCommandDefinition))]
    public sealed class NewVerticalTabGroupCommandDefinition : CommandDefinition<INewVerticalTabGroupCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("NewVerticalTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.NewVerticalTabGroupCommandDefinition_Text;
        public override string ToolTip => null;

        public override ImageMoniker ImageMonikerSource => Monikers.SplitScreenVertical;
        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{5667C276-A91F-428A-86A3-7D95814B4B9F}");
    }
}