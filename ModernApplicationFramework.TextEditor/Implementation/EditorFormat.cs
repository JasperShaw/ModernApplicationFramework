using System.ComponentModel.Composition;
using System.Windows.Media;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("Block Structure Adornments")]
    [UserVisible(true)]
    internal sealed class EditorFormat : EditorFormatDefinition
    {
        public const string FormatName = "Block Structure Adornments";

        public EditorFormat()
        {
            //TODO: Text
            DisplayName = "BlockFormatName";
            BackgroundColor = Color.FromArgb(byte.MaxValue, 208, 208, 208);
            ForegroundCustomizable = false;
        }
    }
}