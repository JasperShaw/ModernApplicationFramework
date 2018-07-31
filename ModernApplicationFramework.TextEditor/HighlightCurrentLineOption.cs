using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("Adornments/HighlightCurrentLine/Enable")]
    public sealed class HighlightCurrentLineOption : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultViewOptions.EnableHighlightCurrentLineId;
    }
}