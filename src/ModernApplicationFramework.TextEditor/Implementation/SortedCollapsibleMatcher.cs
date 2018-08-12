using System.Collections.Generic;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Outlining;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class SortedCollapsibleMatcher
    {
        private readonly IEnumerator<ITrackingSpan> _trackingSpanEnum;
        private readonly ITextSnapshot _textSnapshot;
        private ITrackingSpan _currentTrackingSpan;

        internal bool MatchReturn { get; set; }

        internal SortedCollapsibleMatcher(IEnumerable<ITrackingSpan> trackingSpans, ITextSnapshot textSnapshot)
        {
            _trackingSpanEnum = trackingSpans?.GetEnumerator();
            _textSnapshot = textSnapshot;
            if (_trackingSpanEnum != null && _textSnapshot != null)
                _currentTrackingSpan = _trackingSpanEnum.MoveNext() ? _trackingSpanEnum.Current : null;
            MatchReturn = true;
        }

        internal bool Match(ICollapsible givenCollapsible)
        {
            if (givenCollapsible != null)
            {
                while (_currentTrackingSpan != null)
                {
                    SnapshotSpan span1 = _currentTrackingSpan.GetSpan(_textSnapshot);
                    SnapshotSpan span2 = givenCollapsible.Extent.GetSpan(_textSnapshot);
                    int num = span1.Start != span2.Start ? span1.Start.CompareTo(span2.Start) : -span1.Length.CompareTo(span2.Length);
                    if (num <= 0)
                    {
                        _currentTrackingSpan = _trackingSpanEnum.MoveNext() ? _trackingSpanEnum.Current : null;
                        if (num == 0)
                            return MatchReturn;
                    }
                    else
                        break;
                }
            }
            return !MatchReturn;
        }
    }
}