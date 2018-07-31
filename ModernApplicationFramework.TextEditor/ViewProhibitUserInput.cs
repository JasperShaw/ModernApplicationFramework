using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/ProhibitUserInput")]
    public sealed class ViewProhibitUserInput : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.ViewProhibitUserInputId;
    }
}