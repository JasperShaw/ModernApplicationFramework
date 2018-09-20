using System.ComponentModel.Composition;
using ModernApplicationFramework.Input;

namespace ModernApplicationFramework.Editor
{
    public static class TextEditorGestureScope
    {
        [Export]
        public static GestureScope TextEditorScope =
            new GestureScope("{6EA96A7F-70D5-4647-BE81-2AE8B4788DB5}", EditorResources.TextEditorGestureScope);
    }
}
