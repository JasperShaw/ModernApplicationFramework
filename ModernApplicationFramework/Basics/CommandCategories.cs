using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics
{
    public static class CommandCategories
    {
        [Export] public static CommandCategory WindowCommandCategory = new CommandCategory(Commands_Resources.Category_Window);
        [Export] public static CommandCategory ViewCommandCategory = new CommandCategory(Commands_Resources.Category_View);
        [Export] public static CommandCategory FileCommandCategory = new CommandCategory(Commands_Resources.Category_File);
        [Export] public static CommandCategory EditCommandCategory = new CommandCategory(Commands_Resources.Category_Edit);
        [Export] public static CommandCategory ToolsCommandCategory = new CommandCategory(Commands_Resources.Category_Tools);
    }
}
