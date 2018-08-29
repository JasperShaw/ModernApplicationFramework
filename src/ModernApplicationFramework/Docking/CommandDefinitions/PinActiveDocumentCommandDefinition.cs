using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    public class PinActiveDocumentCommandDefinition : CommandDefinition<IPinActiveDocumentCommand>
    {
        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("PinActiveDocumentCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string Text => DockingResources.PinActiveDocumentCommandDefinition_Text;
        public override ImageMoniker ImageMonikerSource => Monikers.PinDocument;
        public override bool Checkable => true;
        public override string ToolTip => NameUnlocalized;
        public override CommandBarCategory Category => CommandBarCategories.WindowCategory;
        public override Guid Id => new Guid("{EE167099-7700-4D02-BB20-C9258F9E2C5B}");
    }
}
