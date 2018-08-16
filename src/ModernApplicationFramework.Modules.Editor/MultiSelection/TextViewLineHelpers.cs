using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class TextViewLineHelpers
    {
        internal static double GetXCoordinateFromVirtualBufferPosition(ITextViewLine textLine, VirtualSnapshotPoint bufferPosition)
        {
            if (!bufferPosition.IsInVirtualSpace && !(bufferPosition.Position == textLine.Start))
                return textLine.GetExtendedCharacterBounds(bufferPosition.Position - 1).Trailing;
            return textLine.GetExtendedCharacterBounds(bufferPosition).Leading;
        }
    }
}