using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics
{
    public static class CommandCategories
    {
        [Export] public static CommandCategory WindowCommandCategory = new CommandCategory("Window");
        [Export] public static CommandCategory ViewCommandCategory = new CommandCategory("View");
        [Export] public static CommandCategory FileCommandCategory = new CommandCategory("File");
        [Export] public static CommandCategory EditCommandCategory = new CommandCategory("Edit");
        [Export] public static CommandCategory ToolsCommandCategory = new CommandCategory("Tools");
    }
}
