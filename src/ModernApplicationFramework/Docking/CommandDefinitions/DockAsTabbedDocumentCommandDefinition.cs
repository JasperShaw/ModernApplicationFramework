using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(DockAsTabbedDocumentCommandDefinition))]
    public sealed class DockAsTabbedDocumentCommandDefinition : CommandDefinition<IDockAsTabbedDocumentCommand>
    {
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("DockAsTabbedDocumentCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.DockAsTabbedDocumentCommandDefinition_Text;
        public override string ToolTip => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{D82DF723-6ED3-43D3-805A-918CC256F6F4}");
    }
}