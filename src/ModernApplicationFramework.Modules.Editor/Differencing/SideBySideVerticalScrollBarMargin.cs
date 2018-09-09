using System;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal sealed class SideBySideVerticalScrollBarMargin : VerticalScrollBarMargin
    {
        private readonly SideBySideScrollMap _sideBySideMap;

        public SideBySideVerticalScrollBarMargin(ITextView textView, SideBySideScrollMap sideBySideMap, PerformanceBlockMarker performanceBlockMarker)
            : base(textView, sideBySideMap, "VerticalScrollBar", performanceBlockMarker)
        {
            _sideBySideMap = sideBySideMap;
        }

        public bool UseSideBySideMapping
        {
            get => _sideBySideMap.UseSideBySideMapping;
            set => _sideBySideMap.UseSideBySideMapping = value;
        }

        public override void OnEditorLayoutChanged(object sender, EventArgs e)
        {
            if (UseSideBySideMapping)
            {
                Minimum = _sideBySideMap.Start;
                Maximum = _sideBySideMap.End;
                var thumbSize = _sideBySideMap.ThumbSize;
                ViewportSize = thumbSize;
                LargeChange = thumbSize;
                var firstVisibleLine = TextView.TextViewLines.FirstVisibleLine;
                if (firstVisibleLine.VisibilityState == VisibilityState.Hidden)
                    Value = _sideBySideMap.GetCoordinateAtBufferPosition(_sideBySideMap.Viewer.LeftView.TextViewLines.FirstVisibleLine.Start);
                else
                    Value = _sideBySideMap.GetCoordinateAtBufferPosition(firstVisibleLine.Start);
            }
            else
                base.OnEditorLayoutChanged(sender, e);
        }

        public override void ScrollToCoordinate(double coordinate)
        {
            if (UseSideBySideMapping)
            {
                var ofSideBySideLine = _sideBySideMap.GetPositionOfSideBySideLine((int)coordinate, false);
                if (ofSideBySideLine.Snapshot == _sideBySideMap.Viewer.LeftView.TextSnapshot)
                    _sideBySideMap.Viewer.LeftView.DisplayTextLineContainingBufferPosition(ofSideBySideLine, 0.0, ViewRelativePosition.Top);
                else
                    _sideBySideMap.Viewer.RightView.DisplayTextLineContainingBufferPosition(ofSideBySideLine, 0.0, ViewRelativePosition.Top);
            }
            else
                base.ScrollToCoordinate(coordinate);
        }
    }
}