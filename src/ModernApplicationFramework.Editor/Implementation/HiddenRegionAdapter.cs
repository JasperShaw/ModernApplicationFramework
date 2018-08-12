using System;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Tagging;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class HiddenRegionAdapter : IOutliningRegionTag, IHiddenRegion, IHiddenRegionEx
    {
        private byte _typeStorage;
        private byte _behaviorStorage;
        private byte _stateStorage;
        private string _tipText;

        internal HiddenRegionType Type
        {
            get => (HiddenRegionType)_typeStorage;
            set => _typeStorage = (byte)value;
        }

        internal HiddenRegionBehavior Behavior
        {
            get => (HiddenRegionBehavior)_behaviorStorage;
            set => _behaviorStorage = (byte)value;
        }

        internal HiddenRegionState State
        {
            get => (HiddenRegionState)_stateStorage;
            set => _stateStorage = (byte)value;
        }

        internal HiddenTextSessionAdapter HiddenTextSessionAdapter { get; }

        internal TextSpan TextSpan { get; private set; }

        internal ITrackingSpan TrackingSpan { get; private set; }

        internal string Banner { get; set; }

        internal uint ClientData { get; set; }

        internal uint[] BannerAttr { get; private set; }

        internal bool Valid { get; set; }

        internal HiddenRegionAdapter(INewHiddenRegion newHiddenRegion, HiddenTextSessionAdapter vsHiddenTextSessionAdapter)
        {
            if (newHiddenRegion == null)
                throw new ArgumentNullException(nameof(newHiddenRegion));
            HiddenTextSessionAdapter = vsHiddenTextSessionAdapter ?? throw new ArgumentNullException(nameof(vsHiddenTextSessionAdapter));
            Type = (HiddenRegionType)newHiddenRegion.Type;
            Behavior = (HiddenRegionBehavior)newHiddenRegion.Behavior;
            State = (HiddenRegionState)newHiddenRegion.State;
            SetTextSpan(newHiddenRegion.HiddenText, false);
            Banner = newHiddenRegion.Banner;
            ClientData = newHiddenRegion.ClientData;
            BannerAttr = newHiddenRegion.BannerAttr;
            Valid = true;
        }

        public bool IsDefaultCollapsed => State == HiddenRegionState.HrsDefault;

        public bool IsImplementation => false;

        public object CollapsedForm => Banner;

        public object CollapsedHintForm
        {
            get
            {
                var pbstrText = new string[1];
                var hiddenTextClient = HiddenTextSessionAdapter != null ? HiddenTextSessionAdapter.HiddenTextClient : (IHiddenTextClient)null;
                if (hiddenTextClient == null || hiddenTextClient.GetTipText(this, pbstrText) != 0)
                    pbstrText[0] = null;
                return pbstrText[0];
            }
        }

        public int GetType(out int piHiddenRegionType)
        {
            piHiddenRegionType = (int)Type;
            return 0;
        }

        public int GetBehavior(out uint pdwBehavior)
        {
            pdwBehavior = (uint)Behavior;
            return 0;
        }

        public int GetState(out uint dwState)
        {
            if (HiddenTextSessionAdapter.GetCollapsed(this, out var collapsed))
                State = collapsed ? HiddenRegionState.HrsDefault : HiddenRegionState.HrsExpanded;
            dwState = (uint)State;
            return 0;
        }

        public int SetState(uint dwState, uint dwUpdate)
        {
            State = (HiddenRegionState)dwState;
            HiddenTextSessionAdapter.ExpandOrCollapse(this, State != HiddenRegionState.HrsExpanded);
            return 0;
        }

        public int GetBanner(out string pbstrBanner)
        {
            pbstrBanner = Banner;
            return 0;
        }

        public int SetBanner(string pszBanner)
        {
            if (Banner != pszBanner)
            {
                Banner = pszBanner;
                _tipText = null;
                HiddenTextSessionAdapter.InvalidateSpanForRegion(this);
            }
            return 0;
        }

        public int GetSpan(TextSpan[] pSpan)
        {
            if (pSpan == null)
                return -2147024809;
            if (TrackingSpan != null)
            {
                var currentSnapshot = TrackingSpan.TextBuffer.CurrentSnapshot;
                var span = TrackingSpan.GetSpan(currentSnapshot);
                int start = span.Start;
                var lineFromPosition1 = currentSnapshot.GetLineFromPosition(start);
                int end = span.End;
                var lineFromPosition2 = currentSnapshot.GetLineFromPosition(end);
                pSpan[0].iStartLine = lineFromPosition1.LineNumber;
                pSpan[0].iStartIndex = start - lineFromPosition1.Start;
                pSpan[0].iEndLine = lineFromPosition2.LineNumber;
                pSpan[0].iEndIndex = end - lineFromPosition2.Start;
            }
            else
                pSpan[0] = TextSpan;
            return 0;
        }

        public int SetSpan(TextSpan[] pSpan)
        {
            if (pSpan == null)
                return -2147024809;
            var collapsed2 = HiddenTextSessionAdapter.GetCollapsed(this, out var collapsed1);
            if (SetTextSpan(pSpan[0], true) && collapsed2 & collapsed1 && (HiddenTextSessionAdapter.GetCollapsed(this, out var collapsed3) && !collapsed3))
                HiddenTextSessionAdapter.ExpandOrCollapse(this, true);
            return 0;
        }

        public int GetClientData(out uint pdwData)
        {
            pdwData = ClientData;
            return 0;
        }

        public int SetClientData(uint dwData)
        {
            ClientData = dwData;
            return 0;
        }

        public int Invalidate(uint dwUpdate)
        {
            if (Valid)
            {
                Valid = false;
                HiddenTextSessionAdapter.RemoveRegion(this);
            }
            return 0;
        }

        public int IsValid()
        {
            return !Valid ? 0 : 1;
        }

        public int GetBaseBuffer(out IMafTextLines ppBuffer)
        {
            ppBuffer = HiddenTextSessionAdapter.DocData;
            return ppBuffer == null ? -2147418113 : 0;
        }

        public int GetBannerAttr(uint dwLength, uint[] pColorAttr)
        {
            if (BannerAttr == null)
                return -2147467263;
            if (dwLength != Banner.Length || pColorAttr == null)
                return -2147024809;
            for (uint index = 0; index < dwLength; ++index)
                pColorAttr[(int)index] = BannerAttr[(int)index];
            return 0;
        }

        public int SetBannerAttr(uint dwLength, uint[] pColorAttr)
        {
            if (dwLength != Banner.Length || pColorAttr == null)
                return -2147024809;
            BannerAttr = new uint[(int)dwLength];
            for (uint index = 0; index < dwLength; ++index)
                BannerAttr[(int)index] = pColorAttr[(int)index];
            return 0;
        }

        internal bool SetTextSpan(TextSpan textSpan, bool update)
        {
            var trackingSpan = TrackingSpanForTextSpan(textSpan);
            if (trackingSpan != null)
            {
                if (TrackingSpan != trackingSpan)
                {
                    if (update)
                    {
                        try
                        {
                            HiddenTextSessionAdapter.StartBatch();
                            HiddenTextSessionAdapter.RemoveRegion(this);
                            TrackingSpan = trackingSpan;
                            HiddenTextSessionAdapter.AddRegion(this);
                        }
                        finally
                        {
                            HiddenTextSessionAdapter.EndBatch();
                        }
                    }
                    else
                        TrackingSpan = trackingSpan;
                    return true;
                }
            }
            else
                TextSpan = textSpan;
            return false;
        }

        internal bool EnsureTrackingSpan()
        {
            if (TrackingSpan == null)
                TrackingSpan = TrackingSpanForTextSpan(TextSpan);
            return TrackingSpan != null;
        }

        internal ITrackingSpan TrackingSpanForTextSpan(TextSpan textSpan)
        {
            var buffer = HiddenTextSessionAdapter.Buffer;
            var currentSnapshot = buffer?.CurrentSnapshot;
            return currentSnapshot?.CreateTrackingSpan(new SnapshotSpan(currentSnapshot, Span.FromBounds(currentSnapshot.GetLineFromLineNumber(textSpan.iStartLine).Start + textSpan.iStartIndex, currentSnapshot.GetLineFromLineNumber(textSpan.iEndLine).Start + textSpan.iEndIndex)), SpanTrackingMode.EdgeExclusive);
        }

        internal string TipText
        {
            get
            {
                if (_tipText == null)
                {
                    var pbstrText = new string[1];
                    var hiddenTextClient = HiddenTextSessionAdapter != null ? HiddenTextSessionAdapter.HiddenTextClient : (IHiddenTextClient)null;
                    if (hiddenTextClient != null && hiddenTextClient.GetTipText(this, pbstrText) == 0)
                        _tipText = pbstrText[0];
                }
                return _tipText;
            }
        }
    }
}