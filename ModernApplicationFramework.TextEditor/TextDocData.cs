using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using ModernApplicationFramework.TextEditor.Implementation;

namespace ModernApplicationFramework.TextEditor
{
    //TODO: Implement as required
    //TODO: Add undo stuff
    internal abstract class TextDocData : INormalizeNewLines, ICommandTarget, IGetManagedObject, IMafUserData, IMafTextLines
    {
        internal event EventHandler TextBufferInitialized;
        public event LoadCompletedEventHandler OnLoadCompleted;
        public event FileChangedEventHandler OnFileChanged;

        public event ReplaceEventHandler OnReplace;
        public event ChangeStreamTextEventHandler OnChangeStreamText;

        public delegate void ChangeStreamTextEventHandler(int iPos, int iOldLen, int iNewLen, int fLast);
        public delegate void ChangeLineTextEventHandler(TextLineChange[] pTextLineChange, int fLast);


        private IContentType _initialContentType;
        protected MarkerManager MarkerManagerProtected;

        //internal ITextBuffer DocumentTextBuffer;
        //internal ITextBuffer DataTextBuffer;
        internal bool InitializedDocumentTextBuffer;

        internal ITextBuffer _documentTextBuffer;
        internal ITextBuffer _dataTextBuffer;
        internal IEditorOptions _editorOptions;
        internal string _defaultDocumentName = "Temp";
        private ITextDocument _textDocument;
        internal uint _textBufferState;
        private IReadOnlyRegion _userReadOnly;

        private ITextDataModel _textDataModel;

        private bool _eventsInitialized;
        private Dispatcher _dispatcher;
        private uint? _rdtCookie;
        private readonly object _textBufferStateSyncLock = new object();
        private ITextSnapshot _currentSnapshot;

        internal bool LoadingFile;
        internal bool _needToCheckScci;
        private Action _preReloadAction;

        internal byte[] _cachedMD5Hash;
        internal byte[] _cachedSHA1Hash;
        internal byte[] _cachedSHA256Hash;

        public MarkerManager MarkerManager => MarkerManagerProtected;

        public int NumberOfViewsWithPrimaryDocumentRole { get; private set; }

        public bool TextBufferContentInitialized => InitializedDocumentTextBuffer;

        public bool IsClosed { get; set; }

        public uint TextBufferState
        {
            get
            {
                if (_textDocument != null)
                    return _textBufferState | (_textDocument.IsDirty ? 4U : 0U);
                return _textBufferState;
            }
            set
            {
                var textBufferState1 = TextBufferState;
                if ((int)value == (int)textBufferState1)
                    return;
                var textBufferState2 = _textBufferState;
                if (_textDocument != null)
                {
                    var isDirty = (value & 4U) > 0U;
                    if (isDirty != _textDocument.IsDirty)
                        _textDocument.UpdateDirtyState(isDirty, DateTime.UtcNow);
                }
                _textBufferState = value & 4294967291U;
                if ((int)_textBufferState != (int)textBufferState2 && _rdtCookie.HasValue)
                {

                }
                RaiseEventForTextBufferStateChange(textBufferState1, value);
            }
        }

        public ITextDataModel TextDataModel
        {
            get
            {
                if (_textDataModel == null || _textDataModel.DataBuffer != _dataTextBuffer || _textDataModel.DocumentBuffer != _documentTextBuffer)
                    _textDataModel = new TextDataModel(_documentTextBuffer, _dataTextBuffer);
                return _textDataModel;
            }
        }


        public int NormalizeNewlines(uint e)
        {
            return 0;
        }

        public int IsBufferNormalized()
        {
            return 0;
        }

        internal static bool IsCommandSupported(ref Guid commandGroup, uint commandId)
        {
            return false;
        }

        protected internal virtual void SetSite()
        {
            _documentTextBuffer =
                EditorParts.TextBufferFactoryService.CreateTextBuffer(EditorParts.TextBufferFactoryService
                    .InertContentType);

            _dataTextBuffer = _documentTextBuffer;
            var optionsFactoryService = EditorParts.EditorOptionsFactoryService;
            _editorOptions = optionsFactoryService.CreateOptions();
            FixOptionsParent(optionsFactoryService.GetOptions(_dataTextBuffer), _editorOptions);
            _documentTextBuffer.TakeThreadOwnership();
            _dispatcher = Dispatcher.CurrentDispatcher;
            _textDocument =
                EditorParts.TextDocumentFactoryService.CreateTextDocument(_documentTextBuffer,
                    _defaultDocumentName + ".txt");


            if (InitializedDocumentTextBuffer)
                OnTextBufferInitialized(null, null);
            else
                TextBufferInitialized += OnTextBufferInitialized;
        }

        public void UpdateNumberOfViewsWithPrimaryDocumentRole(ITextViewRoleSet roles, bool addingView)
        {
            if (roles != null && !roles.Contains("PRIMARYDOCUMENT"))
                return;
            NumberOfViewsWithPrimaryDocumentRole += addingView ? 1 : -1;
        }

        public void SetInitialContentType(IContentType contentType)
        {
            if (contentType == null)
                return;
            _initialContentType = contentType;
        }

        internal void InitializeDocumentTextBuffer()
        {
            if (InitializedDocumentTextBuffer)
                return;
            if (_documentTextBuffer.ContentType == EditorParts.TextBufferFactoryService.InertContentType)
                _documentTextBuffer.ChangeContentType(_initialContentType ?? EditorParts.TextBufferFactoryService.TextContentType, null);
            //_documentTextBuffer.Changed += MarkDocumentBufferChanged;
            //_documentTextBuffer.ContentTypeChanged += _backupBroker.OnContentTypeChanged;
            _documentTextBuffer.TakeThreadOwnership();
            _dispatcher = Dispatcher.CurrentDispatcher;
            //_documentTextBuffer.Properties.AddProperty(typeof(IVsPersistDocData), this);
            InitializedDocumentTextBuffer = true;
            TextBufferInitialized?.Invoke(this, null);
        }

        protected void OnTextBufferInitialized(object sender, EventArgs e)
        {
            if (MarkerManagerProtected != null)
            {
                MarkerManagerProtected.BufferClosed();
                MarkerManagerProtected = null;
            }

            //MarkerManager.Create(DocumentTextBuffer);
            CleanUpEvents();
            InitializeEvents();
            _documentTextBuffer.Properties.AddProperty(typeof(IMafTextBuffer), this);
            //this.InitializeUndoHistory(EditorParts.TextUndoHistoryRegistry.RegisterHistory(DataTextBuffer));
        }

        protected void CleanUpEvents()
        {
            if (!_eventsInitialized)
                return;
            _documentTextBuffer.Changing -= OnTextBufferChanging;
            _dataTextBuffer.ChangedHighPriority -= OnTextBufferChangedHighPriority;
            _dataTextBuffer.Changed -= OnTextBufferChanged;
            _eventsInitialized = false;
        }

        protected void InitializeEvents()
        {
            if (_eventsInitialized)
                return;
            _dataTextBuffer.Changed += OnTextBufferChanged;
            _dataTextBuffer.ChangedHighPriority += OnTextBufferChangedHighPriority;
            _documentTextBuffer.Changing += OnTextBufferChanging;
            _eventsInitialized = true;
        }


        public ITextBuffer DataTextBuffer
        {
            get
            {
                if (InitializedDocumentTextBuffer)
                    return _dataTextBuffer;
                return null;
            }
            set
            {
                if (!InitializedDocumentTextBuffer)
                    throw new InvalidOperationException("Can't set data buffer before setting document buffer");
                if (value == _documentTextBuffer)
                    return;
                _dataTextBuffer = value;
                var optionsFactoryService = EditorParts.EditorOptionsFactoryService;
                _editorOptions = optionsFactoryService.CreateOptions();
                FixOptionsParent(optionsFactoryService.GetOptions(_dataTextBuffer), _editorOptions);
                _dataTextBuffer.Properties.AddProperty(typeof(IMafTextBuffer), this);
                _dataTextBuffer.Properties.AddProperty("IdentityMapping", _documentTextBuffer);
                //TODO: _dataTextBuffer.Properties[typeof(ITextBufferUndoManager)] = EditorParts.TextBufferUndoManagerProvider.GetTextBufferUndoManager(_documentTextBuffer);
            }
        }

        public ITextBuffer DocumentTextBuffer
        {
            get
            {
                if (InitializedDocumentTextBuffer)
                    return _documentTextBuffer;
                return null;
            }
        }

        private bool _inEvent;

        private void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            if (e.Changes.Count == 0)
                return;
            var length1 = DataTextBuffer.CurrentSnapshot.Length;
            var length2 = DocumentTextBuffer.CurrentSnapshot.Length;
            if (length1 != length2)
            {

            }
            _cachedMD5Hash = _cachedSHA1Hash = _cachedSHA256Hash = null;
            //TODO:
            //if (!this._enableUndoOnClosedFile && this._bufferIsDiskFile && (!string.IsNullOrEmpty(this._bufferMoniker) && this.GlobalUndoCoordinator.InsideOrExecutingGlobalClosedFileTransaction() == 0))
            //    this._enableUndoOnClosedFile = true;
            //Common.Verify(((IVsGlobalUndoCapableUndoManager)this._undoManager).OnTextBufferChanged(this._bufferIsDiskFile ? 1 : 0));
            if (LoadingFile)
                return;
            var fLast = 1;
            var beforeSpan = new SnapshotSpan(e.Before, Span.FromBounds(e.Changes[0].OldPosition, e.Changes[e.Changes.Count - 1].OldEnd));
            var snapshotSpan = new SnapshotSpan(e.After, Span.FromBounds(e.Changes[0].NewPosition, e.Changes[e.Changes.Count - 1].NewEnd));
            _currentSnapshot = e.After;
            _inEvent = true;
            ExtensibilityFireLineCommitEvent(0U, snapshotSpan);
            var changeStreamText = OnChangeStreamText;
            changeStreamText?.Invoke(beforeSpan.Start, beforeSpan.Length, snapshotSpan.Length, fLast);
            _inEvent = false;
            _currentSnapshot = null;
        }

        private void ExtensibilityFireLineCommitEvent(uint dwGestureFlags, SnapshotSpan textSpan)
        {
            //TODO: Text Manager stuff
        }

        private void OnTextBufferChanging(object sender, TextContentChangingEventArgs args)
        {
            if (args == null || args.Canceled || _textDocument == null || _textDocument.IsReloading)
                return;
            _preReloadAction = args.Cancel;
            var hr = TryCheckOut();
            _preReloadAction = null;
            if (!(hr < 0) || hr != -2147217399 && hr != -2147217403 && (hr != -2147217407 && hr != -2147217406))
                return;
            args.Cancel();
        }

        private void OnTextBufferChangedHighPriority(object sender, TextContentChangedEventArgs e)
        {
            if (LoadingFile || _textDocument != null && _textDocument.IsReloading || e.Changes.Count <= 0)
                return;
            if (OnReplace == null)
                return;
            _currentSnapshot = e.Before;
            var span = new SnapshotSpan(e.Before, Span.FromBounds(e.Changes[0].OldPosition, e.Changes[e.Changes.Count - 1].OldEnd));
            var snapshotSpan = new SnapshotSpan(e.After, Span.FromBounds(e.Changes[0].NewPosition, e.Changes[e.Changes.Count - 1].NewEnd));
            var pci = new ChangeInput[1];
            pci[0].MDelSpan = TextConvert.ToVsTextSpan(span);
            pci[0].MDwFlags = 0;
            pci[0].MPszNewText = Marshal.StringToCoTaskMemUni(snapshotSpan.GetText());
            pci[0].MiOldLen = span.Length;
            pci[0].MiNewLen = snapshotSpan.Length;
            try
            {
                OnReplace.Invoke(pci);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pci[0].MPszNewText);
                _currentSnapshot = null;
            }
        }

        internal int TryCheckOut()
        {
            //TODO : TextManager stuff
            if (!_needToCheckScci && (TextBufferState & 2U) <= 0U || _textDocument.IsDirty || true)
                return 0;

            return 0;
        }

        private static void FixOptionsParent(IEditorOptions optionsToFix, IEditorOptions optionsToSet)
        {
            if (optionsToFix.Parent == optionsToSet)
                return;
            for (; optionsToFix.Parent.Parent != null; optionsToFix = optionsToFix.Parent)
            {
                if (optionsToFix.Parent.Parent == optionsToSet)
                    return;
            }
            optionsToFix.Parent = optionsToSet;
        }

        public static TextDocData GetDocDataFromMafTextBuffer(object bufferAdapter)
        {
            TextDocData textDocData = null;
            if (bufferAdapter is IGetManagedObject getter)
            {
                var managedObject = getter.GetManagedObject();
                textDocData = managedObject.Target as TextDocData;
                managedObject.Free();
            }

            return textDocData;
        }

        public GCHandle GetManagedObject()
        {
            return GCHandle.Alloc(this);
        }

        public static IMafTextBuffer GetBufferAdapter(ITextBuffer textBuffer)
        {
            if (!textBuffer.Properties.TryGetProperty(typeof(IMafTextBuffer), out IMafTextBuffer property))
                return null;
            return property;
        }

        public int GetData(MafUserDataFormat format, out object pvtData)
        {
            throw new NotImplementedException();
        }

        public int SetData(MafUserDataFormat format, object vtData)
        {
            throw new NotImplementedException();
        }

        public delegate int LoadCompletedEventHandler();

        public int InitializeContent(string text, int length)
        {
            if (text == null)
                text = string.Empty;
            if (length < 0 || length > text.Length)
                return int.MinValue;
            if (_documentTextBuffer != _dataTextBuffer)
                return int.MinValue;
            if (!_documentTextBuffer.CheckEditAccess())
                return int.MinValue;
            var flag = false;
            if (((int) TextBufferState & 1) != 0)
            {
                SetUserDefinedReadOnly(false);
                flag = true;
            }

            using (var edit = _documentTextBuffer.CreateEdit())
            {
                if (edit.Snapshot.Length > 0 || !edit.Replace(new Span(0, edit.Snapshot.Length), text.Substring(0, length)))
                    return int.MinValue;
                edit.Apply();
            }

            if (flag)
                SetUserDefinedReadOnly(true);
            InitializeDocumentTextBuffer();
            SetDocDataDirty(0);
            //Common.ClearUndoManager(_undoManager);
            return 0;
        }

        private void RaiseEventForTextBufferStateChange(uint oldState, uint newState)
        {
            var onFileChanged = OnFileChanged;
            if (onFileChanged == null)
                return;
            if (((int)oldState & 1) != ((int)newState & 1))
                onFileChanged(1U, ((int)newState & 1) == 0 ? 0U : 1U);
            if (((int)oldState & 2) == ((int)newState & 2))
                return;
            onFileChanged(1U, ((int)newState & 2) == 0 ? 0U : 1U);
        }

        public int SetDocDataDirty(int fDirty)
        {
            lock (_textBufferStateSyncLock)
            {
                if (fDirty == 0)
                    TextBufferState &= 4294967291U;
                else
                    TextBufferState |= 4U;
            }
            return 0;
        }

        internal void SetUserDefinedReadOnly(bool isReadOnly)
        {
            lock (_textBufferStateSyncLock)
            {
                var action = (Action)(() =>
                {
                    if (isReadOnly)
                    {
                        if (((int)TextBufferState & 1) == 0)
                        {
                            using (var readOnlyRegionEdit = _documentTextBuffer.CreateReadOnlyRegionEdit())
                            {
                                _userReadOnly = readOnlyRegionEdit.CreateReadOnlyRegion(new Span(0, readOnlyRegionEdit.Snapshot.Length), SpanTrackingMode.EdgeInclusive, EdgeInsertionMode.Deny);
                                readOnlyRegionEdit.Apply();
                            }
                        }
                        TextBufferState |= 1U;
                    }
                    else
                    {
                        if (((int)TextBufferState & 1) != 0)
                        {
                            using (var readOnlyRegionEdit = _documentTextBuffer.CreateReadOnlyRegionEdit())
                            {
                                readOnlyRegionEdit.RemoveReadOnlyRegion(_userReadOnly);
                                _userReadOnly = null;
                                readOnlyRegionEdit.Apply();
                            }
                        }
                        TextBufferState &= 4294967294U;
                    }
                });
                if (_documentTextBuffer.CheckEditAccess())
                    action();
                else
                    _dispatcher.Invoke(DispatcherPriority.Normal, action);
            }
        }

        public delegate void FileChangedEventHandler(uint grfChange, uint dwFileAttrs);

        internal int EndTemplateEditing()
        {
            var num = 0;



            return num;
        }

        public int QueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            return 0;
        }

        public int Exec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            return 0;
        }

        internal ITextSnapshot GetCurrentSnapshot()
        {
            if (_dataTextBuffer == null)
                throw new InvalidOperationException();
            if (_currentSnapshot == null)
                return _dataTextBuffer.CurrentSnapshot;
            return _currentSnapshot;
        }

        public delegate int ReplaceEventHandler(ChangeInput[] pCI);
    }

    public struct ChangeInput
    {
        public TextSpan MDelSpan;
        public int MiOldLen;
        public int MiNewLen;
        public IntPtr MPszNewText;
        public uint MDwFlags;
    }
}