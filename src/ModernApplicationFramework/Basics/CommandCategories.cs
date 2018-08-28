using System.ComponentModel.Composition;
using System.Globalization;
using System.Resources;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics
{
    /// <summary>
    /// A set of common command categories
    /// </summary>
    public static class CommandCategories
    {
        private static readonly ResourceManager Rm = CommonUI_Resources.ResourceManager;

        [Export] public static CommandBarCategory WindowCategory =
            new CommandBarCategory(Rm.GetString("Category_Window", CultureInfo.InvariantCulture),
                CommonUI_Resources.Category_Window);

        [Export] public static CommandBarCategory ViewCategory =
            new CommandBarCategory(Rm.GetString("Category_View", CultureInfo.InvariantCulture),
                CommonUI_Resources.Category_View);

        [Export] public static CommandBarCategory FileCategory =
            new CommandBarCategory(Rm.GetString("Category_File", CultureInfo.InvariantCulture),
                CommonUI_Resources.Category_File);

        [Export] public static CommandBarCategory EditCategory =
            new CommandBarCategory(Rm.GetString("Category_Edit", CultureInfo.InvariantCulture),
                CommonUI_Resources.Category_Edit);

        [Export] public static CommandBarCategory ToolsCategory =
            new CommandBarCategory(Rm.GetString("Category_Tools", CultureInfo.InvariantCulture),
                CommonUI_Resources.Category_Tools);
    }
}
