using System.Windows.Forms;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal static class ClipboardDataFormats
    {
        public static readonly ushort StringFormatId = (ushort)DataFormats.GetFormat(DataFormats.StringFormat).Id;
        public static readonly ushort CSVFormatId = (ushort)DataFormats.GetFormat(DataFormats.CommaSeparatedValue).Id;
        public static readonly ushort HTMLFormatId = (ushort)DataFormats.GetFormat(DataFormats.Html).Id;
        public static readonly ushort BoxSelectionCutCopyTagId = (ushort)DataFormats.GetFormat(SimpleTextViewWindow.BoxSelectionCutCopyTag).Id;
        public static readonly ushort ClipboardLineBasedCutCopyTagId = (ushort)DataFormats.GetFormat(SimpleTextViewWindow.ClipboardLineBasedCutCopyTag).Id;
        public const ushort TextFormatId = 1;
        public const ushort UnicodeTextFormatId = 13;
    }
}