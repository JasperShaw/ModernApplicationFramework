using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics
{
    public static class CommandCategories
    {
        [Export] public static CommandCategory WindowCommandCategory = new CommandCategory(CommonUI_Resources.Category_Window);
        [Export] public static CommandCategory ViewCommandCategory = new CommandCategory(CommonUI_Resources.Category_View);
        [Export] public static CommandCategory FileCommandCategory = new CommandCategory(CommonUI_Resources.Category_File);
        [Export] public static CommandCategory EditCommandCategory = new CommandCategory(CommonUI_Resources.Category_Edit);
        [Export] public static CommandCategory ToolsCommandCategory = new CommandCategory(CommonUI_Resources.Category_Tools);
    }
}
