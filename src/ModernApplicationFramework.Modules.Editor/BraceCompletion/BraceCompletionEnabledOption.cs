using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("BraceCompletion/Enabled")]
    public sealed class BraceCompletionEnabledOption : EditorOptionDefinition<bool>
    {
        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.BraceCompletionEnabledOptionId;

        public override bool Default => true;
    }
}
