using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextViewHost/IsInContrastMode")]
    public sealed class IsInContrastModeOption : EditorOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultTextViewHostOptions.IsInContrastModeId;
    }
}