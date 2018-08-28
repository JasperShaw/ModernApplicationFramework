using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Extended.CommandBar.Resources;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(SwitchToDocumentCommandListDefinition))]
    public class SwitchToDocumentCommandListDefinition : ListCommandDefinition
    {
        public override string Name => Commands_Resources.SwitchToDocumentCommandListDefinition_Name;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("SwitchToDocumentCommandListDefinition_Name",
                CultureInfo.InvariantCulture);

        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{84279C46-2B2C-4A9B-A06B-6FCFF4487E61}");
    }
}