using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;

namespace ModernApplicationFramework.TextEditor.Implementation
{
    // TODO: Add forward backward stuff
    internal abstract class SimpleTextViewWindow : ICommandTarget, ITypedTextTarget,
        ICommandTargetInner, /*IBackForwardNavigation*/ IMafTextView, IMafUserData, IConnectionAdviseHelper
    {
        public EventHandler TextViewHostUpdated;
        public event EventHandler Initialized;

        private static Exception LastCreatedButNotInitializedException = null;
        private static string LastCreatedButNotInitializedExceptionStackTrace = null;

        private static Exception LastInitializingException;
        private static string LastInitializingExceptionStackTrace;

        private static int _execCount = 0;
        private static int _innerExecCount = 0;
        private static int _insertCharCount = 0;
        internal static bool _disableSettingImeCompositionWindowOptions = false;
        private CancellationTokenSource _codingConventionsCTS = new CancellationTokenSource();

        internal IEditorOptions _editorOptions;
        internal ITextViewHost _textViewHostPrivate;

        private List<IObscuringTip> _openedTips = new List<IObscuringTip>();
        private DispatcherTimer _tipDimmingTimer;

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

        internal IEditorOperations _editorOperations;
        private IViewPrimitives _textViewPrimitivesPrivate;
        private bool _sentTextViewCreatedNotifications;

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
            if (hr1 < 0)
                return hr1;
            var hr2 = 0;
            bool userEngaged = false;
            bool complexEdit = false;
            bool oldIntellisenseActive = false;
            bool newIntellisenseActive = false;

            try
            {
                var num = commandId;
                var category = string.Format(CultureInfo.InvariantCulture, "TextViewAdapter:Exec {0} {1} {2}", commandId.ToString(), commandGroup.ToString(), (++_execCount).ToString());

                var currentSnapshot1 = TextView.TextBuffer.CurrentSnapshot;
                if (Fire_KeyPressEvent(true, commandGroup, commandId, input))
                {
                    hr2 = _commandChain.InnerExec(ref commandGroup, commandId, nCmdexecopt, input, output);
                    Fire_KeyPressEvent(false, commandGroup, commandId, input);
                }

                if (CurrentInitializationState == InitializationState.TextViewAvailable)
                {
                    var currentSnapshot2 = TextView.TextBuffer.CurrentSnapshot;
                    complexEdit = currentSnapshot1.Version.VersionNumber + 1 != currentSnapshot2.Version.VersionNumber
                                  || currentSnapshot1.Length + 1 != currentSnapshot2.Length;
                    //Stuff
                }

            }
            catch (Exception e)
            {
            }
            return hr2;
        }

        private bool Fire_KeyPressEvent(bool isPreEvent, Guid commandGroup, uint commandId, IntPtr înput)
        {
            if (commandGroup == MafConstants.EditorCommandGroup)
            {
                switch (commandId)
                {
                    case (uint) MafConstants.EditorCommands.TypeChar:
                        return Fire_KeyPressEvent(isPreEvent, GetTypeCharFromKeyPressEventArg(înput));
                    case (uint)MafConstants.EditorCommands.Backspace:
                        return Fire_KeyPressEvent(isPreEvent, '\b');

                }
            }
            return true;
        }

        private bool Fire_KeyPressEvent(bool isPreEvent, char key)
        {
            //TODO: Text Manager stuff
            return true;
        }

        internal static unsafe char GetTypeCharFromKeyPressEventArg(IntPtr pvaIn)
        {
            KeyPressEventArg* keyPressEventArgPtr = (KeyPressEventArg*)(void*)pvaIn;
            switch (keyPressEventArgPtr->vt)
            {
                case 2:
                case 18:
                    return keyPressEventArgPtr->ch;
                default:
                    return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct KeyPressEventArg
        {
            [FieldOffset(0)]
            public readonly int vt;
            [FieldOffset(8)]
            public readonly char ch;
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
                    var flag = false;
                    for (var index = 0; index < cCmds; ++index)
                    {
                        if (TextDocData.IsCommandSupported(ref commandGroup, prgCmds[index].cmdID))
                        {
                            flag = true;
                            prgCmds[index].cmdf = 1;
                        }
                    }

                    if (!flag)
                        return int.MinValue;
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
                var handlerServiceFilter = new CommandHandlerServiceFilter(this);
                Marshal.ThrowExceptionForHR(AddCommandFilter(handlerServiceFilter, out var ppNextCmdTarg));
                handlerServiceFilter.Initialize(ppNextCmdTarg);
                CurrentInitializationState = InitializationState.TextViewAvailable;
            }
            catch (Exception ex)
            {
                LastCreatedButNotInitializedException = ex;
                LastCreatedButNotInitializedExceptionStackTrace = ex.StackTrace;
                throw;
            }
            ViewLoadedHandler.OnViewCreated(_textViewHostPrivate);
            //TODO: Theme scrollbars
            CommandRouting.SetInterceptsCommandRouting(textView.VisualElement, false);

            //TODO: Add stuff
            _editorOperations = EditorParts.EditorOperationsFactoryService.GetEditorOperations(textView);
            //Undo
            _textViewPrimitivesPrivate = IoC.Get<IEditorPrimitivesFactoryService>().GetViewPrimitives(textView);
            _classificationFormatMap = EditorParts.ClassificationFormatMapService.GetClassificationFormatMap(textView);
            _urlTagAggregator = EditorParts.ViewTagAggregatorFactoryService.CreateTagAggregator<IUrlTag>(textView);
            CleanUpEvents();
            InitializeEvents();
            var textViewHostUpdated = TextViewHostUpdated;
            textViewHostUpdated?.Invoke(this, EventArgs.Empty);
            //Lang stuff
            if (textView.Options.IsAutoScrollEnabled())
            {
                var currentSnapshot = textView.TextBuffer.CurrentSnapshot;
                if (currentSnapshot.Length > 0)
                {
                    var virtualSnapshotPoint = new VirtualSnapshotPoint(currentSnapshot, currentSnapshot.Length);
                    _editorOperations.SelectAndMoveCaret(virtualSnapshotPoint, virtualSnapshotPoint, TextSelectionMode.Stream);
                }      
            }
            //TODO: TextManager stuff
            //if (!_isViewRegistered && )
            if (!_sentTextViewCreatedNotifications)
            {
                _sentTextViewCreatedNotifications = true;
                SendTextViewCreated();
                if (_textViewHostPrivate.TextView.Properties.TryGetProperty<object>("OverviewMarginContextMenu", out var property))
                {
                    //TODO: Create class OverviewMarginProvider and all implementation around it...
                }
            }

            //StatusBar Stuff
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        private void SendTextViewCreated()
        {
            var list = IoC.GetAll<Lazy<IMafTextViewCreationListener, IContentTypeAndTextViewRoleMetadata>>();
            foreach (var matchingExtension in SelectMatchingExtensions(list, TextView.TextViewModel.DataModel.ContentType, TextView.Roles))
            {
                var instantiatedExtension = EditorParts.GuardedOperations.InstantiateExtension(matchingExtension, matchingExtension);
                if (instantiatedExtension != null)
                    EditorParts.GuardedOperations.CallExtensionPoint(instantiatedExtension, () => instantiatedExtension.MafTextViewCreated(this));
            }
        }

        private static List<Lazy<TProvider, TMetadataView>> SelectMatchingExtensions<TProvider, TMetadataView>(IEnumerable<Lazy<TProvider, TMetadataView>> providerHandles, IContentType documentContentType, ITextViewRoleSet viewRoles) where TMetadataView : IContentTypeAndTextViewRoleMetadata
        {
            var lazyList = new List<Lazy<TProvider, TMetadataView>>();
            foreach (var providerHandle in providerHandles)
            {
                var documentContentType1 = documentContentType;
                var metadata = providerHandle.Metadata;
                var contentTypes = metadata.ContentTypes;
                if (ContentTypeMatch(documentContentType1, contentTypes))
                {
                    var textViewRoleSet = viewRoles;
                    metadata = providerHandle.Metadata;
                    var textViewRoles = metadata.TextViewRoles;
                    if (textViewRoleSet.ContainsAny(textViewRoles))
                        lazyList.Add(providerHandle);
                }
            }
            return lazyList;
        }

        private static bool ContentTypeMatch(IContentType documentContentType, IEnumerable<string> extensionContentTypes)
        {
            return documentContentType != null && extensionContentTypes.Any(documentContentType.IsOfType);
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
                Keyboard.AddKeyDownHandler(textView.VisualElement, OnTextView_KeyDown);
                Keyboard.AddKeyUpHandler(textView.VisualElement, OnTextView_KeyUp);

                _editorAndMenuFocusTracker = new EditorAndMenuFocusTracker(textView);
                //_editorAndMenuFocusTracker.GotFocus += this.OnEditorOrMenuGotFocus;
                //_editorAndMenuFocusTracker.LostFocus += this.OnEditorOrMenuLostFocus;
            }
            finally
            {
                _eventsInitialized = true;
            }
        }

        private void OnTextView_KeyDown(object sender, KeyEventArgs e)
        {
            if (_openedTips.Count <= 0 || !IsDimmingKey(e))
                return;
            if (_tipDimmingTimer == null)
            {
                _tipDimmingTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 250)};
                _tipDimmingTimer.Tick += ((tickSender, tickArgs) => SetTipOpacity(0.3));
            }
            _tipDimmingTimer.Start();
        }

        private void OnTextView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!IsDimmingKey(e))
                return;
            SetTipOpacity(1.0);
        }

        private void SetTipOpacity(double opacity)
        {
            _tipDimmingTimer?.Stop();
            for (var index = _openedTips.Count - 1; index >= 0; --index)
                _openedTips[index].SetOpacity(opacity);
        }

        private static bool IsDimmingKey(KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                return true;
            if (e.Key != Key.System)
                return false;
            if (e.SystemKey != Key.LeftCtrl)
                return e.SystemKey == Key.RightCtrl;
            return true;
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

    public interface IMafTextViewCreationListener
    {
        void MafTextViewCreated(IMafTextView textViewAdapter);
    }
}