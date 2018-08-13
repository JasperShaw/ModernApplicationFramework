using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    internal static class TextEditorScope
    {
        [Export] public static GestureScope TextEditor = new GestureScope("{117154EB-C22B-49F2-85B7-600F4512C4FF}", "Text-Editor");
    }
}
