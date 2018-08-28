using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(CloseAllDockedWindowCommandDefinition))]
    public sealed class CloseAllDockedWindowCommandDefinition : CommandDefinition<ICloseAllDockedWindowCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("CloseAllDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.CloseAllDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;

        public override ImageMoniker ImageMonikerSource => Monikers.CloseDocumentGroup;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{343572A0-6C5A-4FFE-9E84-E1B6E68C82FB}");
    }
}