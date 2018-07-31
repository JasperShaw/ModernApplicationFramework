using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/UseVirtualSpace")]
    public sealed class UseVirtualSpace : ViewOptionDefinition<bool>
    {
        public override bool Default => false;

        public override EditorOptionKey<bool> Key => DefaultTextViewOptions.UseVirtualSpaceId;
    }
}