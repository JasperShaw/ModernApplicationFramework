using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Editor.Commanding;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [Export(typeof(IEdtiorGestureScopeProvider))]
    [ContentType("VerySimpleEditor")]
    [TextViewRole("INTERACTIVE")]
    [Name("SimpleEditorKeyGestureScope")]
    [Order(After = "DefaultKeyGestureScope")]
    internal class SimpleEditorKeyBindingScopeFactory : IEdtiorGestureScopeProvider
    {
        public IEnumerable<GestureScope> GetAssociatedScopes()
        {
            return new[] { LesserPriorityScope.LesserPriority };
        }
    }
}