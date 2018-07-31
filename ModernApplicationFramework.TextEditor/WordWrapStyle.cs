using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/WordWrapStyle")]
    public sealed class WordWrapStyle : ViewOptionDefinition<WordWrapStyles>
    {
        public override WordWrapStyles Default => WordWrapStyles.None;

        public override EditorOptionKey<WordWrapStyles> Key => DefaultTextViewOptions.WordWrapStyleId;
    }
}