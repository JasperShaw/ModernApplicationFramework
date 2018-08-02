using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/DisplayUrlsAsHyperlinks")]
    public sealed class DisplayUrlsAsHyperlinks : EditorOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.DisplayUrlsAsHyperlinksId;
    }
}