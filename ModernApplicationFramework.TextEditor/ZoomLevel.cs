using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/ZoomLevel")]
    public sealed class ZoomLevel : ViewOptionDefinition<double>
    {
        public override double Default => 100.0;

        public override EditorOptionKey<double> Key => DefaultViewOptions.ZoomLevelId;
    }
}