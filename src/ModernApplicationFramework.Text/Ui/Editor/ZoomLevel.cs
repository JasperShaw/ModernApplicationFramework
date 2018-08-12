using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    [Export(typeof(EditorOptionDefinition))]
    [Name("TextView/ZoomLevel")]
    public sealed class ZoomLevel : ViewOptionDefinition<double>
    {
        public override double Default => 100.0;

        public override EditorOptionKey<double> Key => DefaultViewOptions.ZoomLevelId;
    }
}