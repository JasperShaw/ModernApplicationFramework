using System.ComponentModel.Composition;

namespace ModernApplicationFramework.CommandBase
{
    public static class CommandGestureCategories
    {
        [Export] public static CommandGestureCategory GlobalGestureCategory = new CommandGestureCategory("Global");
    }
}