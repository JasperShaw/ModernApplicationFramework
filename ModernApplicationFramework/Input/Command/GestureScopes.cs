using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Input.Command
{
    public static class GestureScopes
    {
        [Export] public static GestureScope GlobalGestureScope =
            new GestureScope("{50035B74-A89B-4CAF-8920-B21443652CF0}", "Global");
    }
}