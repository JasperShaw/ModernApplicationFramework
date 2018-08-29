using System.ComponentModel.Composition;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Editor
{
    public static class TextEditorGestureScope
    {
        [Export]
        public static GestureScope TextEditorScope =
            new GestureScope("{6EA96A7F-70D5-4647-BE81-2AE8B4788DB5}", EditorResources.TextEditorGestureScope);

        [Export]
        public static GestureScope OutputGestureScope =
            new GestureScope("{4D37FB4F-DCC4-4EA2-9832-0893A4A75C73}", "Output");
    }
}
