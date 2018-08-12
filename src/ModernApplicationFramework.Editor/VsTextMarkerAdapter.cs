using System;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Classification;

namespace ModernApplicationFramework.Editor
{
    internal class VsTextMarkerAdapter : IVsTextMarker, IVsTextLineMarker, IVsTextStreamMarker, IDisposable
    {
        private readonly MarkerManager _manager;
        private uint _clientData;
        private bool _notifyClientOnChanges;
        private IVsTextMarkerClientEx _clientEx;
        private IReadOnlyRegion _readOnlyRegion;
        private const int ReadOnlyMarkerType = 1;

        internal IVsTextMarkerClient Client { get; private set; }

        private IVsTextMarkerClientAdvanced ClientAdvanced { get; set; }

        internal bool NotifyClientOnChanges
        {
            get
            {
                if (Client != null)
                    return _notifyClientOnChanges;
                return false;
            }
            set => _notifyClientOnChanges = value;
        }

        internal bool NotifyClientAdvancedOnChanges => ClientAdvanced != null;

        private IMafTextBuffer Adapter => _manager.Buffer.Properties.GetProperty<IMafTextBuffer>(typeof(IMafTextBuffer));

        internal int MarkerTypeCode { get; private set; }

        //public VsTextMarkerAdapter(MarkerManager manager, int type, IVsTextMarkerClient markerClient, VirtualSnapshotSpan virtualSpan)
        //{
        //    _manager = manager;
        //    Client = markerClient;
        //    NotifyClientOnChanges = Client != null;
        //    ClientAdvanced = Client as IVsTextMarkerClientAdvanced;
        //    _clientEx = Client as IVsTextMarkerClientEx;
        //    Type = type;
        //    IsValid = true;
        //    Tag = new VsTextMarkerTag(this, virtualSpan);
        //    AdjustReadOnlyRegion();
        //}

        public ITextBuffer Buffer => _manager.Buffer;

        public uint Behavior { get; private set; }

        public VsTextMarkerTag Tag { get; }

        public bool IsDisposed { get; private set; }

        public bool IsValid { get; }

        public bool IsVisible
        {
            get
            {
                if (IsValid && MarkerTypeCode > 0)
                    return ((int)VisualStyle & 128) == 0;
                return false;
            }
        }

        public bool IsExclusive(ViewMarkerTypeManager manager)
        {
            if (IsVisibleInView(manager))
                return ShowColorForMarker(manager);
            return false;
        }

        private void AdjustReadOnlyRegion()
        {
            AdjustReadOnlyRegion(Type);
        }

        private void AdjustReadOnlyRegion(int markerType)
        {
            if (!IsValid || _readOnlyRegion == null && markerType != 1)
                return;
            var snapshotSpan = Tag.MarkerSpan.SnapshotSpan;
            if (_readOnlyRegion != null && markerType == 1 && _readOnlyRegion.Span.GetSpan(snapshotSpan.Snapshot) == snapshotSpan)
                return;
            using (var readOnlyRegionEdit = Buffer.CreateReadOnlyRegionEdit())
            {
                if (_readOnlyRegion != null)
                {
                    readOnlyRegionEdit.RemoveReadOnlyRegion(_readOnlyRegion);
                    _readOnlyRegion = null;
                }
                if (markerType == 1)
                    _readOnlyRegion = readOnlyRegionEdit.CreateDynamicReadOnlyRegion(snapshotSpan, SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Allow, RespondToReadOnlyCheck);
                readOnlyRegionEdit.Apply();
            }
        }

        private bool RespondToReadOnlyCheck(bool isEdit)
        {
            if (isEdit)
            {
                string pszText = null;
                if (Client != null)
                {
                    var pbstrText = new string[1] { "" };
                    //if (ErrorHandler.Succeeded(Client.GetTipText(this, pbstrText)))
                    //    pszText = pbstrText[0];
                }
                //if (pszText == null)
                //    pszText = Strings.ReadOnlyRegionEditAttempt;
                //IVsUIShell service = Common.GetService<IVsUIShell, SVsUIShell>(_manager._pServiceProvider);
                //if (service != null)
                //{
                //    var empty = Guid.Empty;
                //    int pnResult;
                //    Common.Verify(service.ShowMessageBox(0U, ref empty, (string) null, pszText, (string) null, 0U,
                //        OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_INFO,
                //        0, out pnResult));
                //}
            }
            return true;
        }

        private bool IsVisibleInView(ViewMarkerTypeManager manager)
        {
            if (!IsVisible)
                return false;
            if (manager == null)
                return true;
            return manager.IsMarkerVisibleInView(this);
        }

        private bool ShowColorForMarker(ViewMarkerTypeManager manager)
        {
            if (((int)VisualStyle & 4) == 0)
                return (VisualStyle & 2U) > 0U;
            if (manager == null)
                return true;
            return !manager.VisibleGlyphMargin;
        }

        public IClassificationType TextClassificationType(ViewMarkerTypeManager manager)
        {
            if (((int)Behavior & 32) == 0 && IsVisibleInView(manager) && ShowColorForMarker(manager))
                return MarkerType.TextClassificationType;
            return null;
        }

        public string SquiggleName
        {
            get
            {
                if (IsVisible)
                    return MarkerType.SquiggleName;
                return null;
            }
        }

        public int Priority { get; private set; }

        public int Type
        {
            get => MarkerTypeCode;
            //private set
            //{
            //    var markerType = MarkerType;
            //    MarkerTypeCode = value;
            //    MarkerType = _manager.GetMarkerType(MarkerTypeCode);
            //    Priority = MarkerType.Priority;
            //    Behavior = MarkerType.Behavior;
            //    VisualStyle = MarkerType.VisualStyle;
            //    if (markerType == null)
            //        return;
            //    _manager._markerStore.MarkerTypeChanged(this, markerType, MarkerType, markerType.VisualStyle, MarkerType.VisualStyle);
            //}
        }

        public MarkerType MarkerType { get; private set; }

        //public string DisplayName
        //{
        //    get
        //    {
        //        MarkerType = _manager.GetMarkerType(MarkerTypeCode);
        //        return MarkerType.DisplayName;
        //    }
        //}

        public uint VisualStyle { get; private set; }

        public Linestyle LineStyle => MarkerType.LineStyle;

        public SpanTrackingMode SpanTrackingMode
        {
            get
            {
                var num = (Behavior & 2U) > 0U ? 1 : 0;
                var flag = (Behavior & 4U) > 0U;
                if (num == 0)
                    return !flag ? SpanTrackingMode.EdgeExclusive : SpanTrackingMode.EdgePositive;
                return !flag ? SpanTrackingMode.EdgeNegative : SpanTrackingMode.EdgeInclusive;
            }
        }

        public void OnBufferClose()
        {
            if (Client == null)
                return;
            _manager.CallIntoExternalCode(InternalOnBufferClose);
        }

        public void OnMarkerTextChanged()
        {
            if (ClientAdvanced == null)
                return;
            _manager.CallIntoExternalCode(InternalOnMarkerTextChanged);
        }

        public void OnAfterMarkerChange()
        {
            if (Client == null)
                return;
            _manager.CallIntoExternalCode(InternalOnAfterMarkerChange);
        }

        public void OnMarkerSpanDeleted()
        {
            _manager.RemoveMarker(this, true);
            Dispose();
        }

        private void InternalOnBufferClose()
        {
            IVsTextMarkerClientPrivate client = Client as IVsTextMarkerClientPrivate;
            if (client != null)
                client.OnBeforeBufferClose();
            else if (Client != null)
                Client.OnBeforeBufferClose();
            UnadviseClient();
        }

        private void InternalOnAfterMarkerChange()
        {
            if (Client == null || Client.OnAfterMarkerChange(this) != -2147467263)
                return;
            NotifyClientOnChanges = false;
        }

        private void InternalOnMarkerTextChanged()
        {
            ClientAdvanced?.OnMarkerTextChanged(this);
        }

        public int DrawGlyph(IntPtr hdc, NativeMethods.NativeMethods.RECT[] pRect)
        {
            return MarkerType.VsMarkerType.DrawGlyph(hdc, pRect);
        }

        public int ExecMarkerCommand(int iItem)
        {
            if (Client != null)
                return Client.ExecMarkerCommand(this, iItem);
            return -2147467259;
        }

        public int GetBehavior(out uint pdwBehavior)
        {
            pdwBehavior = Behavior;
            return 0;
        }

        public int GetMarkerCommandInfo(int iItem, string[] pbstrText, uint[] pcmdf)
        {
            if (Client != null)
                return Client.GetMarkerCommandInfo(this, iItem, pbstrText, pcmdf);
            return -2147467259;
        }

        public int GetPriorityIndex(out int piPriorityIndex)
        {
            piPriorityIndex = Priority;
            return 0;
        }

        public int GetTipText(string[] pbstrText)
        {
            if (Client != null)
                return Client.GetTipText(this, pbstrText);
            return -2147467259;
        }

        public int GetType(out int piMarkerType)
        {
            piMarkerType = Type;
            return 0;
        }

        public int GetVisualStyle(out uint pdwFlags)
        {
            pdwFlags = VisualStyle;
            return 0;
        }

        public int Invalidate()
        {
            if (IsDisposed)
                return 1;
            Dispose();
            return 0;
        }

        public int SetBehavior(uint dwBehavior)
        {
            if (IsDisposed)
                return -2147217401;
            if (IsValid)
            {
                var markerSpan1 = Tag.MarkerSpan;
                var markerChangeMask = _manager.GetMarkerChangeMask(this);
                Behavior = dwBehavior;
                Tag.UpdateMarkerSpan();
                AdjustReadOnlyRegion();
                var markerSpan2 = Tag.MarkerSpan;
                var manager = _manager;
                var tagTextBuffer = Tag.TagTextBuffer;
                var snapshot = markerSpan2.Snapshot;
                var virtualSnapshotPoint = markerSpan1.Start;
                int position1 = virtualSnapshotPoint.Position;
                virtualSnapshotPoint = markerSpan2.Start;
                int position2 = virtualSnapshotPoint.Position;
                var start = Math.Min(position1, position2);
                virtualSnapshotPoint = markerSpan1.End;
                int position3 = virtualSnapshotPoint.Position;
                virtualSnapshotPoint = markerSpan2.End;
                int position4 = virtualSnapshotPoint.Position;
                var end = Math.Max(position3, position4);
                var span1 = Span.FromBounds(start, end);
                var span2 = new SnapshotSpan(snapshot, span1);
                var num = (int)(markerChangeMask | _manager.GetMarkerChangeMask(this));
                manager.RaiseChangedEvents(tagTextBuffer, span2, (MarkerManager.MarkerChangedMask)num);
            }
            else
                Behavior = dwBehavior;
            return 0;
        }

        //public int SetType(int iMarkerType)
        //{
        //    if (IsDisposed)
        //        return -2147217401;
        //    if (IsValid)
        //    {
        //        var markerSpan1 = Tag.MarkerSpan;
        //        var markerChangeMask = _manager.GetMarkerChangeMask(this);
        //        Tag.UpdateMarkerSpan();
        //        if (iMarkerType == 1)
        //        {
        //            Type = iMarkerType;
        //            AdjustReadOnlyRegion();
        //        }
        //        else
        //        {
        //            AdjustReadOnlyRegion(iMarkerType);
        //            Type = iMarkerType;
        //        }
        //        var markerSpan2 = Tag.MarkerSpan;
        //        var manager = _manager;
        //        var tagTextBuffer = Tag.TagTextBuffer;
        //        var snapshot = markerSpan2.Snapshot;
        //        var start = Math.Min(markerSpan1.Start.Position, markerSpan2.Start.Position);
        //        var end1 = markerSpan1.End;
        //        int position1 = end1.Position;
        //        end1 = markerSpan2.End;
        //        int position2 = end1.Position;
        //        var end2 = Math.Max(position1, position2);
        //        var span1 = Span.FromBounds(start, end2);
        //        var span2 = new SnapshotSpan(snapshot, span1);
        //        var num = (int)(markerChangeMask | _manager.GetMarkerChangeMask(this));
        //        manager.RaiseChangedEvents(tagTextBuffer, span2, (MarkerManager.MarkerChangedMask)num);
        //    }
        //    else
        //        Type = iMarkerType;
        //    return 0;
        //}

        //public int SetVisualStyle(uint dwFlags)
        //{
        //    if (IsDisposed)
        //        return -2147217401;
        //    if ((int)_visualStyle == (int)dwFlags)
        //        return 0;
        //    var markerChangeMask = _manager.GetMarkerChangeMask(this);
        //    var visualStyle = _visualStyle;
        //    _visualStyle = dwFlags;
        //    _manager._markerStore.MarkerTypeChanged(this, _markerType, _markerType, visualStyle, _visualStyle);
        //    _manager.RaiseChangedEvents(Tag.TagTextBuffer, Tag.MarkerSpan.SnapshotSpan, markerChangeMask | _manager.GetMarkerChangeMask(this));
        //    return 0;
        //}

        public int UnadviseClient()
        {
            if (Client == null)
                return 1;
            Client = null;
            ClientAdvanced = null;
            _clientEx = null;
            return 0;
        }

        //public int GetLineBuffer(out IVsTextLines ppBuffer)
        //{
        //    ppBuffer = (IVsTextLines)Adapter;
        //    return 0;
        //}

        //public int GetCurrentSpan(TextSpan[] pSpan)
        //{
        //    if (!IsValid)
        //        return -2147467259;
        //    var markerSpan = Tag.MarkerSpan;
        //    pSpan[0] = TextConvert.ToVsTextSpan(markerSpan);
        //    return IsDisposed ? 1 : 0;
        //}

        //public int ResetSpan(int iStartLine, int iStartIndex, int iEndLine, int iEndIndex)
        //{
        //    if (IsDisposed)
        //        return -2147217401;
        //    if (!IsValid)
        //        return -2147467259;
        //    var span = new TextSpan
        //    {
        //        iStartLine = iStartLine,
        //        iStartIndex = iStartIndex,
        //        iEndLine = iEndLine,
        //        iEndIndex = iEndIndex
        //    };
        //    var pSpan = new TextSpan[1];
        //    GetCurrentSpan(pSpan);
        //    if (pSpan[0].iStartLine == span.iStartLine && pSpan[0].iStartIndex == span.iStartIndex && (pSpan[0].iEndLine == span.iEndLine && pSpan[0].iEndIndex == span.iEndIndex))
        //        return 0;
        //    var markerSpan = Tag.MarkerSpan;
        //    if (!TextConvert.TryToVirtualSnapshotSpan(markerSpan.Snapshot, span, out var virtualSnapshotSpan))
        //        return -2147024809;
        //    markerSpan = Tag.MarkerSpan;
        //    var snapshotSpan1 = markerSpan.SnapshotSpan;
        //    Tag.UpdateMarkerSpan(virtualSnapshotSpan);
        //    AdjustReadOnlyRegion();
        //    markerSpan = Tag.MarkerSpan;
        //    var snapshotSpan2 = markerSpan.SnapshotSpan;
        //    _manager.RaiseChangedEvents(Tag.TagTextBuffer, new SnapshotSpan(snapshotSpan2.Snapshot, Span.FromBounds(Math.Min(snapshotSpan1.Start, snapshotSpan2.Start), Math.Max(snapshotSpan1.End, snapshotSpan2.End))), _manager.GetMarkerChangeMask(this));
        //    return 0;
        //}

        //public int GetClientData(out uint pdwData)
        //{
        //    pdwData = _clientData;
        //    return 0;
        //}

        //public int SetClientData(uint dwData)
        //{
        //    _clientData = dwData;
        //    return 0;
        //}

        //public int GetStreamBuffer(out IVsTextStream ppBuffer)
        //{
        //    ppBuffer = (IVsTextStream)Adapter;
        //    return 0;
        //}

        //public int GetCurrentSpan(out int piPos, out int piLen)
        //{
        //    InteropHelper.SetOutParameter(out piPos, 0);
        //    InteropHelper.SetOutParameter(out piLen, 0);
        //    if (IsDisposed || !IsValid)
        //        return !IsDisposed ? -2147467259 : -2147217401;
        //    var snapshotSpan = Tag.MarkerSpan.SnapshotSpan;
        //    InteropHelper.SetOutParameter(out piPos, (int)snapshotSpan.Start);
        //    InteropHelper.SetOutParameter(out piLen, snapshotSpan.Length);
        //    return 0;
        //}

        //public int ResetSpan(int iNewPos, int iNewLen)
        //{
        //    if (IsDisposed)
        //        return -2147217401;
        //    if (!IsValid)
        //        return -2147467259;
        //    ITextSnapshot snapshot = Tag.MarkerSpan.Snapshot;
        //    if (iNewLen < 0 || !TextConvert.TryGetLineColFromPosition(snapshot, iNewPos, out int line1, out int col1) || !TextConvert.TryGetLineColFromPosition(snapshot, iNewPos + iNewLen, out int line2, out int col2))
        //        return -2147024809;
        //    return ResetSpan(line1, col1, line2, col2);
        //}

        public void Dispose()
        {
            if (IsDisposed)
                return;
            IsDisposed = true;
            if (_readOnlyRegion != null)
            {
                using (var readOnlyRegionEdit = Buffer.CreateReadOnlyRegionEdit())
                {
                    readOnlyRegionEdit.RemoveReadOnlyRegion(_readOnlyRegion);
                    readOnlyRegionEdit.Apply();
                }
                _readOnlyRegion = null;
            }
            var disposed = Disposed;
            disposed?.Invoke(this, EventArgs.Empty);
            _manager.RemoveMarker(this);
            if (Client != null)
            {
                var client = Client;
                var clientEx = _clientEx;
                var privClient = client as IVsTextMarkerClientPrivate;
                _manager.CallIntoExternalCode(() =>
                {
                    if (privClient != null)
                        privClient.MarkerInvalidated();
                    else
                        client.MarkerInvalidated();
                    clientEx?.MarkerInvalidated((IMafTextLines) Adapter, this);
                });
            }
            GC.SuppressFinalize(this);
        }

        public event EventHandler Disposed;

        //public int DrawGlyphEx(uint dwFlags, IntPtr hdc, NativeMethods.NativeMethods.RECT[] pRect, int iLineHeight)
        //{
        //    return _markerType.VsMarkerType.DrawGlyphEx(dwFlags, hdc, pRect, iLineHeight);
        //}

        //public int GetTextLayer(out IVsTextLayer ppLayer)
        //{
        //    ppLayer = (IVsTextLayer)TextDocData.GetDocDataFromVsTextBuffer((object)Adapter);
        //    return 0;
        //}

        //public int IsInvalidated()
        //{
        //    return IsDisposed ? 0 : 1;
        //}

        //public int QueryClientInterface(ref Guid iid, out IntPtr ppClient)
        //{
        //    if (Client == null)
        //    {
        //        ppClient = IntPtr.Zero;
        //        return -2147467262;
        //    }
        //    var iunknownForObject = Marshal.GetIUnknownForObject((object)Client);
        //    var num = Marshal.QueryInterface(iunknownForObject, ref iid, out ppClient);
        //    Marshal.Release(iunknownForObject);
        //    return num;
        //}

        //public int GetCurrentLineIndex(out int piLine, out int piIndex)
        //{
        //    if (!IsValid)
        //    {
        //        InteropHelper.SetOutParameter(out piLine, 0);
        //        InteropHelper.SetOutParameter(out piIndex, 0);
        //        return -2147467259;
        //    }
        //    var markerSpan = Tag.MarkerSpan;
        //    var lineFromPosition = markerSpan.Snapshot.GetLineFromPosition(markerSpan.Start.Position);
        //    InteropHelper.SetOutParameter(out piLine, lineFromPosition.LineNumber);
        //    InteropHelper.SetOutParameter(out piIndex, markerSpan.Start.Position.Position + markerSpan.Start.VirtualSpaces - lineFromPosition.Start.Position);
        //    return 0;
        //}

        //public int GetPopupContent(bool isMouseHover, out object uiElement)
        //{
        //    uiElement = null;
        //    IVsTextMarkerPopupClient client = Client as IVsTextMarkerPopupClient;
        //    if (client != null)
        //        return client.GetPopupContent(isMouseHover ? 1 : 0, out uiElement);
        //    return -2147467262;
        //}
    }

    public interface IVsTextMarkerClientAdvanced
    {
        int OnMarkerTextChanged(IVsTextMarker pMarker);
    }

    internal interface IVsTextMarkerClientPrivate
    {
        void MarkerInvalidated();

        void OnBeforeBufferClose();
    }
}