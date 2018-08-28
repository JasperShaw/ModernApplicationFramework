using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandBarItemDefinition))]
    [Export(typeof(ListWindowLayoutsCommandDefinition))]
    public sealed class ListWindowLayoutsCommandDefinition : CommandListDefinition
    {
        public override string Name => WindowManagement_Resources.ApplyWindowLayoutListCommandDefinition_Name;

        public override string NameUnlocalized => WindowManagement_Resources.ResourceManager.GetString(
            "ApplyWindowLayoutListCommandDefinition_Name",
            CultureInfo.InvariantCulture);

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{278C1836-FB1F-4B84-920D-86622B1F37C7}");
    }
}
