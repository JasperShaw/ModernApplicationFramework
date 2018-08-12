using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.HighContrast
{
    internal static class HighContrastSelectionClassificationExports
    {
        internal const string ClassificationTypeName = "HighContrastSelection";
        internal const string ClassificationFormatName = "Selected Text in High Contrast";
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("HighContrastSelection")]
        internal static ClassificationTypeDefinition HighContrastSelectionClassificationType;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "HighContrastSelection")]
        [Name("Selected Text in High Contrast")]
        [UserVisible(true)]
        [Order(After = "High Priority")]
        internal sealed class HighContrastSelectionFormat : ClassificationFormatDefinition
        {
            public HighContrastSelectionFormat()
            {
                //TODO: Text
                DisplayName = "Selected Text in High Contrast";
                ForegroundColor = SystemColors.HighlightTextColor;
            }
        }
    }
}
