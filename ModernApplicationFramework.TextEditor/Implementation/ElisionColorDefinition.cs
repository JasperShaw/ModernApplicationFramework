using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("OverviewMarginCollapsedRegion")]
    [UserVisible(true)]
    [Order(Before = "Default Priority")]
    internal sealed class ElisionColorDefinition : EditorFormatDefinition
    {
        public ElisionColorDefinition()
        {
            //TODO: Text
            DisplayName = "ElisionColorDefinitionName";
            ForegroundCustomizable = false;
            BackgroundColor = Color.FromRgb(155, 165, 185);
        }
    }
}