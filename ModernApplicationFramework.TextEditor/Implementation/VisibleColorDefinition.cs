using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("OverviewMarginVisible")]
    [UserVisible(true)]
    [Order(Before = "Default Priority")]
    internal sealed class VisibleColorDefinition : EditorFormatDefinition
    {
        public VisibleColorDefinition()
        {
            //TODO: Text
            DisplayName = "VisibleColorDefinitionName";
            ForegroundColor = Color.FromRgb(0, 0, 0);
            BackgroundColor = Color.FromRgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }
    }
}