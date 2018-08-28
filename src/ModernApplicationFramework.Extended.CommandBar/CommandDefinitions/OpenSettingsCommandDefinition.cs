using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.ItemDefinitions;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(OpenSettingsCommandDefinition))]
    public sealed class OpenSettingsCommandDefinition : CommandDefinition<IOpenSettingsCommand>
    {
        public override ImageMoniker ImageMonikerSource => Monikers.Settings;

        public override string Text => Commands_Resources.OpenSettingsCommandDefinition_Text;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("OpenSettingsCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Text;

        public override CommandBarCategory Category => CommandCategories.ToolsCategory;
        public override Guid Id => new Guid("{71F57742-F483-4E3C-A5DD-79596A86CEC7}");
    }
}