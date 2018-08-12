using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal sealed class ForwardFidelityCustomTrackingSpan : ForwardFidelityTrackingSpan
    {
        private readonly object _customState;
        private readonly CustomTrackToVersion _behavior;

        public ForwardFidelityCustomTrackingSpan(ITextVersion version, Span span, object customState, CustomTrackToVersion behavior)
            : base(version, span, SpanTrackingMode.Custom)
        {
            _behavior = behavior ?? throw new ArgumentNullException(nameof(behavior));
            _customState = customState;
        }

        protected override Span TrackSpanForwardInTime(Span span, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            return _behavior(this, currentVersion, targetVersion, span, _customState);
        }

        protected override Span TrackSpanBackwardInTime(Span span, ITextVersion currentVersion, ITextVersion targetVersion)
        {
            return _behavior(this, currentVersion, targetVersion, span, _customState);
        }
    }
}