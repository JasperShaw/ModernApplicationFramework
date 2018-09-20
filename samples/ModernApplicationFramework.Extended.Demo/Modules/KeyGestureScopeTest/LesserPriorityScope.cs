using System.ComponentModel.Composition;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    internal static class LesserPriorityScope
    {
        [Export] public static GestureScope LesserPriority = new GestureScope("{30F49FC3-7B9E-46CD-AE5C-539E2E434404}", "Lesser Priority");
    }
}
