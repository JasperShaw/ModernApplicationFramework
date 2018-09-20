using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.KeyGestureScope
{
    [Export(typeof(IEdtiorGestureScopeProvider))]
    [Name("DefaultKeyGestureScope")]
    [ContentType("any")]
    [TextViewRole("INTERACTIVE")]
    internal class DefaultEditorKeyGestureScope : IEdtiorGestureScopeProvider
    {
        public IEnumerable<GestureScope> GetAssociatedScopes()
        {
            return new[]
            {
                TextEditorGestureScope.TextEditorScope,
                GestureScopes.GlobalGestureScope
            };
        }
    }
}