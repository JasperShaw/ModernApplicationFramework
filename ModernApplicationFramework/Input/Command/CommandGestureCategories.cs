using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Input.Command
{
    public static class CommandGestureCategories
    {
        [Export] public static CommandGestureCategory GlobalGestureCategory = new CommandGestureCategory("Global");
    }
}