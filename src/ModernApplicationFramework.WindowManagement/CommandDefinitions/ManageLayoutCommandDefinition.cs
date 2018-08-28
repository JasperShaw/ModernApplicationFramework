using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.WindowManagement.Interfaces.Commands;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ManageLayoutCommandDefinition))]
    public sealed class ManageLayoutCommandDefinition : CommandDefinition<IManageLayoutCommand>
    {
        public override string Name => WindowManagement_Resources.ManageLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.ManageLayoutCommandDefinition_Text;

        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("ManageLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;
        public override CommandBarCategory Category => CommandCategories.WindowCategory;
        public override Guid Id => new Guid("{F843A14E-3840-4CC9-AA3F-D70B04DD0ED3}");
    }
}
