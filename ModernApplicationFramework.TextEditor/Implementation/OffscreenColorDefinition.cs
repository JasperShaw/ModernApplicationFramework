using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("OverviewMarginBackground")]
    [UserVisible(true)]
    [Order(Before = "Default Priority")]
    internal sealed class OffscreenColorDefinition : EditorFormatDefinition
    {
        public OffscreenColorDefinition()
        {
            //TODO: Text
            DisplayName = "OffscreenColorDefinitionName";
            ForegroundCustomizable = false;
            BackgroundColor = Color.FromRgb(240, 240, 240);
        }
    }
}