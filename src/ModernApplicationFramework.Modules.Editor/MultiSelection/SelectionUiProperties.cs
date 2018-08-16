using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.MultiSelection
{
    internal class SelectionUiProperties : AbstractSelectionPresentationProperties
    {
        private readonly MultiSelectionBroker _broker;
        private readonly IEditorOptions _options;
        private readonly SelectionTransformer _transformer;

        public SelectionUiProperties(MultiSelectionBrokerFactory factory, MultiSelectionBroker broker, SelectionTransformer transformer)
        {
            _broker = broker;
            _options = factory.EditorOptionsFactoryService.GetOptions(_broker.TextView);
            _transformer = transformer;
        }

        internal void SetPreferredXCoordinate(double value)
        {
            PreferredXCoordinate = value;
        }

        internal void SetPreferredYCoordinate(double value)
        {
            PreferredYCoordinate = value;
        }

        public override bool IsOverwriteMode
        {
            get
            {
                if (_options.IsOverwriteModeEnabled())
                {
                    var insertionPoint = _transformer.Selection.InsertionPoint;
                    if (!insertionPoint.IsInVirtualSpace)
                    {
                        var selection = _transformer.Selection;
                        if (selection.IsEmpty)
                        {
                            var position1 = ContainingTextViewLine.End.Position;
                            selection = _transformer.Selection;
                            insertionPoint = selection.InsertionPoint;
                            var position2 = insertionPoint.Position.Position;
                            return position1 != position2;
                        }
                    }
                }
                return false;
            }
        }

        public override TextBounds CaretBounds
        {
            get
            {
                var caretWidth = CaretWidth;
                var containingTextViewLine = ContainingTextViewLine;
                return new TextBounds(!IsOverwriteMode ? GetXCoordinateFromVirtualBufferPosition(containingTextViewLine, _transformer.Selection.InsertionPoint) : ContainingTextViewLine.GetExtendedCharacterBounds(_transformer.Selection.InsertionPoint).Left, containingTextViewLine.Top, caretWidth, containingTextViewLine.Height, containingTextViewLine.TextTop, containingTextViewLine.TextHeight);
            }
        }

        public override ITextViewLine ContainingTextViewLine
        {
            get
            {
                var position = _transformer.Selection.InsertionPoint.Position;
                var containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(position);
                if (_transformer.Selection.InsertionPointAffinity == PositionAffinity.Predecessor && containingBufferPosition.Start == position && _broker.TextView.TextSnapshot.GetLineFromPosition((int)position).Start != position)
                    containingBufferPosition = _broker.TextView.GetTextViewLineContainingBufferPosition(position - 1);
                return containingBufferPosition;
            }
        }

        public double CaretWidth
        {
            get
            {
                if (IsOverwriteMode)
                    return ContainingTextViewLine.GetExtendedCharacterBounds(_transformer.Selection.InsertionPoint).Width;
                return _options.GetOptionValue(DefaultTextViewOptions.CaretWidthId);
            }
        }

        public override bool IsWithinViewport
        {
            get
            {
                if (ContainingTextViewLine.VisibilityState == VisibilityState.FullyVisible && CaretBounds.Left >= _broker.TextView.ViewportLeft)
                    return CaretBounds.Right <= _broker.TextView.ViewportRight;
                return false;
            }
        }

        internal static double GetXCoordinateFromVirtualBufferPosition(ITextViewLine textLine, VirtualSnapshotPoint bufferPosition)
        {
            if (!bufferPosition.IsInVirtualSpace && !(bufferPosition.Position == textLine.Start))
                return textLine.GetExtendedCharacterBounds(bufferPosition.Position - 1).Trailing;
            return textLine.GetExtendedCharacterBounds(bufferPosition).Leading;
        }
    }
}
