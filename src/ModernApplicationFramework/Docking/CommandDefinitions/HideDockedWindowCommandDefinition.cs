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
    [Export(typeof(HideDockedWindowCommandDefinition))]
    public sealed class HideDockedWindowCommandDefinition : CommandDefinition<IHideDockedWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("HideDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.HideDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override ImageMoniker ImageMonikerSource => Monikers.HideToolWindow;

        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{E1BBFA22-EADF-445D-810A-4984E91D17B7}");
    }
}