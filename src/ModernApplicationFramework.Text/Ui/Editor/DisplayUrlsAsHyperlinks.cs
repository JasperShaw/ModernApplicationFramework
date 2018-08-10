using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/DisplayUrlsAsHyperlinks")]
    public sealed class DisplayUrlsAsHyperlinks : EditorOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.DisplayUrlsAsHyperlinksId;
    }
}