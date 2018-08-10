using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/WordWrapStyle")]
    public sealed class WordWrapStyle : ViewOptionDefinition<WordWrapStyles>
    {
        public override WordWrapStyles Default => WordWrapStyles.None;

        public override EditorOptionKey<WordWrapStyles> Key => DefaultTextViewOptions.WordWrapStyleId;
    }
}