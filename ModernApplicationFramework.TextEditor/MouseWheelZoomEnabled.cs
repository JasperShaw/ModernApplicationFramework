using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/MouseWheelZoom")]
    public sealed class MouseWheelZoomEnabled : ViewOptionDefinition<bool>
    {
        public override bool Default => true;

        public override EditorOptionKey<bool> Key => DefaultViewOptions.EnableMouseWheelZoomId;
    }
}