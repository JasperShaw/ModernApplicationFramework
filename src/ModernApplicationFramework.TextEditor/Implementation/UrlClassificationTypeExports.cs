using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Logic.Classification;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    internal static class UrlClassificationTypeExports
    {
        [Export]
        [Name("url")]
        internal static ClassificationTypeDefinition UrlClassificationType;

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "url")]
        [Name("urlformat")]
        [UserVisible(true)]
        [Order(After = "High Priority")]
        internal sealed class UrlHyperlinkFormat : ClassificationFormatDefinition
        {
            public UrlHyperlinkFormat()
            {
                TextDecorations = new TextDecorationCollection
                {
                    System.Windows.TextDecorations.Underline
                };
                ForegroundColor = Colors.Blue;

                //TODO: Text
                DisplayName = "UrlClassificationDisplayName";
            }
        }
    }
}