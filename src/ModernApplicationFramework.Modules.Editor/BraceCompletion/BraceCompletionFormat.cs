using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.BraceCompletion
{
    [Export(typeof(EditorFormatDefinition))]
    [Name(FormatName)]
    [UserVisible(true)]
    [Order(Before = "Default Priority")]
    internal sealed class BraceCompletionFormat : EditorFormatDefinition
    {
        public const string FormatName = "BraceCompletionClosingBrace";

        public BraceCompletionFormat()
        {
            //TODO: Localize
            DisplayName = "Closing Brace";
            BackgroundBrush = Brushes.LightBlue;
            ForegroundCustomizable = false;
            BackgroundCustomizable = true;
        }
    }
}
