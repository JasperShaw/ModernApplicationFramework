using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Editor.NativeMethods;
using ModernApplicationFramework.Editor.Outlining;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Tagging;
using ModernApplicationFramework.Text.Ui.Classification;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Ui.Outlining;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;
using IWin32Window = System.Windows.Interop.IWin32Window;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace ModernApplicationFramework.Editor.Implementation
{
    // TODO: Add forward backward stuff
    internal abstract class SimpleTextViewWindow : ICommandTarget, ITypedTextTarget,
        ICommandTargetInner, /*IBackForwardNavigation*/ IMafTextView, IConnectionAdviseHelper, IReadOnlyViewNotification, ITextEditorPropertyCategoryContainer
    {
        public EventHandler TextViewHostUpdated;
        public event EventHandler Initialized;
        public event ChangeScrollInfoEventHandler OnChangeScrollInfo;

        public delegate void ChangeScrollInfoEventHandler(IMafTextView pView, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit);

        public delegate void SetFocusEventHandler(IMafTextView pView);
        public delegate void KillFocusEventHandler(IMafTextView pView);

        public event KillFocusEventHandler OnKillFocus;

        public event SetFocusEventHandler OnSetFocus;

        private static Exception LastCreatedButNotInitializedException = null;
        private static string LastCreatedButNotInitializedExceptionStackTrace = null;

        private static Exception LastInitializingException;
        private static string LastInitializingExceptionStackTrace;

        private static int _execCount = 0;
        private static int _innerExecCount = 0;
        private static int _insertCharCount = 0;
        internal static bool _disableSettingImeCompositionWindowOptions = false;
        private CancellationTokenSource _codingConventionsCTS = new CancellationTokenSource();

        private FontsAndColorsCategory _fontsAndColorsCategory = new FontsAndColorsCategory(ImplGuidList.GuidDefaultFileType, CategoryGuids.GuidTextEditorGroup, CategoryGuids.GuidTextEditorGroup);

        internal IEditorOptions _editorOptions;
        internal ITextViewHost _textViewHostPrivate;

        private List<IReadOnlyViewNotification> _readOnlyNotifications = new List<IReadOnlyViewNotification>();

        private List<IObscuringTip> _openedTips = new List<IObscuringTip>();
        private DispatcherTimer _tipDimmingTimer;

        internal ITagAggregator<IUrlTag> _urlTagAggregator;

        private CommandChainNode _commandChain;

        protected ConnectionPointContainer connectionPointContainerHelper;
        private IClassificationFormatMap _classificationFormatMap;
        private bool _delayedTextBufferLoad;
        private EditorAndMenuFocusTracker _editorAndMenuFocusTracker;
        private bool _eventsInitialized;

        private TextViewInitFlags _initFlags;
        private ITextViewRoleSet _initialRoles;
        private IScrollMap _scrollMap;

        internal ITextViewFilter _textViewFilter;

        private TextDocData _textDocData;
        private FrameworkElement _zoomControl;

        internal bool _canCaretAndSelectionMapToDataBuffer = true;

        private IEditorFormatMap EditorFormatMap { get; set; }


        internal IEditorOperations _editorOperations;
        private IViewPrimitives _textViewPrimitivesPrivate;
        private bool _sentTextViewCreatedNotifications;
        private bool _startNewClipboardCycle = true;

        internal int _oldVerticalMaxUnits = -1;
        internal int _oldVerticalVisibleUnits = -1;
        internal int _oldVerticalFirstVisibleUnit = -1;
        internal int _oldHorzMaxUnits = -1;
        internal int _oldHorzVisibleUnits = -1;
        internal int _oldHorzFirstVisibleUnit = -1;

        internal ViewMarkerTypeManager ViewMarkerTypeManager;

        internal TextViewShimHost ShimHost { get; set; }


        public InitializationState CurrentInitializationState { get; internal set; }

        internal ITextSnapshot DataTextSnapshot => TextDocData.GetCurrentSnapshot();

        internal TextDocData TextDocData
        {
            get => _textDocData;
            set => _textDocData = value;
        }

        internal FontsAndColorsCategory FontsAndColorsCategory
        {
            get => _fontsAndColorsCategory;
            set
            {
                _fontsAndColorsCategory = value;
                if (CurrentInitializationState >= InitializationState.TextViewAvailable && _editorOptions.GetOptionValue(DefaultViewOptions.AppearanceCategory) != value.AppearanceCategory)
                    _editorOptions.SetOptionValue(DefaultViewOptions.AppearanceCategory, value.AppearanceCategory);
                if (CurrentInitializationState < InitializationState.TextBufferAvailable || TextView == null)
                    return;
                EditorFormatMap = IoC.Get<IEditorFormatMapService>().GetEditorFormatMap(TextView);
                ApplyBackgroundColor();
            }
        }

        public IndentStyle IndentStyle { get; internal set; }


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
            }
        }

        internal bool RaiseGoBackEvents { get; set; }

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

        public int Exec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            var hr1 = PreOuterQueryStatus(commandGroup, 1, new[]
            {
                new Olecmd {cmdf = 0, cmdID = commandId}
            });
            if (hr1 < 0)
                return hr1;
            var hr2 = 0;
            var userEngaged = false;
            var complexEdit = false;
            var oldIntellisenseActive = false;
            var newIntellisenseActive = false;

            try
            {
                var num = commandId;
                var category = string.Format(CultureInfo.InvariantCulture, "TextViewAdapter:Exec {0} {1} {2}", commandId.ToString(), commandGroup.ToString(), (++_execCount).ToString());

                var currentSnapshot1 = TextView.TextBuffer.CurrentSnapshot;
                if (Fire_KeyPressEvent(true, commandGroup, commandId, input))
                {
                    hr2 = _commandChain.InnerExec(commandGroup, commandId, nCmdexecopt, input, output);
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
                    case (uint)MafConstants.EditorCommands.TypeChar:
                        return Fire_KeyPressEvent(isPreEvent, GetTypeCharFromKeyPressEventArg(înput));
                    case (uint)MafConstants.EditorCommands.Backspace:
                        return Fire_KeyPressEvent(isPreEvent, '\b');

                }
            }
            return true;
        }


        private int? _backgroundColorIndex;

        private int? BackgroundColorIndex
        {
            get => _backgroundColorIndex;
            set
            {
                if (!value.HasValue)
                    return;
                _backgroundColorIndex = value;
                ApplyBackgroundColor();
            }
        }

        private void ApplyBackgroundColor()
        {
            if (CurrentInitializationState < InitializationState.TextViewAvailable)
                return;
            var backgroundColorIndex = BackgroundColorIndex;
            if (!backgroundColorIndex.HasValue || _textDocData == null || _textDocData.MarkerManager == null)
                return;
            var nullable = new Color?();
            var editorFormatMap = EditorFormatMap;
            var markerManager = _textDocData.MarkerManager;
            backgroundColorIndex = BackgroundColorIndex;
            var type = backgroundColorIndex.Value;
            var mergeName = markerManager.GetMarkerType(type).MergeName;
            var properties = editorFormatMap.GetProperties(mergeName);
            if (properties.Contains("BackgroundColor"))
                nullable = properties["BackgroundColor"] as Color?;
            if (!nullable.HasValue && properties.Contains("Background"))
            {
                if (properties["Background"] is SolidColorBrush solidColorBrush)
                    nullable = solidColorBrush.Color;
            }
            if (!nullable.HasValue)
                return;
            var solidColorBrush1 = new SolidColorBrush(nullable.Value);
            solidColorBrush1.Freeze();
            TextView.Background = solidColorBrush1;
        }

        public int SetBackgroundColorIndex(int iBackgroundIndex)
        {
            BackgroundColorIndex = iBackgroundIndex;
            return 0;
        }

        private bool Fire_KeyPressEvent(bool isPreEvent, char key)
        {
            //TODO: Text Manager stuff
            return true;
        }

        internal static unsafe char GetTypeCharFromKeyPressEventArg(IntPtr pvaIn)
        {
            var keyPressEventArgPtr = (KeyPressEventArg*)(void*)pvaIn;
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

        private int PreOuterQueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds)
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
                        if (TextDocData.IsCommandSupported(commandGroup, prgCmds[index].cmdID))
                        {
                            flag = true;
                            prgCmds[index].cmdf = Olecmdf.Supported;
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

            if (format == MafUserDataFormat.ContextMenuId)
            {
                pvtData = ContextMenuId;
                return 0;
            }

            pvtData = null;
            return -2147467259;
        }

        public int InnerExec(Guid commandGroup, uint commandId, uint nCmdexecopt, IntPtr input, IntPtr output)
        {
            try
            {
                if (IsCommandExecutionProhibited())
                    return -2147221248;

                var result = 0;

                if (IsViewOrBufferReadOnly() && IsEditingCommand(commandGroup, commandId) &&
                    !IsSearchingCommand(commandGroup, commandId))
                    return OnDisabledEditingCommand(commandGroup, commandId);

                var newClipboardCycle = _startNewClipboardCycle;
                //Should always be true: if (commandGroup != VSConstants.GUID_VSStandardCommandSet97 || commandId != 655U)
                _startNewClipboardCycle = true;

                if (commandGroup == MafConstants.EditorCommandGroup)
                {
                    switch ((MafConstants.EditorCommands)commandId)
                    {
                        // Some other Commands
                        case MafConstants.EditorCommands.MoveSelLinesDown:
                            return _editorOperations.MoveSelectedLinesDown() ? 0 : -2147467259;
                        case MafConstants.EditorCommands.MoveSelLinesUp:
                            return _editorOperations.MoveSelectedLinesUp() ? 0 : -2147467259;
                        case MafConstants.EditorCommands.ZoomIn:
                            _editorOperations.ZoomIn();
                            break;
                        case MafConstants.EditorCommands.ZoomOut:
                            _editorOperations.ZoomOut();
                            break;
                        // 2k Commands
                        case MafConstants.EditorCommands.TypeChar:
                            InsertChar(input, InProvisionalInput);
                            break;
                        case MafConstants.EditorCommands.Backspace:
                            Backsapce();
                            break;
                        case MafConstants.EditorCommands.Return:
                            InsertNewLine();
                            break;
                        case MafConstants.EditorCommands.Left:
                            _editorOperations.MoveToPreviousCharacter(false);
                            break;
                        case MafConstants.EditorCommands.Copy:
                            Copy();
                            break;
                        case MafConstants.EditorCommands.ShowContextMenu:
                            ShowContextMenu(input, ref result);
                            break;
                        default:
                            result = -2147221248;
                            break;
                    }
                }
                else
                    result = -2147221244;

                if (result == -2147221248 || result == -2147221244)
                {
                    _startNewClipboardCycle = newClipboardCycle;
                    return result;
                }
                return DefaultErrorHandler(result, ref commandGroup);


            }
            catch
            {
            }
            return -2147221248;
        }

        private void Copy()
        {
            _editorOperations.CopySelection();
        }

        private void ShowContextMenu(IntPtr location, ref int result)
        {
            Point[] pos = null;
            if (location != IntPtr.Zero)
            {
                var forNativeVariant1 = Marshal.GetObjectForNativeVariant(location);
                var forNativeVariant2 = Marshal.GetObjectForNativeVariant(new IntPtr(location.ToInt32() + 16));

                var nullable1 = forNativeVariant1 as short?;
                var nullable2 = forNativeVariant2 as short?;
                if (nullable1.HasValue && nullable2.HasValue)
                {
                    pos = new Point[1];
                    pos[0].X = nullable1.Value;
                    pos[0].Y = nullable2.Value;
                }
            }

            if (pos == null)
            {
                var wpfTextView = TextView;
                if (PresentationSource.FromDependencyObject(wpfTextView.VisualElement) != null)
                {
                    var contextMenuPosition = CalculateContextMenuPosition(wpfTextView);
                    var screen = wpfTextView.VisualElement.PointToScreen(contextMenuPosition);
                    pos = new Point[1];
                    pos[0].X = (short)screen.X;
                    pos[0].Y = (short)screen.Y;
                }
            }

            if (pos != null)
            {
                if (GetData(MafUserDataFormat.ContextMenuId, out var data) == 0 && data is Guid contextMenuId)
                    IoC.Get<IMafUIShell>().ShowContextMenu(pos[0], contextMenuId, TextView.VisualElement);
            }
            else
                result = -2147221248;
        }

        private void InsertNewLine()
        {
            _editorOperations.InsertNewLine();
        }

        private void Backsapce()
        {
            if (_editorOperations.ProvisionalCompositionSpan != null)
                _editorOperations.InsertText("");
            else
                _editorOperations.Backspace();
        }

        private void InsertChar(IntPtr pvaIn, bool provisionalText)
        {
            var chr = GetTypeCharFromKeyPressEventArg(pvaIn);
            var text = chr.ToString();

            if (chr == '\t' && _editorOptions.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId))
            {
                var value = _editorOptions.GetOptionValue(DefaultOptions.TabSizeOptionId);
                text = new string(' ', value - TextViewPrimitives.Caret.Column % value);
            }

            if (provisionalText)
                _editorOperations.InsertProvisionalText(text);
            else
                _editorOperations.InsertText(text);
        }

        internal IViewPrimitives TextViewPrimitives
        {
            get
            {
                if (_textViewPrimitivesPrivate == null)
                {
                    Init_OnActivation();
                    if (_textViewPrimitivesPrivate == null)
                        throw new InvalidOperationException("VsSimpleTextViewWindow initialization hasn't initialized text view primitives!");
                }
                return _textViewPrimitivesPrivate;
            }
            set => _textViewPrimitivesPrivate = value;
        }

        private static int DefaultErrorHandler(int report, ref Guid pguidCmdGroup)
        {
            if (report >= 0 || !(pguidCmdGroup == MafConstants.EditorCommandGroup))
                return report;
            report = 0;
            return report;
        }

        public int OnDisabledEditingCommand(Guid pguidCmdGuid, uint dwCmdId)
        {
            var num1 = 0;
            foreach (var onlyNotification in _readOnlyNotifications)
            {
                var num2 = onlyNotification.OnDisabledEditingCommand(pguidCmdGuid, dwCmdId);
                if (num2 < 0)
                    num1 = num2;
            }
            return num1;
        }

        public bool IsIncrementalSearchInProgress { get; set; }

        public bool IsSearchingCommand(Guid cmdGroup, uint cmdId)
        {
            if (!IsIncrementalSearchInProgress)
                return false;
            if (cmdGroup == MafConstants.EditorCommandGroup)
            {
                switch ((MafConstants.EditorCommands)cmdId)
                {
                    case MafConstants.EditorCommands.TypeChar:
                    case MafConstants.EditorCommands.Backspace:
                    case MafConstants.EditorCommands.Return:
                        return true;
                }
            }

            return false;
        }

        private bool IsEditingCommand(Guid commandGroup, uint commandId)
        {
            if (commandGroup == MafConstants.EditorCommandGroup)
            {
                switch ((MafConstants.EditorCommands)commandId)
                {
                    case MafConstants.EditorCommands.TypeChar:
                    case MafConstants.EditorCommands.Backspace:
                    case MafConstants.EditorCommands.Return:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }

        private bool IsViewOrBufferReadOnly()
        {
            return IsReadOnly() == 0 || (TextDocData.TextBufferState & 1) != 0;
        }

        public int IsReadOnly()
        {
            return _editorOptions.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId) ? 0 : 1;
        }

        public int InnerQueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            if (IsCommandExecutionProhibited())
                return -2147221248;
            if (commandGroup == MafConstants.EditorCommandGroup)
            {
                for (var i = 0; i < cCmds; ++i)
                {
                    switch (prgCmds[i].cmdID)
                    {
                        case 57:
                            break;
                        default:
                            prgCmds[i].cmdf = prgCmds[i].cmdID <= 0 || prgCmds[i].cmdID >= 145
                                ? Olecmdf.None
                                : Olecmdf.Supported | Olecmdf.Enabled;
                            break;
                    }
                }

            }
            return 0;
        }

        public int QueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            var hr = PreOuterQueryStatus(commandGroup, cCmds, prgCmds);
            if (hr < 0)
                return hr;
            return _commandChain.QueryStatus(commandGroup, cCmds, prgCmds, pCmdText);
        }

        public int SetBuffer(IMafTextLines pBuffer)
        {
            return Init_SetBuffer(pBuffer);
        }

        public void Initialize(IMafTextLines buffer, IntPtr hwndParent, TextViewInitFlags flags)
        {
            Init_Initialize(buffer, hwndParent, flags);
        }

        private void Init_Initialize(IMafTextLines buffer, IntPtr hwndParent, TextViewInitFlags flags)
        {
            if (CurrentInitializationState != InitializationState.Sited)
                FailInitializationAndThrow("Initialize is being called before the adapter has been sited.");
            _initFlags = flags;

            var t = ((int)flags & 32768) != 0;

            ShimHost.Initialize(((int)flags & 32768) == 0, hwndParent);
            ShimHost.NowVisible += ShimHostNowVisible;
            Init_SetBuffer(buffer);
        }

        private void ShimHostNowVisible(object sender, EventArgs e)
        {
            if (CurrentInitializationState < InitializationState.TextBufferAvailable)
                return;
            Init_OnActivation();
        }

        public int SetData(MafUserDataFormat format, object vtData)
        {
            if (format == MafUserDataFormat.ContextMenuId && vtData is Guid contextMenuId)
            {
                ContextMenuId = contextMenuId;
                return 0;
            }

            return -2147467263;
        }

        private Guid ContextMenuId { get; set; } = Guid.Empty;

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
            if (_eventsInitialized)
            {
                //    var textView = _textViewHostPrivate.TextView;
                //    textView.Caret.PositionChanged -= Caret_PositionChanged;
                //    textView.Selection.SelectionChanged -= Selection_SelectionChanged;
                //    textView.LayoutChanged -= View_LayoutChanged;
                //    Keyboard.RemoveKeyDownHandler(textView.VisualElement, OnTextView_KeyDown);
                //    Keyboard.RemoveKeyUpHandler(textView.VisualElement, OnTextView_KeyUp);
                //    textView.ViewportLeftChanged -= View_ViewportLeftChanged;
                //    textView.ViewportWidthChanged -= View_ViewportWidthChanged;
                //    _editorOptions.OptionChanged -= EditorOptions_OptionChanged;

                //    textView.TextBuffer.Changed -= DocData_OnChangeLineText;
                //    //TODO: Add textDocData stuff
                _editorAndMenuFocusTracker.GotFocus -= OnEditorOrMenuGotFocus;
                _editorAndMenuFocusTracker.LostFocus -= OnEditorOrMenuLostFocus;
                _editorAndMenuFocusTracker = null;
                if (_zoomControl != null)
                {
                    _zoomControl.IsKeyboardFocusWithinChanged -= OnZoomIsKeyboardFocusWithinChanged;
                    _zoomControl = null;
                }
                EditorParts.EditorFormatMapService.GetEditorFormatMap(TextView).FormatMappingChanged -= OnFormatMapChanged;
                _classificationFormatMap.ClassificationFormatMappingChanged -= ClassificationFormatMap_ClassificationFormatMappingChanged;
            }
            _eventsInitialized = false;
            _scrollMap = null;
        }

        private void OnFormatMapChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("TextView Background"))
                return;
            this.ApplyBackgroundColor();
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
            ShimHost = new TextViewShimHost(this);
            InitializeConnectionPoints();
            _commandChain = new CommandChainNode
            {
                Next = this
            };
            RaiseGoBackEvents = true;
            //TODO: Marker stuff
            ViewMarkerTypeManager = new ViewMarkerTypeManager();
            CurrentInitializationState = InitializationState.Constructed;
        }

        private void Init_InitializeWpfTextView()
        {
            FontsAndColorsCategory andColorsCategory;
            ITextView textView;
            ITextViewHost textViewHost;
            try
            {
                CurrentInitializationState = InitializationState.TextViewInitializing;
                if (_initialRoles == null)
                    _initialRoles = EditorParts.TextEditorFactoryService.DefaultRoles;
                SetViewOptions();
                andColorsCategory = FontsAndColorsCategory.SetLanguageService(TextDocData.ActualLanguageServiceID);
                _editorOptions.SetOptionValue(DefaultViewOptions.AppearanceCategory, andColorsCategory.AppearanceCategory);

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
            ThemeTextViewHostScrollBars(_textViewHostPrivate);
            CommandRouting.SetInterceptsCommandRouting(textView.VisualElement, false);
            _outliningManager = EditorParts.OutliningManagerService.GetOutliningManager(textView) as IAccurateOutliningManager;
            if (_outliningManager != null)
                new HiddenTextSessionCoordinator(this, textView, _outliningManager, _editorOptions, _textDocData);
            FontsAndColorsCategory = andColorsCategory;
            ShimHost.TextViewHost = _textViewHostPrivate;
            _editorOperations = EditorParts.EditorOperationsFactoryService.GetEditorOperations(textView);
            //Undo
            ViewMarkerTypeManager.AttachToView(textView);
            _textViewPrimitivesPrivate = IoC.Get<IEditorPrimitivesFactoryService>().GetViewPrimitives(textView);
            _classificationFormatMap = EditorParts.ClassificationFormatMapService.GetClassificationFormatMap(textView);
            _urlTagAggregator = EditorParts.ViewTagAggregatorFactoryService.CreateTagAggregator<IUrlTag>(textView);
            CleanUpEvents();
            InitializeEvents();
            RegisterApplicationCommands();
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
                    if (property is ContextMenu contextMenu)
                    {
                        contextMenu.Items.Add(new Separator());
                        var menuItem = new MenuItem
                        {
                            Header = "Options",
                            IsCheckable = false,
                            IsChecked = false
                        };
                        //TODO: Localize
                        var optionsMenuCommand = new ScrollBarsToolsOptionsMenuCommand();
                        menuItem.Command = optionsMenuCommand;
                        contextMenu.Items.Add(menuItem);
                    }
                }
            }

            //StatusBar Stuff
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        private void ThemeTextViewHostScrollBars(ITextViewHost textViewHost)
        {
            var hostControl = textViewHost?.HostControl;
            if (hostControl == null)
                return;
            ImageThemingUtilities.SetThemeScrollBars(hostControl, true);
        }

        private void RegisterApplicationCommands()
        {
            CommandHelpers.RegisterCommandHandler(TextView.VisualElement.GetType(), ApplicationCommands.Copy, OnCopy, CanCopy);
        }

        private static void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            var guid = MafConstants.EditorCommandGroup;
            var prgCmds = new[]
            {
                new Olecmd { cmdID = (uint) MafConstants.EditorCommands.Copy }
            };

            if (!(sender is IPropertyOwner textView))
                return;
            var target = GetTarget(textView);

            target?.QueryStatus(guid, (uint)prgCmds.Length, prgCmds, IntPtr.Zero);
            e.CanExecute = prgCmds[0].cmdf.HasFlag(Olecmdf.Supported) && prgCmds[0].cmdf.HasFlag(Olecmdf.Enabled);
        }

        private static void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is IPropertyOwner textView))
                return;
            var target = GetTarget(textView);
            target?.Exec(MafConstants.EditorCommandGroup, (uint) MafConstants.EditorCommands.Copy, 0, IntPtr.Zero,
                IntPtr.Zero);
        }

        private static ICommandTarget GetTarget(IPropertyOwner textView)
        {
            return textView.Properties.GetProperty(typeof(ICommandTarget)) as ICommandTarget;
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
            if (pNewCmdTarg is IReadOnlyViewNotification viewNotification)
                _readOnlyNotifications.Add(viewNotification);
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
                if (host.GetTextViewMargin("VerticalScrollBar") is IVerticalScrollBar scrollBar)
                    _scrollMap = scrollBar.Map;
                textView.LayoutChanged += View_LayoutChanged;


                Keyboard.AddKeyDownHandler(textView.VisualElement, OnTextView_KeyDown);
                Keyboard.AddKeyUpHandler(textView.VisualElement, OnTextView_KeyUp);

                textView.TextBuffer.Changed += DocData_OnChangeLineText;

                _editorAndMenuFocusTracker = new EditorAndMenuFocusTracker(textView);
                _editorAndMenuFocusTracker.GotFocus += OnEditorOrMenuGotFocus;
                _editorAndMenuFocusTracker.LostFocus += OnEditorOrMenuLostFocus;

                var zoomControl = TextViewHost.GetTextViewMargin("ZoomControl");
                if (zoomControl != null)
                {
                    _zoomControl = zoomControl.VisualElement;
                    if (_zoomControl != null)
                        _zoomControl.IsKeyboardFocusWithinChanged += OnZoomIsKeyboardFocusWithinChanged;
                }
                _classificationFormatMap.ClassificationFormatMappingChanged += ClassificationFormatMap_ClassificationFormatMappingChanged;
                SetPlainTextFont();
            }
            finally
            {
                _eventsInitialized = true;
            }
        }

        private void OnEditorOrMenuLostFocus(object sender, EventArgs e)
        {
            var raiseGoBackEvents = RaiseGoBackEvents;
            try
            {
                RaiseGoBackEvents = false;
                var onKillFocus = OnKillFocus;
                onKillFocus?.Invoke(this);
            }
            finally
            {
                RaiseGoBackEvents = raiseGoBackEvents;
            }

        }

        private void OnEditorOrMenuGotFocus(object sender, EventArgs e)
        {
            var onSetFocus = OnSetFocus;
            onSetFocus?.Invoke(this);

        }

        internal void ClassificationFormatMap_ClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            SetPlainTextFont();
        }

        private string _plainTextFont;
        private bool _canChangeWordWrap;
        private bool _canChangeUseVirtualSpace;
        private bool _canChangeSelectionMarginEnabled;
        private bool _canChangeOvertypeMode;
        private bool _canChangeVisibleWhitespace;
        private bool _canChangeShowVerticalScrollBar;
        private bool _canChangeShowHorizontalScrollBar;
        private bool _canChangeIndentStyle;
        private bool _canChangeGlyphMarginEnabled;
        private bool _canChangeTrackChanges;
        private bool _canChangeHotURLs;
        private IAccurateOutliningManager _outliningManager;

        internal void SetPlainTextFont()
        {
            if (_disableSettingImeCompositionWindowOptions)
                return;
            var source = _classificationFormatMap.DefaultTextProperties.Typeface.FontFamily.Source;
            if (_plainTextFont == source)
                return;
            _plainTextFont = source;
            var str1 = "CompositionFonts\\" + _plainTextFont;
            //TODO:
            //using (ServiceProvider serviceProvider = new ServiceProvider(Common.GlobalServiceProvider))
            //{
            //    SettingsStore onlySettingsStore = new ShellSettingsManager((IServiceProvider)serviceProvider).GetReadOnlySettingsStore(SettingsScope.UserSettings);
            //    string str2 = onlySettingsStore.GetString(str1, "CompositionFont", "");
            //    double doubleFromStore1 = GetDoubleFromStore(onlySettingsStore, str1, "TopOffset");
            //    double doubleFromStore2 = GetDoubleFromStore(onlySettingsStore, str1, "BottomOffset");
            //    double doubleFromStore3 = GetDoubleFromStore(onlySettingsStore, str1, "HeightOffset");
            //    _editorOptions.SetOptionValue("ImeCompositionWindowFont", str2);
            //    _editorOptions.SetOptionValue("ImeCompositionWindowTopOffset", doubleFromStore1);
            //    _editorOptions.SetOptionValue("ImeCompositionWindowBottomOffset", doubleFromStore2);
            //    _editorOptions.SetOptionValue("ImeCompositionWindowHeightOffset", doubleFromStore3);
            //}
        }

        private void OnZoomIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_zoomControl.IsKeyboardFocusWithin)
                return;
            Keyboard.Focus(TextView.VisualElement);
            //TODO:
        }

        private void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (CurrentInitializationState < InitializationState.TextViewAvailable)
                return;
            HandleVerticalScroll();
        }

        private void HandleVerticalScroll()
        {
            var iMinUnit = 0;
            int iMaxUnits;
            int iVisibleUnits;
            int iFirstVisibleUnit;
            if (_scrollMap == null)
            {
                iMaxUnits = TextView.TextSnapshot.LineCount;
                iVisibleUnits = (int)(TextView.ViewportHeight / 14.0);
                iFirstVisibleUnit = TextView.TextViewLines.FirstVisibleLine.Start.GetContainingLine().LineNumber;
            }
            else
            {
                iMaxUnits = (int)(_scrollMap.End - _scrollMap.Start);
                iVisibleUnits = (int)_scrollMap.ThumbSize;
                iFirstVisibleUnit =
                    (int)_scrollMap.GetCoordinateAtBufferPosition(TextView.TextViewLines.FirstVisibleLine.Start);
            }
            if (_oldVerticalMaxUnits == iMaxUnits && _oldVerticalVisibleUnits == iVisibleUnits && _oldVerticalFirstVisibleUnit == iFirstVisibleUnit)
                return;
            _oldVerticalMaxUnits = iMaxUnits;
            _oldVerticalVisibleUnits = iVisibleUnits;
            _oldVerticalFirstVisibleUnit = iFirstVisibleUnit;
            OnChangeScrollInfo?.Invoke(this, 1, iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);
        }

        private void DocData_OnChangeLineText(object sender, TextContentChangedEventArgs e)
        {
            //ClearBraceHighlighing();
        }

        private void OnTextView_KeyDown(object sender, KeyEventArgs e)
        {
            if (_openedTips.Count <= 0 || !IsDimmingKey(e))
                return;
            if (_tipDimmingTimer == null)
            {
                _tipDimmingTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 250) };
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
            if ((_initFlags & TextViewInitFlags.Vscroll) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId, true);
                _canChangeShowVerticalScrollBar = false;
            }

            if ((_initFlags & TextViewInitFlags.Hscroll) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewHostOptions.HorizontalScrollBarId, true);
                _canChangeShowHorizontalScrollBar = false;
            }
            if ((_initFlags & TextViewInitFlags.IndentMode) != 0)
            {
                IndentStyle = IndentStyle.None;
                _canChangeIndentStyle = false;
            }

            if ((_initFlags & TextViewInitFlags.Readonly) != 0)
                _editorOptions.SetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId, true);
            if ((_initFlags & TextViewInitFlags.SuppressTrackGoBack) != 0)
                RaiseGoBackEvents = false;
            if ((_initFlags & TextViewInitFlags.UpdateStatusBar) != 0)
                SupressUpdateStatusBarEvents = false;
            if ((_initFlags & TextViewInitFlags.WidgetMargin) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewHostOptions.GlyphMarginId, false);
                _canChangeGlyphMarginEnabled = false;
            }
            if ((_initFlags & TextViewInitFlags.SelectionMargin) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewHostOptions.SelectionMarginId, false);
                _canChangeSelectionMarginEnabled = false;
            }
            if ((_initFlags & TextViewInitFlags.VirtualSpace) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewOptions.UseVirtualSpaceId, false);
                _canChangeUseVirtualSpace = false;
            }
            if ((_initFlags & TextViewInitFlags.Overtype) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewOptions.OverwriteModeId, false);
                _canChangeOvertypeMode = false;
            }
            if ((_initFlags & TextViewInitFlags.SuppressTrackChanges) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewHostOptions.ChangeTrackingId, false);
                _canChangeTrackChanges = false;
            }
            if ((_initFlags & TextViewInitFlags.Hoturls) != 0)
            {
                _editorOptions.SetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId, false);
                _canChangeHotURLs = false;
            }
            if ((_initFlags & TextViewInitFlags.EnableAutoModusFind) != 0)
                _editorOptions.SetOptionValue(EnableFindOptionDefinition.KeyId, true);
        }

        internal bool SupressUpdateStatusBarEvents { get; set; }

        internal static Point CalculateContextMenuPosition(ITextView textView)
        {
            if ((uint)textView.Caret.ContainingTextViewLine.VisibilityState > 0U && textView.Caret.Right >= textView.ViewportLeft && textView.Caret.Right <= textView.ViewportRight)
                return new Point(textView.Caret.Right - textView.ViewportLeft, textView.Caret.Bottom - textView.ViewportTop);
            return new Point(0.0, 0.0);
        }

        public int GetPropertyCategory(Guid rguidCategory, out ITextEditorPropertyContainer ppProp)
        {
            if (rguidCategory == DefGuidList.GuidEditPropCategoryViewMasterSettings)
            {
                ppProp = new TextEditorPropertyContainerAdapter(this);
                return 0;
            }
            ppProp = null;
            return -2147024809;
        }

        internal int GetPropertyFromPropertyContainer(EditPropId idProp, out object pvar)
        {
            pvar = null;
            switch (idProp)
            {
                case EditPropId.ViewGeneralColorCategory:
                    pvar = FontsAndColorsCategory.ColorCategory;
                    return 0;
                case EditPropId.ViewGeneralFontCategory:
                    pvar = FontsAndColorsCategory.FontCategory;
                    return 0;
                default:
                    return -2147024809;
            }
        }

        internal int SetPropertyInPropertyContainer(EditPropId idProp, object pvar)
        {
            switch (idProp)
            {
                case EditPropId.ViewGeneralFontCategory:
                    FontsAndColorsCategory = FontsAndColorsCategory.SetFontCategory(new Guid(pvar.ToString()));
                    return 0;
                case EditPropId.ViewGeneralColorCategory:
                    FontsAndColorsCategory = FontsAndColorsCategory.SetColorCategory(new Guid(pvar.ToString()));
                    return 0;
                default:
                    return -2147024809;
            }
        }

        public int RemovePropertyFromPropertyContainer(EditPropId idProp)
        {
            if (idProp <= EditPropId.ViewLangOptWordWrap)
            {
                if (idProp == EditPropId.ViewLangOptRawTextDisplay)
                    return -2147467259;
                if (idProp != EditPropId.ViewLangOptVirtualSpace)
                {
                    if (idProp != EditPropId.ViewLangOptWordWrap)
                        return -2147024809;
                    _canChangeWordWrap = true;
                    return 0;
                }
                _canChangeUseVirtualSpace = true;
                return 0;
            }
            if (idProp == EditPropId.ViewGlobalOptAutoScrollCaretOnTextEntry)
                return -2147467259;
            switch (idProp - -131075)
            {
                case ~EditPropId.Last:
                    _canChangeSelectionMarginEnabled = true;
                    return 0;
                case (EditPropId)1:
                    _canChangeOvertypeMode = true;
                    return 0;
                case (EditPropId)2:
                    _canChangeVisibleWhitespace = true;
                    return 0;
                default:
                    return -2147024809;
            }
        }

        internal class TextViewShimHost : IDisposable
        {
            private ContentPresenter _contentPresenter;
            private ForwardFocusPanel _forwardFocusPanel;
            private ITextViewHost _textViewHost;


            internal event EventHandler<EventArgs> NowVisible;

            internal bool CreateHwnd { get; private set; }

            internal bool HasBeenShown { get; private set; }

            internal bool Initialized { get; private set; }

            private SimpleTextViewWindow TextView { get; }

            internal HwndSource HwndSource { get; set; }

            internal Win32Window Win32Window { get; private set; }

            internal ITextViewHost TextViewHost
            {
                get => _textViewHost;
                set
                {
                    _textViewHost = value ?? throw new ArgumentNullException(nameof(value));
                    if (CreateHwnd)
                        TextOptions.SetTextFormattingMode(_textViewHost.HostControl, TextFormattingMode.Display);
                    if (CreateHwnd && HwndSource != null)
                    {
                        HwndSource.RootVisual = _textViewHost.HostControl;
                    }
                    else
                    {
                        if (CreateHwnd)
                            return;
                        _forwardFocusPanel.ViewHost = _textViewHost;
                        _contentPresenter.Content = _textViewHost.HostControl;
                    }
                }
            }

            internal TextViewShimHost(SimpleTextViewWindow textView)
            {
                Initialized = false;
                CreateHwnd = false;
                TextView = textView;
            }

            public void Dispose()
            {
                if (_contentPresenter != null)
                    PresentationSource.RemoveSourceChangedHandler(_contentPresenter, OnSourceChanged);
                if (CreateHwnd && HwndSource != null)
                    HwndSource.Dispose();
                else if (_forwardFocusPanel != null)
                    _forwardFocusPanel.Dispose();
                GC.SuppressFinalize(this);
            }

            internal void Initialize(bool createHwnd, IntPtr hwndParent)
            {
                CreateHwnd = createHwnd;
                if (createHwnd)
                    HasBeenShown = true;
                if (createHwnd && hwndParent != IntPtr.Zero)
                    CreateHwndSource(hwndParent);
                else if (!createHwnd)
                {
                    _contentPresenter = new ContentPresenter();
                    InputMethod.SetIsInputMethodSuspended(_contentPresenter, true);
                    _forwardFocusPanel = new ForwardFocusPanel();
                    Win32Window = new Win32Window(_forwardFocusPanel.Handle);
                    PresentationSource.AddSourceChangedHandler(_contentPresenter, OnSourceChanged);
                }

                Initialized = true;
            }

            private void CreateHwndSource(IntPtr hwndParent)
            {
                var parameters = new HwndSourceParameters
                {
                    Width = 0,
                    Height = 0,
                    WindowStyle = 335544320
                };
                if (hwndParent != IntPtr.Zero)
                    parameters.WindowStyle |= 1073741824;
                parameters.ParentWindow = hwndParent;
                HwndSource = new HwndSource(parameters);
                HwndSource.AddHook(HwndSourceWndProc);
                if (TextViewHost != null)
                    HwndSource.RootVisual = _textViewHost.HostControl;
                MouseCursorResponsiveNativeWindow.OverrideWmSetCursor(HwndSource.Handle);
                Win32Window = new Win32Window(HwndSource.Handle);
            }

            private IntPtr HwndSourceWndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                if (msg == 7)
                {
                    try
                    {
                        if (wParam != HwndSource.Handle)
                        {
                            if (TextView?.TextViewHost != null)
                                Keyboard.Focus(TextView.TextViewHost.TextView.VisualElement);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                return IntPtr.Zero;
            }

            private void OnSourceChanged(object sender, SourceChangedEventArgs e)
            {
                PresentationSource.RemoveSourceChangedHandler(_contentPresenter, OnSourceChanged);
                _contentPresenter.LayoutUpdated += OnLayoutUpdated;
            }

            private void OnLayoutUpdated(object sender, EventArgs e)
            {
                _contentPresenter.LayoutUpdated -= OnLayoutUpdated;
                HasBeenShown = true;
                var nowVisible = NowVisible;
                nowVisible?.Invoke(this, EventArgs.Empty);
            }
        }

        internal sealed class Win32Window : IWin32Window
        {
            public Win32Window(IntPtr handle)
            {
                Handle = handle;
            }

            public IntPtr Handle { get; }
        }

        internal class ForwardFocusPanel : HwndWrapper
        {
            internal ITextViewHost ViewHost { get; set; }

            protected override ushort CreateWindowClassCore()
            {
                return RegisterClass("HwndlessEditorFakeHwndWrapper" + Guid.NewGuid());
            }

            protected override IntPtr CreateWindowCore()
            {
                var minValue = int.MinValue;
                var moduleHandle = Kernel32.GetModuleHandle(null);
                return User32.CreateWindowEx(0, new IntPtr(WindowClassAtom), null, minValue, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, moduleHandle, IntPtr.Zero);
            }

            internal void UpdatePosition()
            {
                if (ViewHost == null)
                    return;
                var hostControl = ViewHost.HostControl;
                if (PresentationSource.FromDependencyObject(hostControl) == null)
                    return;
                var screen = hostControl.PointToScreen(new Point(0.0, 0.0));
                var deviceUnits = DpiHelper.Default.LogicalToDeviceUnits(new Size(hostControl.ActualWidth, hostControl.ActualHeight));
                User32.SetWindowPos(Handle, IntPtr.Zero, (int)screen.X, (int)screen.Y, (int)deviceUnits.Width, (int)deviceUnits.Height, 20);
            }

            protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                if (msg != 7 || ViewHost == null || ViewHost.TextView == null)
                    return base.WndProc(hwnd, msg, wParam, lParam);
                if (Keyboard.FocusedElement == ViewHost.TextView.VisualElement)
                {
                    if (PresentationSource.FromVisual(ViewHost.TextView.VisualElement) is HwndSource hwndSource)
                        User32.SetFocus(hwndSource.Handle);
                }
                Keyboard.Focus(ViewHost.TextView.VisualElement);
                return IntPtr.Zero;
            }
        }

        internal void StartOutlining(bool removeAdhoc)
        {
            if (_outliningManager == null)
                return;
            if (removeAdhoc)
            {
                var outlinerForView = AdhocOutliner.GetOutlinerForView(TextView, _outliningManager);
                if (outlinerForView != null)
                {
                    var textSnapshot = TextView.TextSnapshot;
                    //TODO: outlinerForView.RemoveRegions(this._undoManager.TextBufferUndoHistory, new SnapshotSpan(textSnapshot, 0, textSnapshot.Length));
                }
            }
            _outliningManager.Enabled = true;
        }

        internal void StopOutlining()
        {
            if (_outliningManager == null)
                return;
            _outliningManager.Enabled = false;
            var outlinerForView = AdhocOutliner.GetOutlinerForView(TextView, _outliningManager);
            if (outlinerForView == null)
                return;
            var textSnapshot = TextView.TextSnapshot;
            //TODO: outlinerForView.RemoveRegions(this._undoManager.TextBufferUndoHistory, new SnapshotSpan(textSnapshot, 0, textSnapshot.Length));
        }

        private class ScrollBarsToolsOptionsMenuCommand : ICommand
        {

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                //TODO: Open settings for scroll bar
            }
        }
    }

    public interface ITextEditorPropertyCategoryContainer
    {
        int GetPropertyCategory(Guid rguidCategory, out ITextEditorPropertyContainer ppProp);
    }

    public interface ITextEditorPropertyContainer
    {
        int GetProperty(EditPropId idProp, out object pvar);

        int SetProperty(EditPropId idProp, object var);

        int RemoveProperty(EditPropId idProp);
    }

    public interface IMafTextViewEvents
    {
        void OnSetFocus(IMafTextView view);

        void OnKillFocus(IMafTextView view);

        void OnSetBuffer(IMafTextView view, IMafTextLines pBuffer);

        void OnChangeScrollInfo(IMafTextView view, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits, int iFirstVisibleUnit);

        void OnChangeCaretLine(IMafTextView view, int iNewLine, int iOldLine);
    }

    public interface IMafTextView : IMafUserData
    {
        int SetBuffer(IMafTextLines pBuffer);

        void Initialize(IMafTextLines buffer, IntPtr hwndParent, TextViewInitFlags flags);
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

    public enum IndentStyle
    {
        None,
        Default,
        Smart
    }
}