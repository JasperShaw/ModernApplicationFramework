using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("Inactive Selected Text")]
    [UserVisible(true)]
    internal sealed class InactiveSelectionProperties : EditorFormatDefinition
    {
        public InactiveSelectionProperties()
        {
            BackgroundColor = SystemColors.GrayTextColor;
            DisplayName = "Inactive Selected Text";
        }
    }
}