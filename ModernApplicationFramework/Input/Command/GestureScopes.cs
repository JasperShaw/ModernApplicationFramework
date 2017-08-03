using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Input.Command
{
    public static class GestureScopes
    {
        [Export] public static GestureScope GlobalGestureScope = new GestureScope("Global");
    }
}