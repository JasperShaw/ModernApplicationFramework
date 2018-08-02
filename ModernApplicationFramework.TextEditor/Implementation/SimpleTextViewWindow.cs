using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    // TODO: Add forward backward stuff
    internal abstract class SimpleTextViewWindow : ICommandTarget, ITypedTextTarget,
        ICommandTargetInner, /*IBackForwardNavigation*/ IMafTextView, IMafUserData, IConnectionAdviseHelper
    {
        private static Exception LastCreatedButNotInitializedException = null;
        private static string LastCreatedButNotInitializedExceptionStackTrace = null;

        private static Exception LastInitializingException;
        private static string LastInitializingExceptionStackTrace;

        internal IEditorOptions _editorOptions;
        internal ITextViewHost _textViewHostPrivate;

        internal ITagAggregator<IUrlTag> _urlTagAggregator;

        private CommandChainNode _commandChain;

        protected ConnectionPointContainer connectionPointContainerHelper;
        private IClassificationFormatMap _classificationFormatMap;
        private bool _delayedTextBufferLoad;
        private EditorAndMenuFocusTracker _editorAndMenuFocusTracker;
        private bool _eventsInitialized;

        private uint _initFlags;
        private ITextViewRoleSet _initialRoles;
        private IScrollMap _scrollMap;

        internal ITextViewFilter _textViewFilter;

        private TextDocData _textDocData;
        private FrameworkElement _zoomControl;

        internal bool _canCaretAndSelectionMapToDataBuffer = true;

        //internal TextViewShimHost ShimHost { get; set; }


        public InitializationState CurrentInitializationState { get; internal set; }

        internal ITextSnapshot DataTextSnapshot => TextDocData.GetCurrentSnapshot();

        internal TextDocData TextDocData
        {
            get => _textDocData;
            set => _textDocData = value;
        }


        public bool InProvisionalInput { get; set; }

        public bool IsTextViewHostAccessible
        {
            get
            {
                if (CurrentInitializationState != InitializationState.TextBufferAvailable)
                    return CurrentInitializationState >= InitializationState.TextViewCreatedButNotInitialized;
                return true;
            }
        }

        internal bool CanCaretAndSelectionMapToDataBuffer
        {
            get => _canCaretAndSelectionMapToDataBuffer;
            private set
            {
                if (value == _canCaretAndSelectionMapToDataBuffer)
                    return;
                _canCaretAndSelectionMapToDataBuffer = value;
                InvalidateShellQueryStatusCache();
            }
        }

        internal bool RaiseGoBackEvents { get; set; }

        private void InvalidateShellQueryStatusCache()
        {
            throw new NotImplementedException();
        }

        public ITextView TextView => TextViewHost.TextView;

        public ITextViewHost TextViewHost
        {
            get
            {
                if (!IsTextViewHostAccessible)
                    throw new InvalidOperationException("Attempting to get the view from an adapter in state " +
                                                        CurrentInitializationState);
                if (CurrentInitializationState == InitializationState.TextBufferAvailable)
                    Init_OnActivation();
                return _textViewHostPrivate;
            }
        }

        protected SimpleTextViewWindow()
        {
            Init_Construct();
        }

        public enum InitializationState
        {
            Constructing,
            Closed,
            Constructed,
            Sited,
            AdapterInitialized,
            TextDocDataAvailable,
            TextBufferAvailable,
            TextViewInitializing,
            TextViewCreatedButNotInitialized,
            TextViewAvailable
        }

        public bool Advise(Type eventType, object eventSink)
        {
            throw new NotImplementedException();
        }

        public int Exec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            var hr1 = PreOuterQueryStatus(ref commandGroup, 1, new[]
            {
                new Olecmd {cmdf = 0, cmdID = commandId}
            });
            return 0;
        }

        private int PreOuterQueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds)
        {
            if (IsCommandExecutionProhibited())
                return int.MinValue;
            Init_OnActivation();
            if (TextViewHost.HostControl.IsKeyboardFocusWithin)
            {
                if (Keyboard.FocusedElement is DependencyObject focusedElement &&
                    CommandRouting.GetInterceptsCommandRouting(focusedElement))
                {

                }
            }
            return 0;
        }

        private bool IsCommandExecutionProhibited()
        {
            if (CurrentInitializationState == InitializationState.TextDocDataAvailable || CurrentInitializationState == InitializationState.TextBufferAvailable || CurrentInitializationState == InitializationState.TextViewAvailable)
                return TextDocData.IsClosed;
            return true;
        }

        public int GetData(MafUserDataFormat format, out object pvtData)
        {
            if (format == MafUserDataFormat.TextViewHost)
            {
                pvtData = CurrentInitializationState >= InitializationState.TextBufferAvailable ? TextViewHost : null;
                return 0;
            }

            pvtData = null;
            return int.MinValue;
        }

        public int InnerExec(ref Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            return 0;
        }

        public int InnerQueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            return 0;
        }

        public int QueryStatus(ref Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            return 0;
        }

        public int SetBuffer(IMafTextLines pBuffer)
        {
            return Init_SetBuffer(pBuffer);
        }

        public int SetData(MafUserDataFormat format, object vtData)
        {
            throw new NotImplementedException();
        }

        public void SetInitialRoles(ITextViewRoleSet roles)
        {
            _textDocData?.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, false);
            _initialRoles = roles;
            _textDocData?.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, true);
        }

        public void SetSite()
        {
            Init_SetSite();
        }

        public bool Unadvise(Type eventType, object eventSink)
        {
            throw new NotImplementedException();
        }

        protected virtual void InitializeConnectionPoints()
        {
            connectionPointContainerHelper = new ConnectionPointContainer(this);
            connectionPointContainerHelper.AddEventType<IMafTextViewEvents>();
        }

        private static void FailInitializationAndThrow(string message)
        {
            throw new InvalidOperationException(message);
        }

        private void CleanUpEvents()
        {
            //if (_eventsInitialized)
            //{
            //    var textView = _textViewHostPrivate.TextView;
            //    textView.Caret.PositionChanged -= Caret_PositionChanged;
            //    textView.Selection.SelectionChanged -= Selection_SelectionChanged;
            //    textView.LayoutChanged -= View_LayoutChanged;
            //    Keyboard.RemoveKeyDownHandler(textView.VisualElement, OnTextView_KeyDown);
            //    Keyboard.RemoveKeyUpHandler(textView.VisualElement, OnTextView_KeyUp);
            //    textView.ViewportLeftChanged -= View_ViewportLeftChanged;
            //    textView.ViewportWidthChanged -= View_ViewportWidthChanged;
            //    _editorOptions.OptionChanged -= EditorOptions_OptionChanged;
            //    _classificationFormatMap.ClassificationFormatMappingChanged -= ClassificationFormatMap_ClassificationFormatMappingChanged;
            //    textView.TextBuffer.Changed -= DocData_OnChangeLineText;
            //    //TODO: Add textDocData stuff
            //    _editorAndMenuFocusTracker.GotFocus -= OnEditorOrMenuGotFocus;
            //    _editorAndMenuFocusTracker.LostFocus -= OnEditorOrMenuLostFocus;
            //    _editorAndMenuFocusTracker = null;
            //    if (_zoomControl != null)
            //    {
            //        _zoomControl.IsKeyboardFocusWithinChanged -= OnZoomIsKeyboardFocusWithinChanged;
            //        _zoomControl = null;
            //    }
            //    EditorParts.EditorFormatMapService.GetEditorFormatMap(textView).FormatMappingChanged -= OnFormatMapChanged;
            //}
            _eventsInitialized = false;
            _scrollMap = null;
        }

        private void CloseBufferRelatedResources()
        {
            _textDocData.EndTemplateEditing();
            _textDocData.TextBufferInitialized -= Init_OnTextBufferInitialized;
            _textDocData.OnLoadCompleted -= Init_OnTextBufferLoaded;

            _textDocData.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, false);

            //TODO: Add Find related stuff
            CleanUpEvents();
            CloseExistingTextView();

            //TODO: Add undo stuff
        }

        private void CloseExistingTextView()
        {
            if (_urlTagAggregator != null)
            {
                _urlTagAggregator.Dispose();
                _urlTagAggregator = null;
            }

            if (_textViewHostPrivate != null)
            {
                var host = _textViewHostPrivate;
                var view = _textViewHostPrivate.TextView;
                if (!host.IsClosed)
                    host.Close();

                view.Properties.RemoveProperty(typeof(IMafTextView));
                view.Properties.RemoveProperty(typeof(ICommandTarget));
                //view.Properties.RemoveProperty(typeof(VerticalDetailUI));
                //TODO: Undo stuff
                _textViewHostPrivate = null;
                if (!view.IsClosed)
                    view.Close();
            }

            CurrentInitializationState = InitializationState.TextBufferAvailable;
        }

        private void Init_Construct()
        {
            //TODO: Shim stuff
            InitializeConnectionPoints();
            _commandChain = new CommandChainNode
            {
                Next = this
            };
            RaiseGoBackEvents = true;
            //TODO: Marker stuff
            CurrentInitializationState = InitializationState.Constructed;
        }

        private void Init_InitializeWpfTextView()
        {
            ITextView textView;
            ITextViewHost textViewHost;
            try
            {
                CurrentInitializationState = InitializationState.TextViewInitializing;
                if (_initialRoles == null)
                    _initialRoles = EditorParts.TextEditorFactoryService.DefaultRoles;
                SetViewOptions();

                //TODO: Add FontsAndColorsCategory stuff
                if (_textViewHostPrivate != null)
                    return;

                textView = EditorParts.TextEditorFactoryService.CreateTextViewWithoutInitialization(
                    _textDocData.TextDataModel, _initialRoles, _editorOptions);
                textViewHost =
                    EditorParts.TextEditorFactoryService.CreateTextViewHostWithoutInitialization(textView, false);
                _editorOptions = textView.Options;
                EmbeddedObjectHelper.SetOleCommandTarget(textViewHost.HostControl, this);
                //TODO: UserContext Stuff
                _textViewHostPrivate = textViewHost;
                textView.Properties.AddProperty(typeof(IMafTextView), this);
                textView.Properties.AddProperty(typeof(ICommandTarget), this);
            }
            catch (Exception ex)
            {
                LastInitializingException = ex;
                LastInitializingExceptionStackTrace = ex.StackTrace;
                throw;
            }

            try
            {
                CurrentInitializationState = InitializationState.TextViewCreatedButNotInitialized;
                EditorParts.TextEditorFactoryService.InitializeTextView(textView);
                EditorParts.TextEditorFactoryService.InitializeTextViewHost(textViewHost);
                var serviceFilter = new CommandHandlerServiceFilter(this);
                Marshal.ThrowExceptionForHR(AddCommandFilter(serviceFilter, out var target));
                serviceFilter.Initialize(target);
                CurrentInitializationState = InitializationState.TextViewAvailable;
            }
            catch (Exception ex)
            {
                LastCreatedButNotInitializedException = ex;
                LastCreatedButNotInitializedExceptionStackTrace = ex.StackTrace;
                throw;
            }
        }

        public int AddCommandFilter(ICommandTarget pNewCmdTarg, out ICommandTarget ppNextCmdTarg)
        {
            return AddCommandFilter(pNewCmdTarg, out ppNextCmdTarg, false);
        }

        internal void ClearCommandContext()
        {
            if (_textViewHostPrivate == null || _textViewHostPrivate.IsClosed)
                return;
            TextView.Properties.RemoveProperty(typeof(MafConstants.EditorCommands));
        }

        internal void SetCommandContext(MafConstants.EditorCommands command)
        {
            TextView.Properties[typeof(MafConstants.EditorCommands)] = command;
        }

        internal SnapshotPoint? DataPointFromViewPoint(SnapshotPoint viewPoint)
        {
            return TextView.BufferGraph.MapDownToSnapshot(viewPoint, PointTrackingMode.Positive, DataTextSnapshot, PositionAffinity.Successor);
        }

        

        private int AddCommandFilter(ICommandTarget pNewCmdTarg, out ICommandTarget ppNextCmdTarg,
            bool projectionAware)
        {
            if (_textViewFilter == null)
                _textViewFilter = pNewCmdTarg as ITextViewFilter;
            var node = new CommandChainNode
            {
                Next = _commandChain.Next,
                FilterObject = pNewCmdTarg,
                ContainingTextView = projectionAware ? null : this
            };
            _commandChain.Next = node;
            ppNextCmdTarg = node;
            

            //TODO: Notifications stuff

            return 0;
        }

        private void Init_OnActivation()
        {
            if (CurrentInitializationState >= InitializationState.TextViewInitializing)
            {
                if (CurrentInitializationState != InitializationState.TextViewAvailable && CurrentInitializationState !=
                    InitializationState.TextViewCreatedButNotInitialized)
                    throw new InvalidOperationException("Attempting to activate an adapter in state " +
                                                        CurrentInitializationState);
            }
            else
            {
                if (CurrentInitializationState == InitializationState.TextBufferAvailable)
                    try
                    {
                        Init_InitializeWpfTextView();
                    }
                    catch (Exception e)
                    {
                        EditorParts.GuardedOperations.HandleException(this, e);
                    }

                if (!_delayedTextBufferLoad)
                    return;
                Init_OnTextBufferLoaded();
            }
        }

        private void Init_OnTextBufferInitialized(object sender, EventArgs e)
        {
            if (CurrentInitializationState < InitializationState.AdapterInitialized)
                FailInitializationAndThrow("Received OnTextBufferInitialized before the view adapter was initialized.");

            if (CurrentInitializationState == InitializationState.TextViewAvailable)
            {
                CleanUpEvents();
                CloseExistingTextView();
            }

            CurrentInitializationState = InitializationState.TextBufferAvailable;

            Init_OnActivation();
        }

        private int Init_OnTextBufferLoaded()
        {
            if (CurrentInitializationState < InitializationState.TextViewAvailable)
            {
                _delayedTextBufferLoad = true;
                return 0;
            }

            _delayedTextBufferLoad = false;
            //TODO Textmanager stuff
            return 0;
        }

        private int Init_SetBuffer(IMafTextLines pBuffer)
        {
            if (CurrentInitializationState >= InitializationState.TextDocDataAvailable)
                CloseBufferRelatedResources();
            _textDocData = TextDocData.GetDocDataFromMafTextBuffer(pBuffer);
            if (_textDocData == null)
                throw new ArgumentException("Could not find adapter for the given buffer", nameof(pBuffer));
            _textDocData.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, true);
            CurrentInitializationState = InitializationState.TextDocDataAvailable;
            if (_textDocData.InitializedDocumentTextBuffer)
            {
                Init_OnTextBufferInitialized(null, null);
                Init_OnTextBufferLoaded();
            }

            _textDocData.TextBufferInitialized += Init_OnTextBufferInitialized;
            _textDocData.OnLoadCompleted += Init_OnTextBufferLoaded;


            return 0;
        }

        private void Init_SetSite()
        {
            if (CurrentInitializationState == InitializationState.Constructing)
                FailInitializationAndThrow("SetSite is being called while the adapter is being constructed.");
            if (CurrentInitializationState >= InitializationState.Sited)
                return;
            //TODO: Status bar
            //TODO: Textmanager
            _editorOptions = EditorParts.EditorOptionsFactoryService.CreateOptions();
            CurrentInitializationState = InitializationState.Sited;
        }

        private void InitializeEvents()
        {
            if (_eventsInitialized)
                return;
            try
            {
                var host = _textViewHostPrivate;
                var textView = host.TextView;
                //var textViewMargin1 = host.GetTextViewMargin("VerticalScrollBar") as IVerticalScrollBar;
            }
            finally
            {
                _eventsInitialized = true;
            }
        }

        private void SetViewOptions()
        {
            if (_initFlags == 0)
                return;

            //TODO: look at this and implement
        }

        internal static Point CalculateContextMenuPosition(ITextView textView)
        {
            if ((uint)textView.Caret.ContainingTextViewLine.VisibilityState > 0U && textView.Caret.Right >= textView.ViewportLeft && textView.Caret.Right <= textView.ViewportRight)
                return new Point(textView.Caret.Right - textView.ViewportLeft, textView.Caret.Bottom - textView.ViewportTop);
            return new Point(0.0, 0.0);
        }
    }

    public interface IMafTextViewEvents
    {
        void OnSetFocus(IMafTextView view);

        void OnKillFocus(IMafTextView view);

        void OnSetBuffer(IMafTextView view, IMafTextLines pBuffer);

        void OnChangeScrollInfo(IMafTextView view, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit);

        void OnChangeCaretLine(IMafTextView view, int iNewLine, int iOldLine);
    }

    public interface IMafTextView
    {
        int SetBuffer(IMafTextLines pBuffer);
    }

    public interface IMafTextBuffer
    {
        int InitializeContent(string text, int length);
    }

    public interface IMafTextLines : IMafTextBuffer
    {
    }
}