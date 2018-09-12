using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.GotoDialog;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Controls.Dialogs;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Editor.NativeMethods;
using ModernApplicationFramework.Editor.Outlining;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Imaging;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Native;
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
using IDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace ModernApplicationFramework.Editor.Implementation
{
    // TODO: Add forward backward stuff
    internal abstract class SimpleTextViewWindow : ICommandTarget, ITypedTextTarget,
        ICommandTargetInner, /*IBackForwardNavigation*/ IMafTextView, IConnectionAdviseHelper,
        IReadOnlyViewNotification, ITextEditorPropertyCategoryContainer
    {
        internal static bool _disableSettingImeCompositionWindowOptions = false;
        internal static string ClipboardLineBasedCutCopyTag = "EditorOperationsLineCutCopyClipboardTag";
        internal static string BoxSelectionCutCopyTag = "ColumnSelect";

        private static int _execCount = 0;
        private static int _innerExecCount = 0;
        private static int _insertCharCount = 0;
        public EventHandler TextViewHostUpdated;

        internal bool _canCaretAndSelectionMapToDataBuffer = true;


        internal IEditorOperations _editorOperations;

        internal IEditorOptions _editorOptions;
        internal int _oldHorzFirstVisibleUnit = -1;
        internal int _oldHorzMaxUnits = -1;
        internal int _oldHorzVisibleUnits = -1;
        internal int _oldVerticalFirstVisibleUnit = -1;

        internal int _oldVerticalMaxUnits = -1;
        internal int _oldVerticalVisibleUnits = -1;

        internal ITextViewFilter _textViewFilter;
        internal ITextViewHost _textViewHostPrivate;

        internal ITagAggregator<IUrlTag> _urlTagAggregator;

        internal ViewMarkerTypeManager ViewMarkerTypeManager;

        protected ConnectionPointContainer connectionPointContainerHelper;

        private int? _backgroundColorIndex;
        private bool _canChangeGlyphMarginEnabled;
        private bool _canChangeHotURLs;
        private bool _canChangeIndentStyle;
        private bool _canChangeOvertypeMode;
        private bool _canChangeSelectionMarginEnabled;
        private bool _canChangeShowHorizontalScrollBar;
        private bool _canChangeShowVerticalScrollBar;
        private bool _canChangeTrackChanges;
        private bool _canChangeUseVirtualSpace;
        private bool _canChangeVisibleWhitespace;
        private bool _canChangeWordWrap;
        private IClassificationFormatMap _classificationFormatMap;
        private CancellationTokenSource _codingConventionsCTS = new CancellationTokenSource();

        private CommandChainNode _commandChain;
        private bool _delayedTextBufferLoad;
        private EditorAndMenuFocusTracker _editorAndMenuFocusTracker;
        private bool _eventsInitialized;

        private FontsAndColorsCategory _fontsAndColorsCategory = new FontsAndColorsCategory(
            ImplGuidList.GuidDefaultFileType, CategoryGuids.GuidTextEditorGroup, CategoryGuids.GuidTextEditorGroup);

        private TextViewInitFlags _initFlags;
        private ITextViewRoleSet _initialRoles;

        private readonly List<IObscuringTip> _openedTips = new List<IObscuringTip>();
        private IAccurateOutliningManager _outliningManager;

        private string _plainTextFont;

        private readonly List<IReadOnlyViewNotification> _readOnlyNotifications = new List<IReadOnlyViewNotification>();
        private IScrollMap _scrollMap;
        private bool _sentTextViewCreatedNotifications;
        private bool _startNewClipboardCycle = true;

        private IViewPrimitives _textViewPrimitivesPrivate;
        private DispatcherTimer _tipDimmingTimer;
        private FrameworkElement _zoomControl;
        internal int MaxBraceMatchLinesToSearch;
        private bool _isCodeWindow;
        private bool _canChangeLineNumberMarginEnabled;
        private bool _canChangeCutOrCopyBlankLines;
        private bool _canChangeConvertTabsToSpace;
        private bool _canChangeTabSize;
        private bool _canChangeIndentSize;
        private bool _canChangeEncoding;
        private bool _canChangeIsDefaultCodeExpandSelectionEnabled;
        private bool _canChangeBraceCompletion;
        private bool _canChangeOverviewWidth;
        private bool _canChangeShowPreview;
        private bool _canChangeUseMapMode;
        private bool _canChangeShowCaretPosition;
        private bool _canChangeShowChanges;
        private bool _canChangeShowErrors;
        private bool _canChangeShowMarks;
        private bool _canChangeLineEnding;
        private bool _canChangeRequireFinalNewline;
        private bool _canChangeAllowTrailingWhitespace;
        private bool _canChangeHighlightCurrentLineEnabled;
        private bool _canChangeShowAnnotations;

        public delegate void ChangeScrollInfoEventHandler(IMafTextView pView, int iBar, int iMinUnit, int iMaxUnits,
            int iVisibleUnits, int iFirstVisibleUnit);

        public delegate void KillFocusEventHandler(IMafTextView pView);

        public delegate void SetFocusEventHandler(IMafTextView pView);

        public event EventHandler Initialized;
        public event ChangeScrollInfoEventHandler OnChangeScrollInfo;

        public event KillFocusEventHandler OnKillFocus;

        public event SetFocusEventHandler OnSetFocus;


        public InitializationState CurrentInitializationState { get; internal set; }

        public IndentStyle IndentStyle { get; internal set; }


        public bool InProvisionalInput { get; set; }

        public bool IsIncrementalSearchInProgress { get; set; }

        public bool IsTextViewHostAccessible
        {
            get
            {
                if (CurrentInitializationState != InitializationState.TextBufferAvailable)
                    return CurrentInitializationState >= InitializationState.TextViewCreatedButNotInitialized;
                return true;
            }
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

        internal ITextSnapshot DataTextSnapshot => TextDocData.GetCurrentSnapshot();

        internal FontsAndColorsCategory FontsAndColorsCategory
        {
            get => _fontsAndColorsCategory;
            set
            {
                _fontsAndColorsCategory = value;
                if (CurrentInitializationState >= InitializationState.TextViewAvailable &&
                    _editorOptions.GetOptionValue(DefaultViewOptions.AppearanceCategory) != value.AppearanceCategory)
                    _editorOptions.SetOptionValue(DefaultViewOptions.AppearanceCategory, value.AppearanceCategory);
                if (CurrentInitializationState < InitializationState.TextBufferAvailable || TextView == null)
                    return;
                EditorFormatMap = IoC.Get<IEditorFormatMapService>().GetEditorFormatMap(TextView);
                ApplyBackgroundColor();
            }
        }

        internal VirtualSnapshotPoint? CaretInDataSnapshot =>
            DataPointFromViewPoint(TextView.Caret.Position.VirtualBufferPosition);

        internal bool RaiseGoBackEvents { get; set; }

        internal TextViewShimHost ShimHost { get; set; }

        internal bool SupressUpdateStatusBarEvents { get; set; }

        internal TextDocData TextDocData { get; set; }

        internal IViewPrimitives TextViewPrimitives
        {
            get
            {
                if (_textViewPrimitivesPrivate == null)
                {
                    Init_OnActivation();
                    if (_textViewPrimitivesPrivate == null)
                        throw new InvalidOperationException(
                            "SimpleTextViewWindow initialization hasn't initialized text view primitives!");
                }

                return _textViewPrimitivesPrivate;
            }
            set => _textViewPrimitivesPrivate = value;
        }

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

        private Guid ContextMenuId { get; set; } = Guid.Empty;

        private IEditorFormatMap EditorFormatMap { get; set; }

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

        public int AddCommandFilter(ICommandTarget pNewCmdTarg, out ICommandTarget ppNextCmdTarg)
        {
            return AddCommandFilter(pNewCmdTarg, out ppNextCmdTarg, false);
        }


        public bool Advise(Type eventType, object eventSink)
        {
            throw new NotImplementedException();
        }

        public int SetSelection(int iAnchorLine, int iAnchorCol, int iEndLine, int iEndCol)
        {
            Init_OnActivation();
            if (!TextConvert.TryToVirtualSnapshotPoint(DataTextSnapshot, iAnchorLine, iAnchorCol,
                    out var virtualPoint1) ||
                !TextConvert.TryToVirtualSnapshotPoint(DataTextSnapshot, iEndLine, iEndCol, out var virtualPoint2))
                return -2147024809;
            var anchorPoint = ViewPointFromDataPoint(virtualPoint1);
            var activePoint = ViewPointFromDataPoint(virtualPoint2);
            EnsureSpanExpanded(anchorPoint.Position <= activePoint.Position
                ? new SnapshotSpan(anchorPoint.Position, activePoint.Position)
                : new SnapshotSpan(activePoint.Position, anchorPoint.Position));
            _editorOperations.SelectAndMoveCaret(anchorPoint, activePoint, TextView.Selection.Mode,
                EnsureSpanVisibleOptions.None);
            return 0;
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
            try
            {
                if (Fire_KeyPressEvent(true, commandGroup, commandId, input))
                {
                    hr2 = _commandChain.InnerExec(commandGroup, commandId, nCmdexecopt, input, output);
                    Fire_KeyPressEvent(false, commandGroup, commandId, input);
                }
            }
            catch
            {
                // ignored
            }

            return hr2;
        }

        public int GetData(MafUserDataFormat format, out object pvtData)
        {
            if (format == MafUserDataFormat.TextViewHost)
            {
                pvtData = CurrentInitializationState >= InitializationState.TextBufferAvailable ? TextViewHost : null;
                return 0;
            }

            if (format == MafUserDataFormat.TextViewRoles)
            {
                if (_initialRoles != null)
                {
                    pvtData = string.Join(",", _initialRoles);
                }
                else if (EditorParts.TextEditorFactoryService != null)
                {
                    pvtData = string.Join(",", EditorParts.TextEditorFactoryService.DefaultRoles);
                }
                else
                {
                    pvtData = null;
                    return -2147467259;
                }

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

        public void Initialize(IMafTextLines buffer, IntPtr hwndParent, TextViewInitFlags flags)
        {
            Init_Initialize(buffer, hwndParent, flags);
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
                    switch ((MafConstants.EditorCommands) commandId)
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
                        case MafConstants.EditorCommands.Tab:
                            InsertTab();
                            break;
                        case MafConstants.EditorCommands.BackTab:
                            Backtab();
                            break;
                        case MafConstants.EditorCommands.Delete:
                            Delete();
                            break;
                        case MafConstants.EditorCommands.Left:
                            _editorOperations.MoveToPreviousCharacter(false);
                            break;
                        case MafConstants.EditorCommands.LeftExt:
                            _editorOperations.MoveToPreviousCharacter(true);
                            break;
                        case MafConstants.EditorCommands.Right:
                            _editorOperations.MoveToNextCharacter(false);
                            break;
                        case MafConstants.EditorCommands.RightExt:
                            _editorOperations.MoveToNextCharacter(true);
                            break;
                        case MafConstants.EditorCommands.Up:
                            _editorOperations.MoveLineUp(false);
                            break;
                        case MafConstants.EditorCommands.UpExt:
                            if (TextView.Selection.IsEmpty)
                                TextView.Caret.MoveTo(TextView.Caret.Position.VirtualBufferPosition);
                            _editorOperations.MoveLineUp(true);
                            break;
                        case MafConstants.EditorCommands.Down:
                            _editorOperations.MoveLineDown(false);
                            break;
                        case MafConstants.EditorCommands.DownExt:
                            if (TextView.Selection.IsEmpty)
                                TextView.Caret.MoveTo(TextView.Caret.Position.VirtualBufferPosition);
                            _editorOperations.MoveLineDown(true);
                            break;
                        case MafConstants.EditorCommands.Home:
                            _editorOperations.MoveToStartOfDocument(false);
                            break;
                        case MafConstants.EditorCommands.HomeExt:
                            _editorOperations.MoveToStartOfDocument(true);
                            break;
                        case MafConstants.EditorCommands.End:
                            _editorOperations.MoveToEndOfDocument(false);
                            break;
                        case MafConstants.EditorCommands.EndExt:
                            _editorOperations.MoveToEndOfDocument(true);
                            break;
                        case MafConstants.EditorCommands.BeginOfLine:
                            _editorOperations.MoveToHome(false);
                            break;
                        case MafConstants.EditorCommands.BeginOfLineExt:
                            _editorOperations.MoveToHome(true);
                            break;
                        case MafConstants.EditorCommands.FirstChar:
                            _editorOperations.MoveToStartOfLineAfterWhiteSpace(false);
                            break;
                        case MafConstants.EditorCommands.FirstCharExt:
                            _editorOperations.MoveToStartOfLineAfterWhiteSpace(true);
                            break;
                        case MafConstants.EditorCommands.EndOfLine:
                            _editorOperations.MoveToEndOfLine(false);
                            break;
                        case MafConstants.EditorCommands.EndOfLineExt:
                            _editorOperations.MoveToEndOfLine(true);
                            break;
                        case MafConstants.EditorCommands.LastChar:
                            _editorOperations.MoveToLastNonWhiteSpaceCharacter(false);
                            break;
                        case MafConstants.EditorCommands.LastCharExt:
                            _editorOperations.MoveToLastNonWhiteSpaceCharacter(true);
                            break;
                        case MafConstants.EditorCommands.ShowContextMenu:
                            ShowContextMenu(input, ref result);
                            break;
                        case MafConstants.EditorCommands.PageUp:
                            _editorOperations.PageUp(false);
                            break;
                        case MafConstants.EditorCommands.PageUpExt:
                            _editorOperations.PageUp(true);
                            break;
                        case MafConstants.EditorCommands.PageDown:
                            _editorOperations.PageDown(false);
                            break;
                        case MafConstants.EditorCommands.PageDownExt:
                            _editorOperations.PageDown(true);
                            break;
                        case MafConstants.EditorCommands.TopLine:
                            _editorOperations.MoveToTopOfView(false);
                            break;
                        case MafConstants.EditorCommands.TopLineExt:
                            _editorOperations.MoveToTopOfView(true);
                            break;
                        case MafConstants.EditorCommands.BottomLine:
                            _editorOperations.MoveToBottomOfView(false);
                            break;
                        case MafConstants.EditorCommands.BottomLineExt:
                            _editorOperations.MoveToBottomOfView(true);
                            break;
                        case MafConstants.EditorCommands.ScrollUp:
                            _editorOperations.ScrollUpAndMoveCaretIfNecessary();
                            break;
                        case MafConstants.EditorCommands.ScrollDown:
                            _editorOperations.ScrollDownAndMoveCaretIfNecessary();
                            break;
                        case MafConstants.EditorCommands.ScrollPageUp:
                            _editorOperations.ScrollPageUp();
                            break;
                        case MafConstants.EditorCommands.ScrollPageDown:
                            _editorOperations.ScrollPageDown();
                            break;
                        case MafConstants.EditorCommands.ScrollLeft:
                            _editorOperations.ScrollColumnLeft();
                            break;
                        case MafConstants.EditorCommands.ScrollRight:
                            _editorOperations.ScrollColumnRight();
                            break;
                        case MafConstants.EditorCommands.ScrollBottom:
                            _editorOperations.ScrollLineBottom();
                            break;
                        case MafConstants.EditorCommands.ScrollCenter:
                            _editorOperations.ScrollLineCenter();
                            break;
                        case MafConstants.EditorCommands.ScrollTop:
                            _editorOperations.ScrollLineTop();
                            break;
                        case MafConstants.EditorCommands.SelectAll:
                            SelectAll();
                            break;
                        case MafConstants.EditorCommands.TabifySelection:
                            Tabify();
                            break;
                        case MafConstants.EditorCommands.UntabifySelection:
                            Untabify();
                            break;
                        case MafConstants.EditorCommands.MakeLowerCase:
                            _editorOperations.MakeLowercase();
                            break;
                        case MafConstants.EditorCommands.MakeUpperCase:
                            _editorOperations.MakeUppercase();
                            break;
                        case MafConstants.EditorCommands.ToggleCase:
                            _editorOperations.ToggleCase();
                            break;
                        case MafConstants.EditorCommands.Capitalize:
                            _editorOperations.Capitalize();
                            break;
                        case MafConstants.EditorCommands.SwapAnchor:
                            _editorOperations.SwapCaretAndAnchor();
                            break;
                        case MafConstants.EditorCommands.GotoLine:
                            int lineNumber = GetLineNumberFromDialog();
                            GoToLine(lineNumber);
                            break;
                        case MafConstants.EditorCommands.GotoBrace:
                        case MafConstants.EditorCommands.GotoBraceExt:
                            GotoMatchingBrace(54U == commandId);
                            break;
                        case MafConstants.EditorCommands.ToggleOverTypeMode:
                            if (_canChangeOvertypeMode)
                            {
                                _editorOptions.SetOptionValue(DefaultTextViewOptions.OverwriteModeId,
                                    !_editorOptions.GetOptionValue(DefaultTextViewOptions.OverwriteModeId));
                                UpdateToolsOptionsPreferences(DefaultTextViewOptions.OverwriteModeId.Name);
                            }

                            break;
                        case MafConstants.EditorCommands.Copy:
                            Copy();
                            break;
                        case MafConstants.EditorCommands.Cut:
                            Cut();
                            break;
                        case MafConstants.EditorCommands.Paste:
                            Paste();
                            break;
                        default:
                            result = -2147221248;
                            break;
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
            catch (Exception e)
            {
            }

            return -2147221248;
        }

        private void UpdateToolsOptionsPreferences(string optionName)
        {

        }

        private int GetLineNumberFromDialog()
        {
            int lineCount = TextView.TextSnapshot.LineCount;
            int num = TextView.TextSnapshot.GetLineNumberFromPosition(TextView.Caret.Position.BufferPosition) + 1;
            var dataSource = new GotoDialogDataSource
            {
                MinimumLine = 1,
                MaximumLine = lineCount,
                CurrentLine = num
            };
            if (WindowHelper.ShowModalElement(new GotoDialog(), dataSource) == 1)
                return dataSource.CurrentLine - 1;
            return -1;
        }

        public int InnerQueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            if (IsCommandExecutionProhibited())
                return -2147221248;
            if (commandGroup == MafConstants.EditorCommandGroup)
                for (var i = 0; i < cCmds; ++i)
                {
                    switch (prgCmds[i].cmdID)
                    {
                        case 57:
                            prgCmds[i].cmdf = Olecmdf.Supported | Olecmdf.Enabled;
                            if (_editorOptions.GetOptionValue(DefaultTextViewOptions.OverwriteModeId))
                            {
                                prgCmds[i].cmdf |= Olecmdf.Latched;
                            }

                            break;
                        case 60:
                            prgCmds[i].cmdf = CanPaste();
                            break;
                        default:
                            prgCmds[i].cmdf = prgCmds[i].cmdID <= 0 || prgCmds[i].cmdID >= 145
                                ? Olecmdf.None
                                : Olecmdf.Supported | Olecmdf.Enabled;
                            break;
                    }
                }

            return 0;
        }

        public void GoToLine(int lineNumber)
        {
            if (lineNumber < 0)
                return;
            var textSnapshot = _editorOperations.TextView.TextSnapshot;
            lineNumber = Math.Min(lineNumber, textSnapshot.LineCount - 1);
            EnsureSpanExpanded(new SnapshotSpan(textSnapshot.GetLineFromLineNumber(lineNumber).Start, 0));
            _editorOperations.GotoLine(lineNumber);
            _editorOperations.MoveToStartOfLineAfterWhiteSpace(false);
        }

        public int IsReadOnly()
        {
            return _editorOptions.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId) ? 0 : 1;
        }

        public int GetCaretPos(out int piLine, out int piColumn)
        {
            var caretInDataSnapshot = CaretInDataSnapshot;
            if (!caretInDataSnapshot.HasValue)
            {
                InteropHelper.SetOutParameter(out piLine, 0);
                InteropHelper.SetOutParameter(out piColumn, 0);
                return -2147467259;
            }

            var containingLine = caretInDataSnapshot.Value.Position.GetContainingLine();
            InteropHelper.SetOutParameter(out piLine, containingLine.LineNumber);
            InteropHelper.SetOutParameter(out piColumn,
                caretInDataSnapshot.Value.Position - containingLine.Start + caretInDataSnapshot.Value.VirtualSpaces);
            return 0;
        }

        public bool IsSearchingCommand(Guid cmdGroup, uint cmdId)
        {
            if (!IsIncrementalSearchInProgress)
                return false;
            if (cmdGroup == MafConstants.EditorCommandGroup)
                switch ((MafConstants.EditorCommands) cmdId)
                {
                    case MafConstants.EditorCommands.TypeChar:
                    case MafConstants.EditorCommands.Backspace:
                    case MafConstants.EditorCommands.Return:
                        //case VSConstants.VSStd2KCmdID.CANCEL:
                        return true;
                }

            return false;
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

        public int QueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds, IntPtr pCmdText)
        {
            var hr = PreOuterQueryStatus(commandGroup, cCmds, prgCmds);
            return hr < 0 ? hr : _commandChain.QueryStatus(commandGroup, cCmds, prgCmds, pCmdText);
        }

        public int SetCaretPos(int iLine, int iColumn)
        {
            if (!TextConvert.TryToVirtualSnapshotPoint(DataTextSnapshot, iLine, iColumn, out var virtualPoint))
                return -2147024809;
            VirtualSnapshotPoint virtualSnapshotPoint = ViewPointFromDataPoint(virtualPoint);
            EnsureSpanExpanded(new SnapshotSpan(virtualSnapshotPoint.Position, virtualSnapshotPoint.Position));
            _editorOperations.SelectAndMoveCaret(virtualSnapshotPoint, virtualSnapshotPoint, TextView.Selection.Mode,
                EnsureSpanVisibleOptions.MinimumScroll);
            return 0;
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
                case (EditPropId) 1:
                    _canChangeOvertypeMode = true;
                    return 0;
                case (EditPropId) 2:
                    _canChangeVisibleWhitespace = true;
                    return 0;
                default:
                    return -2147024809;
            }
        }

        public int SetBackgroundColorIndex(int iBackgroundIndex)
        {
            BackgroundColorIndex = iBackgroundIndex;
            return 0;
        }

        public int SetBuffer(IMafTextLines pBuffer)
        {
            return Init_SetBuffer(pBuffer);
        }

        public int SetData(MafUserDataFormat format, object vtData)
        {
            if (format == MafUserDataFormat.TextViewRoles)
            {
                if (!(vtData is string))
                    return -2147024809;
                SetInitialRoles(vtData.ToString());
                return 0;
            }

            if (format == MafUserDataFormat.ContextMenuId && vtData is Guid contextMenuId)
            {
                ContextMenuId = contextMenuId;
                return 0;
            }

            return -2147467263;
        }

        public void SetInitialRoles(ITextViewRoleSet roles)
        {
            TextDocData?.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, false);
            _initialRoles = roles;
            TextDocData?.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, true);
        }

        public void SetInitialRoles(string roles)
        {
            SetInitialRoles(EditorParts.TextEditorFactoryService.CreateTextViewRoleSet(roles.Split(',', ' ')));
        }

        public void SetSite()
        {
            Init_SetSite();
        }

        public bool Unadvise(Type eventType, object eventSink)
        {
            throw new NotImplementedException();
        }

        internal static Point CalculateContextMenuPosition(ITextView textView)
        {
            if ((uint) textView.Caret.ContainingTextViewLine.VisibilityState > 0U &&
                textView.Caret.Right >= textView.ViewportLeft && textView.Caret.Right <= textView.ViewportRight)
                return new Point(textView.Caret.Right - textView.ViewportLeft,
                    textView.Caret.Bottom - textView.ViewportTop);
            return new Point(0.0, 0.0);
        }

        internal static unsafe char GetTypeCharFromKeyPressEventArg(IntPtr pvaIn)
        {
            var keyPressEventArgPtr = (KeyPressEventArg*) (void*) pvaIn;
            switch (keyPressEventArgPtr->vt)
            {
                case 2:
                case 18:
                    return keyPressEventArgPtr->ch;
                default:
                    return (char) (ushort) Marshal.GetObjectForNativeVariant(pvaIn);
            }
        }

        internal void ClassificationFormatMap_ClassificationFormatMappingChanged(object sender, EventArgs e)
        {
            SetPlainTextFont();
        }

        internal VirtualSnapshotPoint ViewPointFromDataPoint(VirtualSnapshotPoint dataPoint)
        {
            return new VirtualSnapshotPoint(ViewPointFromDataPoint(dataPoint.Position), dataPoint.VirtualSpaces);
        }

        internal SnapshotPoint ViewPointFromDataPoint(SnapshotPoint dataPoint)
        {
            var snapshot = TextView.BufferGraph.MapUpToSnapshot(dataPoint, PointTrackingMode.Positive,
                PositionAffinity.Successor, TextView.TextSnapshot);
            if (!snapshot.HasValue)
                return new SnapshotPoint(TextView.TextSnapshot, 0);
            return snapshot.Value;
        }

        internal void EnsureSpanExpanded(SnapshotSpan span)
        {
            if (TextView.TextViewModel.IsPointInVisualBuffer(span.Start, PositionAffinity.Successor) &&
                (span.IsEmpty || TextView.TextViewModel.IsPointInVisualBuffer(span.End, PositionAffinity.Predecessor)))
                return;
            EnsureSpansExpanded(new NormalizedSnapshotSpanCollection(span));
        }

        internal void ClearCommandContext()
        {
            if (_textViewHostPrivate == null || _textViewHostPrivate.IsClosed)
                return;
            TextView.Properties.RemoveProperty(typeof(MafConstants.EditorCommands));
        }

        internal SnapshotPoint? DataPointFromViewPoint(SnapshotPoint viewPoint)
        {
            return TextView.BufferGraph.MapDownToSnapshot(viewPoint, PointTrackingMode.Positive, DataTextSnapshot,
                PositionAffinity.Successor);
        }

        internal VirtualSnapshotPoint? DataPointFromViewPoint(VirtualSnapshotPoint viewPoint)
        {
            var nullable = DataPointFromViewPoint(viewPoint.Position);
            if (!nullable.HasValue)
                return new VirtualSnapshotPoint?();
            return new VirtualSnapshotPoint(nullable.Value, viewPoint.VirtualSpaces);
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

        internal void SetCommandContext(MafConstants.EditorCommands command)
        {
            TextView.Properties[typeof(MafConstants.EditorCommands)] = command;
        }

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

        internal int SetPropertyInPropertyContainer(EditPropId idProp, object pvar)
        {
            switch (idProp)
            {
                case EditPropId.ViewLangOptVirtualSpace:
                    if (!(pvar is bool vflag))
                        return -2147024809;
                    if (vflag && (_editorOptions.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId) &
                                  WordWrapStyles.WordWrap) != WordWrapStyles.None)
                        return -2147467259;
                    _canChangeUseVirtualSpace = false;
                    _editorOptions.SetOptionValue(DefaultTextViewOptions.UseVirtualSpaceId, vflag);
                    return 0;
                case EditPropId.ViewLangOptWordWrap:
                    if (!(pvar is bool wflag))
                        return -2147024809;
                    _canChangeWordWrap = false;
                    if (wflag)
                    {
                        if (_editorOptions.GetOptionValue(DefaultTextViewOptions.UseVirtualSpaceId))
                            return -2147467259;
                        _editorOptions.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId,
                            _editorOptions.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId) |
                            WordWrapStyles.WordWrap);
                    }
                    else
                        _editorOptions.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId,
                            _editorOptions.GetOptionValue(DefaultTextViewOptions.WordWrapStyleId) &
                            ~WordWrapStyles.WordWrap);

                    return 0;
                case EditPropId.ViewGlobalOptAutoScrollCaretOnTextEntry:
                    _editorOptions.SetOptionValue(DefaultTextViewOptions.AutoScrollId, (bool) pvar);
                    return 0;
                case EditPropId.ViewGlobalOptSelectionMargin:
                    if (!(pvar is bool sflag))
                        return -2147024809;
                    _canChangeSelectionMarginEnabled = false;
                    _editorOptions.SetOptionValue(DefaultTextViewHostOptions.SelectionMarginId, sflag);
                    return 0;
                case EditPropId.ViewGlobalOptOvertype:
                    if (!(pvar is bool oflag))
                        return -2147024809;
                    _canChangeOvertypeMode = false;
                    _editorOptions.SetOptionValue(DefaultTextViewOptions.OverwriteModeId, oflag);
                    return 0;
                case EditPropId.ViewGeneralFontCategory:
                    FontsAndColorsCategory = FontsAndColorsCategory.SetFontCategory(new Guid(pvar.ToString()));
                    return 0;
                case EditPropId.ViewGeneralColorCategory:
                    FontsAndColorsCategory = FontsAndColorsCategory.SetColorCategory(new Guid(pvar.ToString()));
                    return 0;
                case EditPropId.ViewCompositeAllCodeWindowDefaults:
                    SetPropertiesToCodeWindowDefaults();
                    return 0;
                default:
                    return -2147024809;
            }
        }

        internal void SetPropertiesToCodeWindowDefaults()
        {
            _isCodeWindow = true;
            FontsAndColorsCategory = new FontsAndColorsCategory(ImplGuidList.GuidDefaultFileType,
                DefGuidList.TextEditorCategory, DefGuidList.TextEditorCategory);
            _canChangeUseVirtualSpace = true;
            _canChangeOvertypeMode = true;
            _canChangeTrackChanges = true;
            _canChangeGlyphMarginEnabled = true;
            _canChangeSelectionMarginEnabled = true;
            _canChangeLineNumberMarginEnabled = true;
            _canChangeCutOrCopyBlankLines = true;
            _canChangeWordWrap = true;
            _canChangeIndentStyle = true;
            _canChangeConvertTabsToSpace = true;
            _canChangeTabSize = true;
            _canChangeIndentSize = true;
            _canChangeEncoding = true;
            _canChangeLineEnding = true;
            _canChangeRequireFinalNewline = true;
            _canChangeAllowTrailingWhitespace = true;
            _canChangeVisibleWhitespace = true;
            _canChangeHotURLs = true;
            _canChangeHighlightCurrentLineEnabled = true;
            _canChangeShowHorizontalScrollBar = true;
            _canChangeShowVerticalScrollBar = true;
            _canChangeShowAnnotations = true;
            _canChangeShowChanges = true;
            _canChangeShowMarks = true;
            _canChangeShowErrors = true;
            _canChangeShowCaretPosition = true;
            _canChangeUseMapMode = true;
            _canChangeShowPreview = true;
            _canChangeOverviewWidth = true;
            _canChangeBraceCompletion = true;
            _canChangeIsDefaultCodeExpandSelectionEnabled = true;
            RaiseGoBackEvents = true;
            SupressUpdateStatusBarEvents = false;
            IndentStyle = IndentStyle.None;
            _editorOptions.SetOptionValue(DefaultTextViewOptions.UseVirtualSpaceId, false);
            _editorOptions.SetOptionValue(DefaultTextViewOptions.OverwriteModeId, false);
            _editorOptions.SetOptionValue(DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId, true);
            _editorOptions.SetOptionValue(DefaultTextViewHostOptions.GlyphMarginId, true);
            _editorOptions.SetOptionValue(DefaultTextViewHostOptions.SelectionMarginId, true);
            //_editorOptions.SetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId, false);
            _editorOptions.SetOptionValue(DefaultTextViewHostOptions.HorizontalScrollBarId, true);
            _editorOptions.SetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId, true);
            _editorOptions.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.AutoIndent);
            _editorOptions.SetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId, false);
            _editorOptions.SetOptionValue(DefaultTextViewOptions.UseVisibleWhitespaceId, false);
            _editorOptions.SetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId, true);
            _editorOptions.SetOptionValue(DefaultViewOptions.EnableHighlightCurrentLineId, true);
            _editorOptions.SetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId, true);
            TextDocData?.EditorOptions?.SetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId, true);
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

        internal void EnsureSpansExpanded(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0 || _outliningManager == null)
                return;
            var span1 = spans[0];
            var start = span1.Start;
            span1 = spans[spans.Count - 1];
            var end = span1.End;
            var total = new SnapshotSpan(start, end);
            var singlePoint = new SnapshotPoint?();
            if (spans.Count == 1)
            {
                span1 = spans[0];
                if (span1.Length == 0)
                {
                    span1 = spans[0];
                    singlePoint = span1.Start;
                }
            }

            _outliningManager.ExpandAll(total, collapsible =>
            {
                var span2 = collapsible.Extent.GetSpan(total.Snapshot);
                if (spans.OverlapsWith(span2))
                    return true;
                if (singlePoint.HasValue && singlePoint.Value > span2.Start)
                    return singlePoint.Value < span2.End;
                return false;
            });
        }

        protected virtual void InitializeConnectionPoints()
        {
            connectionPointContainerHelper = new ConnectionPointContainer(this);
            connectionPointContainerHelper.AddEventType<IMafTextViewEvents>();
        }

        protected virtual bool ClipboardContainsTextOrHtml()
        {
            return User32.GetPriorityClipboardFormat(new[]
            {
                1U,
                13U,
                ClipboardDataFormats.HTMLFormatId
            }, 3) > 0;
        }

        private static void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            CanExecuteApplicationCommand(MafConstants.EditorCommands.Copy, sender, e);
        }

        private static void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            CanExecuteApplicationCommand(MafConstants.EditorCommands.Cut, sender, e);
        }

        private static void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            CanExecuteApplicationCommand(MafConstants.EditorCommands.Paste, sender, e);
        }

        private static void CanExecuteApplicationCommand(MafConstants.EditorCommands command, object sender,
            CanExecuteRoutedEventArgs e)
        {
            var guid = MafConstants.EditorCommandGroup;
            var prgCmds = new[]
            {
                new Olecmd {cmdID = (uint) command}
            };

            if (!(sender is IPropertyOwner textView))
                return;
            var target = GetTarget(textView);

            target?.QueryStatus(guid, (uint) prgCmds.Length, prgCmds, IntPtr.Zero);
            e.CanExecute = prgCmds[0].cmdf.HasFlag(Olecmdf.Supported) && prgCmds[0].cmdf.HasFlag(Olecmdf.Enabled);
        }

        private static bool ContentTypeMatch(IContentType documentContentType,
            IEnumerable<string> extensionContentTypes)
        {
            return documentContentType != null && extensionContentTypes.Any(documentContentType.IsOfType);
        }

        private static int DefaultErrorHandler(int report, ref Guid pguidCmdGroup)
        {
            if (report >= 0 || !(pguidCmdGroup == MafConstants.EditorCommandGroup))
                return report;
            report = 0;
            return report;
        }

        private static void FailInitializationAndThrow(string message)
        {
            throw new InvalidOperationException(message);
        }

        private static ICommandTarget GetTarget(IPropertyOwner textView)
        {
            return textView.Properties.GetProperty(typeof(ICommandTarget)) as ICommandTarget;
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

        private static void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            OnApplicationCommand(MafConstants.EditorCommands.Copy, sender, e);
        }

        private static void OnCut(object sender, ExecutedRoutedEventArgs e)
        {
            OnApplicationCommand(MafConstants.EditorCommands.Cut, sender, e);
        }

        private static void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            OnApplicationCommand(MafConstants.EditorCommands.Paste, sender, e);
        }

        private void Paste()
        {
            object dataObj = null;
            try
            {
                OleGetClipboard(out dataObj);
            }
            catch (ExternalException)
            {
            }
            catch (OutOfMemoryException)
            {
            }

            Paste((System.Runtime.InteropServices.ComTypes.IDataObject) dataObj);
        }

        private bool Paste(System.Runtime.InteropServices.ComTypes.IDataObject dataObject)
        {

            //TODO: undo stuff
            //using (var transaction = _undoManager.TextBufferUndoHistory.CreateTransaction(Strings.Paste))
            //{
            _editorOperations.AddBeforeTextBufferChangePrimitive();
            var currentSnapshot = TextDocData.DataTextBuffer.CurrentSnapshot;
            var flag = true;
            try
            {
                if (dataObject != null)
                {
                    if ((GetDataPresent(dataObject, ClipboardDataFormats.ClipboardLineBasedCutCopyTagId)
                            ? 1
                            : (GetDataPresent(dataObject, ClipboardDataFormats.BoxSelectionCutCopyTagId) ? 1 : 0)) == 0)
                    {
                        if (DataObjectHelper.ContainsText(dataObject, TextDocData.ActualLanguageServiceID, TextDocData))
                        {
                            string textToInsert = DataObjectHelper.GetText(dataObject, TextDocData.ActualLanguageServiceID, TextDocData);
                            if (!string.IsNullOrEmpty(textToInsert))
                            {
                                PasteTextAndAddGoBackLocations(() => _editorOperations.InsertText(textToInsert));
                                flag = false;
                            }
                        }
                    }
                }
            }
            catch (ExternalException)
            {
            }
            catch (OutOfMemoryException)
            {
            }
            if (flag)
                PasteTextAndAddGoBackLocations(() => _editorOperations.Paste());
            if (currentSnapshot != TextDocData.DataTextBuffer.CurrentSnapshot)
            {
                //if (service != null)
                //{
                //    INormalizedTextChangeCollection changes = currentSnapshot.Version.Changes;
                //    TextSpan[] ptsInsertedText = new TextSpan[1]
                //    {
                //        TextConvert.ToVsTextSpan(currentSnapshot.Version.Next.CreateTrackingSpan(Span.FromBounds(changes[0].NewSpan.Start, changes[changes.Count - 1].NewSpan.End), SpanTrackingMode.EdgeInclusive).GetSpan(this._textDocData.DataTextBuffer.CurrentSnapshot))
                //    };
                //    service.DataObjectRendered((IVsTextLines)this._textDocData, 4U, ptsInsertedText);
                //}
                _editorOperations.AddAfterTextBufferChangePrimitive();
                //transaction.Complete();
                return true;
            }
            //transaction.Cancel();
            return false;
            //}
        }

        private void PasteTextAndAddGoBackLocations(Func<bool> pasteAction)
        {
            var start = TextView.Selection.Start;
            int lengthForVirtualSpace = GetWhitespaceLengthForVirtualSpace(start);
            if (!pasteAction())
                return;
            //TODO: go back stuff
            //SetGoBackMarker(Math.Min(start.Position.Position + lengthForVirtualSpace, TextView.TextSnapshot.Length),
            //    CaretMoveType.DestructiveCaretMove | CaretMoveType.NonMergeable, GoBackFlags.ForceMerge);
            //SetGoBackMarker(TextViewPrimitives.Caret.CurrentPosition,
            //    CaretMoveType.DestructiveCaretMove | CaretMoveType.NonMergeable, GoBackFlags.ForceAdd);
        }

        private int GetWhitespaceLengthForVirtualSpace(VirtualSnapshotPoint point)
        {
            if (!point.IsInVirtualSpace)
                return 0;
            var virtualSpaces = point.VirtualSpaces;
            if (TextView.Options.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId))
                return virtualSpaces;
            var optionValue = TextView.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);
            var displayColumn = TextViewPrimitives.View.GetTextPoint(point.Position).DisplayColumn;
            var num1 = displayColumn + virtualSpaces;
            var num2 = num1 % optionValue;
            return num2 + (num1 - num2 - displayColumn + optionValue - 1) / optionValue;
        }

        private static bool GetDataPresent(IDataObject dataObject,
            ushort dataFormat)
        {
            var format = new FORMATETC
            {
                cfFormat = (short) dataFormat, dwAspect = DVASPECT.DVASPECT_CONTENT, lindex = -1, tymed = 0
            };
            return dataObject.QueryGetData(ref format) == 0;
        }

        protected virtual int OleGetClipboard(out object dataObj)
        {
            return Ole32.OleGetClipboard(out dataObj);
        }


        private Olecmdf CanPaste()
        {
            var result = Olecmdf.Supported;
            if (!DoesViewProhibitUserInput() &&
                (ClipboardContainsTextOrHtml()
                 //TODO: text manager stuff
                 //|| DataObjectHelper.IsClipboardDataSupportedByLanguageService(_serviceProvider, _textManager, TextDocData) 
                 || _editorOperations.CanPaste))
                result |= Olecmdf.Enabled;
            return result;
        }

        private bool DoesViewProhibitUserInput()
        {
            return _editorOperations.Options.GetOptionValue(DefaultTextViewOptions.ViewProhibitUserInputId);
        }


        private static void OnApplicationCommand(MafConstants.EditorCommands command, object sender,
            ExecutedRoutedEventArgs e)
        {
            if (!(sender is IPropertyOwner textView))
                return;
            var target = GetTarget(textView);
            target?.Exec(MafConstants.EditorCommandGroup, (uint) command, 0, IntPtr.Zero,
                IntPtr.Zero);
        }

        private static List<Lazy<TProvider, TMetadataView>> SelectMatchingExtensions<TProvider, TMetadataView>(
            IEnumerable<Lazy<TProvider, TMetadataView>> providerHandles, IContentType documentContentType,
            ITextViewRoleSet viewRoles) where TMetadataView : IContentTypeAndTextViewRoleMetadata
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

        private void ApplyBackgroundColor()
        {
            if (CurrentInitializationState < InitializationState.TextViewAvailable)
                return;
            var backgroundColorIndex = BackgroundColorIndex;
            if (!backgroundColorIndex.HasValue || TextDocData == null || TextDocData.MarkerManager == null)
                return;
            var nullable = new Color?();
            var editorFormatMap = EditorFormatMap;
            var markerManager = TextDocData.MarkerManager;
            backgroundColorIndex = BackgroundColorIndex;
            var type = backgroundColorIndex.Value;
            var mergeName = markerManager.GetMarkerType(type).MergeName;
            var properties = editorFormatMap.GetProperties(mergeName);
            if (properties.Contains("BackgroundColor"))
                nullable = properties["BackgroundColor"] as Color?;
            if (!nullable.HasValue && properties.Contains("Background"))
                if (properties["Background"] is SolidColorBrush solidColorBrush)
                    nullable = solidColorBrush.Color;
            if (!nullable.HasValue)
                return;
            var solidColorBrush1 = new SolidColorBrush(nullable.Value);
            solidColorBrush1.Freeze();
            TextView.Background = solidColorBrush1;
        }

        private void Backsapce()
        {
            if (_editorOperations.ProvisionalCompositionSpan != null)
                _editorOperations.InsertText("");
            else
                _editorOperations.Backspace();
        }

        private void CleanUpEvents()
        {
            if (_eventsInitialized)
            {
                var textView = _textViewHostPrivate.TextView;
                //textView.Caret.PositionChanged -= Caret_PositionChanged;
                //textView.Selection.SelectionChanged -= Selection_SelectionChanged;
                textView.LayoutChanged -= View_LayoutChanged;
                Keyboard.RemoveKeyDownHandler(textView.VisualElement, OnTextView_KeyDown);
                Keyboard.RemoveKeyUpHandler(textView.VisualElement, OnTextView_KeyUp);
                //textView.ViewportLeftChanged -= View_ViewportLeftChanged;
                //textView.ViewportWidthChanged -= View_ViewportWidthChanged;
                //_editorOptions.OptionChanged -= EditorOptions_OptionChanged;

                textView.TextBuffer.Changed -= DocData_OnChangeLineText;
                //    //TODO: Add textDocData stuff
                _editorAndMenuFocusTracker.GotFocus -= OnEditorOrMenuGotFocus;
                _editorAndMenuFocusTracker.LostFocus -= OnEditorOrMenuLostFocus;
                _editorAndMenuFocusTracker = null;
                if (_zoomControl != null)
                {
                    _zoomControl.IsKeyboardFocusWithinChanged -= OnZoomIsKeyboardFocusWithinChanged;
                    _zoomControl = null;
                }

                EditorParts.EditorFormatMapService.GetEditorFormatMap(TextView).FormatMappingChanged -=
                    OnFormatMapChanged;
                _classificationFormatMap.ClassificationFormatMappingChanged -=
                    ClassificationFormatMap_ClassificationFormatMappingChanged;
            }

            _eventsInitialized = false;
            _scrollMap = null;
        }

        private void CloseBufferRelatedResources()
        {
            TextDocData.EndTemplateEditing();
            TextDocData.TextBufferInitialized -= Init_OnTextBufferInitialized;
            TextDocData.OnLoadCompleted -= Init_OnTextBufferLoaded;

            TextDocData.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, false);

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

        private void Copy()
        {
            _editorOperations.CopySelection();
        }

        private void Cut()
        {
            _editorOperations.CutSelection();
        }

        private void DocData_OnChangeLineText(object sender, TextContentChangedEventArgs e)
        {
            //ClearBraceHighlighing();
        }

        private bool Fire_KeyPressEvent(bool isPreEvent, Guid commandGroup, uint commandId, IntPtr înput)
        {
            if (commandGroup == MafConstants.EditorCommandGroup)
                switch (commandId)
                {
                    case (uint) MafConstants.EditorCommands.TypeChar:
                        return Fire_KeyPressEvent(isPreEvent, GetTypeCharFromKeyPressEventArg(înput));
                    case (uint) MafConstants.EditorCommands.Backspace:
                        return Fire_KeyPressEvent(isPreEvent, '\b');
                    case (uint) MafConstants.EditorCommands.Return:
                        //case VSConstants.VSStd2KCmdID.OPENLINEABOVE:
                        return Fire_KeyPressEvent(isPreEvent, '\r');
                    case (uint) MafConstants.EditorCommands.Tab:
                    case (uint) MafConstants.EditorCommands.BackTab:
                        return Fire_KeyPressEvent(isPreEvent, '\t');
                    case (uint) MafConstants.EditorCommands.Delete:
                        return Fire_KeyPressEvent(isPreEvent, '\x007F');
                }
            return true;
        }

        private bool Fire_KeyPressEvent(bool isPreEvent, char key)
        {
            //TODO: Text Manager stuff
            return true;
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
                iVisibleUnits = (int) (TextView.ViewportHeight / 14.0);
                iFirstVisibleUnit = TextView.TextViewLines.FirstVisibleLine.Start.GetContainingLine().LineNumber;
            }
            else
            {
                iMaxUnits = (int) (_scrollMap.End - _scrollMap.Start);
                iVisibleUnits = (int) _scrollMap.ThumbSize;
                iFirstVisibleUnit =
                    (int) _scrollMap.GetCoordinateAtBufferPosition(TextView.TextViewLines.FirstVisibleLine.Start);
            }

            if (_oldVerticalMaxUnits == iMaxUnits && _oldVerticalVisibleUnits == iVisibleUnits &&
                _oldVerticalFirstVisibleUnit == iFirstVisibleUnit)
                return;
            _oldVerticalMaxUnits = iMaxUnits;
            _oldVerticalVisibleUnits = iVisibleUnits;
            _oldVerticalFirstVisibleUnit = iFirstVisibleUnit;
            OnChangeScrollInfo?.Invoke(this, 1, iMinUnit, iMaxUnits, iVisibleUnits, iFirstVisibleUnit);
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

        private void Init_Initialize(IMafTextLines buffer, IntPtr hwndParent, TextViewInitFlags flags)
        {
            if (CurrentInitializationState != InitializationState.Sited)
                FailInitializationAndThrow("Initialize is being called before the adapter has been sited.");
            _initFlags = flags;
            ShimHost.Initialize(((int) flags & 32768) == 0, hwndParent);
            ShimHost.NowVisible += ShimHostNowVisible;
            Init_SetBuffer(buffer);
        }

        private void Init_InitializeWpfTextView()
        {
            CurrentInitializationState = InitializationState.TextViewInitializing;
            if (_initialRoles == null)
                _initialRoles = EditorParts.TextEditorFactoryService.DefaultRoles;
            SetViewOptions();
            var andColorsCategory = FontsAndColorsCategory.SetLanguageService(TextDocData.ActualLanguageServiceID);
            _editorOptions.SetOptionValue(DefaultViewOptions.AppearanceCategory, andColorsCategory.AppearanceCategory);

            if (_textViewHostPrivate != null)
                return;

            var textView = EditorParts.TextEditorFactoryService.CreateTextViewWithoutInitialization(
                TextDocData.TextDataModel, _initialRoles, _editorOptions);
            var textViewHost =
                EditorParts.TextEditorFactoryService.CreateTextViewHostWithoutInitialization(textView, false);
            _editorOptions = textView.Options;
            EmbeddedObjectHelper.SetOleCommandTarget(textViewHost.HostControl, this);
            //TODO: UserContext Stuff
            _textViewHostPrivate = textViewHost;
            textView.Properties.AddProperty(typeof(IMafTextView), this);
            textView.Properties.AddProperty(typeof(ICommandTarget), this);

            CurrentInitializationState = InitializationState.TextViewCreatedButNotInitialized;
            EditorParts.TextEditorFactoryService.InitializeTextView(textView);
            EditorParts.TextEditorFactoryService.InitializeTextViewHost(textViewHost);
            var handlerServiceFilter = new CommandHandlerServiceFilter(this);
            Marshal.ThrowExceptionForHR(AddCommandFilter(handlerServiceFilter, out var ppNextCmdTarg));
            handlerServiceFilter.Initialize(ppNextCmdTarg);
            CurrentInitializationState = InitializationState.TextViewAvailable;
            ViewLoadedHandler.OnViewCreated(_textViewHostPrivate);
            ThemeTextViewHostScrollBars(_textViewHostPrivate);
            CommandRouting.SetInterceptsCommandRouting(textView.VisualElement, false);
            _outliningManager =
                EditorParts.OutliningManagerService.GetOutliningManager(textView) as IAccurateOutliningManager;
            if (_outliningManager != null)
                new HiddenTextSessionCoordinator(this, textView, _outliningManager, _editorOptions, TextDocData);
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
                    _editorOperations.SelectAndMoveCaret(virtualSnapshotPoint, virtualSnapshotPoint,
                        TextSelectionMode.Stream);
                }
            }

            //TODO: TextManager stuff
            //if (!_isViewRegistered && )
            if (!_sentTextViewCreatedNotifications)
            {
                _sentTextViewCreatedNotifications = true;
                SendTextViewCreated();
                if (_textViewHostPrivate.TextView.Properties.TryGetProperty<object>("OverviewMarginContextMenu",
                    out var property))
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

            //StatusBar Stuff
            Initialized?.Invoke(this, EventArgs.Empty);
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
            TextDocData = TextDocData.GetDocDataFromMafTextBuffer(pBuffer);
            if (TextDocData == null)
                throw new ArgumentException("Could not find adapter for the given buffer", nameof(pBuffer));
            TextDocData.UpdateNumberOfViewsWithPrimaryDocumentRole(_initialRoles, true);
            CurrentInitializationState = InitializationState.TextDocDataAvailable;
            if (TextDocData.InitializedDocumentTextBuffer)
            {
                Init_OnTextBufferInitialized(null, null);
                Init_OnTextBufferLoaded();
            }

            TextDocData.TextBufferInitialized += Init_OnTextBufferInitialized;
            TextDocData.OnLoadCompleted += Init_OnTextBufferLoaded;


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

                _classificationFormatMap.ClassificationFormatMappingChanged +=
                    ClassificationFormatMap_ClassificationFormatMappingChanged;
                SetPlainTextFont();
            }
            finally
            {
                _eventsInitialized = true;
            }
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

        private void InsertNewLine()
        {
            _editorOperations.InsertNewLine();
        }

        private bool IsCommandExecutionProhibited()
        {
            if (CurrentInitializationState == InitializationState.TextDocDataAvailable ||
                CurrentInitializationState == InitializationState.TextBufferAvailable ||
                CurrentInitializationState == InitializationState.TextViewAvailable)
                return TextDocData.IsClosed;
            return true;
        }

        private bool IsEditingCommand(Guid commandGroup, uint commandId)
        {
            if (commandGroup == MafConstants.EditorCommandGroup)
                switch ((MafConstants.EditorCommands) commandId)
                {
                    case MafConstants.EditorCommands.TypeChar:
                    case MafConstants.EditorCommands.Backspace:
                    case MafConstants.EditorCommands.Return:
                    case MafConstants.EditorCommands.Tab:
                    case MafConstants.EditorCommands.BackTab:
                    case MafConstants.EditorCommands.Delete:
                    case MafConstants.EditorCommands.TabifySelection:
                    case MafConstants.EditorCommands.UntabifySelection:
                    case MafConstants.EditorCommands.MakeLowerCase:
                    case MafConstants.EditorCommands.MakeUpperCase:
                    case MafConstants.EditorCommands.ToggleCase:
                    case MafConstants.EditorCommands.Capitalize:
                    case MafConstants.EditorCommands.ToggleOverTypeMode:
                    case MafConstants.EditorCommands.Cut:
                    case MafConstants.EditorCommands.Paste:
                        return true;
                    default:
                        return false;
                }

            return false;
        }

        private bool IsViewOrBufferReadOnly()
        {
            return IsReadOnly() == 0 || (TextDocData.TextBufferState & 1) != 0;
        }

        private void OnEditorOrMenuGotFocus(object sender, EventArgs e)
        {
            var onSetFocus = OnSetFocus;
            onSetFocus?.Invoke(this);
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

        private void OnFormatMapChanged(object sender, FormatItemsEventArgs e)
        {
            if (!e.ChangedItems.Contains("TextView Background"))
                return;
            ApplyBackgroundColor();
        }

        private void OnTextView_KeyDown(object sender, KeyEventArgs e)
        {
            if (_openedTips.Count <= 0 || !IsDimmingKey(e))
                return;
            if (_tipDimmingTimer == null)
            {
                _tipDimmingTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 250)};
                _tipDimmingTimer.Tick += (tickSender, tickArgs) => SetTipOpacity(0.3);
            }

            _tipDimmingTimer.Start();
        }

        private void OnTextView_KeyUp(object sender, KeyEventArgs e)
        {
            if (!IsDimmingKey(e))
                return;
            SetTipOpacity(1.0);
        }

        private void OnZoomIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_zoomControl.IsKeyboardFocusWithin)
                return;
            Keyboard.Focus(TextView.VisualElement);
            //TODO:
        }

        private int PreOuterQueryStatus(Guid commandGroup, uint cCmds, Olecmd[] prgCmds)
        {
            if (IsCommandExecutionProhibited())
                return -2147221248;
            Init_OnActivation();
            if (TextViewHost.HostControl.IsKeyboardFocusWithin)
                if (Keyboard.FocusedElement is DependencyObject focusedElement &&
                    CommandRouting.GetInterceptsCommandRouting(focusedElement))
                {
                    var flag = false;
                    for (var index = 0; index < cCmds; ++index)
                        if (TextDocData.IsCommandSupported(commandGroup, prgCmds[index].cmdID))
                        {
                            flag = true;
                            prgCmds[index].cmdf = Olecmdf.Supported;
                        }

                    if (!flag)
                        return -2147221248;
                }

            return 0;
        }

        private void RegisterApplicationCommands()
        {
            CommandHelpers.RegisterCommandHandler(TextView.VisualElement.GetType(), ApplicationCommands.Copy, OnCopy,
                CanCopy);
            CommandHelpers.RegisterCommandHandler(TextView.VisualElement.GetType(), ApplicationCommands.Cut, OnCut,
                CanCut);
            CommandHelpers.RegisterCommandHandler(TextView.VisualElement.GetType(), ApplicationCommands.Paste, OnPaste,
                CanPaste);
        }

        private void SendTextViewCreated()
        {
            var list = IoC.GetAll<Lazy<IMafTextViewCreationListener, IContentTypeAndTextViewRoleMetadata>>();
            foreach (var matchingExtension in SelectMatchingExtensions(list,
                TextView.TextViewModel.DataModel.ContentType, TextView.Roles))
            {
                var instantiatedExtension =
                    EditorParts.GuardedOperations.InstantiateExtension(matchingExtension, matchingExtension);
                if (instantiatedExtension != null)
                    EditorParts.GuardedOperations.CallExtensionPoint(instantiatedExtension,
                        () => instantiatedExtension.MafTextViewCreated(this));
            }
        }

        private void SetTipOpacity(double opacity)
        {
            _tipDimmingTimer?.Stop();
            for (var index = _openedTips.Count - 1; index >= 0; --index)
                _openedTips[index].SetOpacity(opacity);
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

        private void ShimHostNowVisible(object sender, EventArgs e)
        {
            if (CurrentInitializationState < InitializationState.TextBufferAvailable)
                return;
            Init_OnActivation();
        }

        private void ShowContextMenu(IntPtr location, ref int result)
        {
            Point[] pos = null;
            if (location != IntPtr.Zero)
            {
                var forNativeVariant1 = Marshal.GetObjectForNativeVariant(location);
                var forNativeVariant2 = Marshal.GetObjectForNativeVariant(new IntPtr(location.ToInt32() + 16));

                if (forNativeVariant1 is short nullable1 && forNativeVariant2 is short nullable2)
                {
                    pos = new Point[1];
                    pos[0].X = nullable1;
                    pos[0].Y = nullable2;
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
                    pos[0].X = (short) screen.X;
                    pos[0].Y = (short) screen.Y;
                }
            }

            if (pos != null)
            {
                if (GetData(MafUserDataFormat.ContextMenuId, out var data) == 0 && data is Guid contextMenuId)
                    IoC.Get<IMafUIShell>().ShowContextMenu(pos[0], contextMenuId, TextView.VisualElement);
            }
            else
            {
                result = -2147221248;
            }
        }

        private static void ThemeTextViewHostScrollBars(ITextViewHost textViewHost)
        {
            var hostControl = textViewHost?.HostControl;
            if (hostControl == null)
                return;
            ImageThemingUtilities.SetThemeScrollBars(hostControl, true);
        }

        private void View_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (CurrentInitializationState < InitializationState.TextViewAvailable)
                return;
            HandleVerticalScroll();
        }

        private void InsertTab()
        {
            _editorOperations.Indent();
        }

        private void Delete()
        {
            _editorOperations.Delete();
        }

        private void Backtab()
        {
            EnsureSpansExpanded(_editorOperations.TextView.Selection.SelectedSpans);
            _editorOperations.Unindent();
        }

        private void SelectAll()
        {
            _editorOperations.SelectAll();
        }

        private void Untabify()
        {
            _editorOperations.Untabify();
        }

        private void Tabify()
        {
            _editorOperations.Tabify();
        }

        private void GotoMatchingBrace(bool fExtendSelection)
        {
            var hr = 1;
            var piLine = 0;
            var piColumn = 0;
            if (GetCaretPos(out piLine, out piColumn) < 0)
                return;
            var textSpanArray = new TextSpan[1];
            var textSpan1 = new TextSpan();

            //if (_languageTextOps != null)
            {
                //TODO: Language Service
            }
            if (hr != 0 && _textViewFilter != null)
            {
                //TODO: Filter stuff
            }

            if (hr != 0)
            {
                var lineFromLineNumber = TextDocData.DataTextBuffer.CurrentSnapshot.GetLineFromLineNumber(piLine);
                var point = lineFromLineNumber.Start + Math.Min(piColumn, lineFromLineNumber.Length);
                var num = point.Position - lineFromLineNumber.Start.Position;
                if (num < lineFromLineNumber.Length &&
                    PairMatching.FindEnclosingPair(point, MaxBraceMatchLinesToSearch, out var pairSpan) || num > 0 &&
                    PairMatching.FindEnclosingPair(point - 1, MaxBraceMatchLinesToSearch, out pairSpan))
                {
                    textSpan1 = TextConvert.ToMafTextSpan(pairSpan);
                    hr = 0;
                }

                if (hr < 0 || textSpan1.iStartLine == textSpan1.iEndLine &&
                    textSpan1.iStartIndex == textSpan1.iEndIndex)
                    return;
                if (fExtendSelection)
                {
                    ++textSpan1.iEndIndex;
                    if (Math.Abs(textSpan1.iStartLine - piLine) < Math.Abs(textSpan1.iEndLine - piLine)
                        || textSpan1.iStartLine == textSpan1.iEndLine && piLine == textSpan1.iStartLine &&
                        Math.Abs(textSpan1.iStartIndex - piColumn) <= Math.Abs(textSpan1.iEndIndex - piColumn))
                        SetSelection(textSpan1.iStartLine, textSpan1.iStartIndex, textSpan1.iEndLine,
                            textSpan1.iEndIndex);
                    else
                        SetSelection(textSpan1.iEndLine, textSpan1.iEndIndex, textSpan1.iStartLine,
                            textSpan1.iStartIndex);
                }
                else if (Math.Abs(textSpan1.iStartLine - piLine) < Math.Abs(textSpan1.iEndLine - piLine) ||
                         textSpan1.iStartLine == textSpan1.iEndLine &&
                         piLine == textSpan1.iStartLine &&
                         Math.Abs(textSpan1.iStartIndex - piColumn) <= Math.Abs(textSpan1.iEndIndex - piColumn))
                    SetCaretPos(textSpan1.iEndLine, textSpan1.iEndIndex);
                else
                    SetCaretPos(textSpan1.iStartLine, textSpan1.iStartIndex);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        private struct KeyPressEventArg
        {
            [FieldOffset(0)] public readonly int vt;
            [FieldOffset(8)] public readonly char ch;
        }

        internal class ForwardFocusPanel : HwndWrapper
        {
            internal ITextViewHost ViewHost { get; set; }

            internal void UpdatePosition()
            {
                if (ViewHost == null)
                    return;
                var hostControl = ViewHost.HostControl;
                if (PresentationSource.FromDependencyObject(hostControl) == null)
                    return;
                var screen = hostControl.PointToScreen(new Point(0.0, 0.0));
                var deviceUnits =
                    DpiHelper.Default.LogicalToDeviceUnits(new Size(hostControl.ActualWidth, hostControl.ActualHeight));
                User32.SetWindowPos(Handle, IntPtr.Zero, (int) screen.X, (int) screen.Y, (int) deviceUnits.Width,
                    (int) deviceUnits.Height, 20);
            }

            protected override ushort CreateWindowClassCore()
            {
                return RegisterClass("HwndlessEditorFakeHwndWrapper" + Guid.NewGuid());
            }

            protected override IntPtr CreateWindowCore()
            {
                var minValue = int.MinValue;
                var moduleHandle = Kernel32.GetModuleHandle(null);
                return User32.CreateWindowEx(0, new IntPtr(WindowClassAtom), null, minValue, 0, 0, 0, 0, IntPtr.Zero,
                    IntPtr.Zero, moduleHandle, IntPtr.Zero);
            }

            protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                if (msg != 7 || ViewHost == null || ViewHost.TextView == null)
                    return base.WndProc(hwnd, msg, wParam, lParam);
                if (Keyboard.FocusedElement == ViewHost.TextView.VisualElement)
                    if (PresentationSource.FromVisual(ViewHost.TextView.VisualElement) is HwndSource hwndSource)
                        User32.SetFocus(hwndSource.Handle);
                Keyboard.Focus(ViewHost.TextView.VisualElement);
                return IntPtr.Zero;
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

            internal HwndSource HwndSource { get; set; }

            internal bool Initialized { get; private set; }

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

            internal Win32Window Win32Window { get; private set; }

            private SimpleTextViewWindow TextView { get; }

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
                else
                    _forwardFocusPanel?.Dispose();

                GC.SuppressFinalize(this);
            }

            internal void Initialize(bool createHwnd, IntPtr hwndParent)
            {
                CreateHwnd = createHwnd;
                if (createHwnd)
                    HasBeenShown = true;
                if (createHwnd && hwndParent != IntPtr.Zero)
                {
                    CreateHwndSource(hwndParent);
                }
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
                    try
                    {
                        if (wParam != HwndSource.Handle)
                            if (TextView?.TextViewHost != null)
                                Keyboard.Focus(TextView.TextViewHost.TextView.VisualElement);
                    }
                    catch (InvalidOperationException)
                    {
                    }

                return IntPtr.Zero;
            }

            private void OnLayoutUpdated(object sender, EventArgs e)
            {
                _contentPresenter.LayoutUpdated -= OnLayoutUpdated;
                HasBeenShown = true;
                var nowVisible = NowVisible;
                nowVisible?.Invoke(this, EventArgs.Empty);
            }

            private void OnSourceChanged(object sender, SourceChangedEventArgs e)
            {
                PresentationSource.RemoveSourceChangedHandler(_contentPresenter, OnSourceChanged);
                _contentPresenter.LayoutUpdated += OnLayoutUpdated;
            }
        }

        internal sealed class Win32Window : IWin32Window
        {
            public IntPtr Handle { get; }

            public Win32Window(IntPtr handle)
            {
                Handle = handle;
            }
        }

        private class ScrollBarsToolsOptionsMenuCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                return true;
            }

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

        int RemoveProperty(EditPropId idProp);

        int SetProperty(EditPropId idProp, object var);
    }

    public interface IMafTextViewEvents
    {
        void OnChangeCaretLine(IMafTextView view, int iNewLine, int iOldLine);

        void OnChangeScrollInfo(IMafTextView view, int iBar, int iMinUnit, int iMaxUnits, int iVisibleUnits,
            int iFirstVisibleUnit);

        void OnKillFocus(IMafTextView view);

        void OnSetBuffer(IMafTextView view, IMafTextLines pBuffer);
        void OnSetFocus(IMafTextView view);
    }

    public interface IMafTextView : IMafUserData
    {
        void Initialize(IMafTextLines buffer, IntPtr hwndParent, TextViewInitFlags flags);
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

    public enum IndentStyle
    {
        None,
        Default,
        Smart
    }

    internal static class DataObjectHelper
    {
        public static bool ContainsText(IDataObject dataObject, Guid languageServiceId, IMafTextLines buffer)
        {
            if (dataObject == null)
                return false;

            //TODO: languageService stuff
            int num = 0;

            if (!string.IsNullOrEmpty(GetText(dataObject)))
                num = 1;
            return num != 0;
        }

        public static string GetText(IDataObject dataObject)
        {
            var data = new OleDataObject(dataObject);
            if (data.GetDataPresent(DataFormats.Text) ||
                data.GetDataPresent(DataFormats.UnicodeText) ||
                data.GetDataPresent(DataFormats.CommaSeparatedValue))
                return (string) data.GetData(DataFormats.UnicodeText, true);
            if (data.GetDataPresent(DataFormats.Html))
                return ExtractHTMLText(data).Trim();
            return string.Empty;
        }

        public static string GetText(IDataObject dataObject, Guid languageServiceID, TextDocData docData)
        {
            //TODO: Languageservice stuff
            return GetText(dataObject);
        }


        internal static string ExtractHTMLText(OleDataObject data)
        {
            var data1 = data.GetData(DataFormats.Html) as string;
            string str;
            if (data1 != null)
            {
                str = data1;
            }
            else
            {
                MemoryStream data2;
                try
                {
                    data2 = (MemoryStream) data.GetData(DataFormats.Html);
                }
                catch
                {
                    throw new InvalidOperationException(
                        "Can't examine data in IDataObject object, MemoryStream expected but not found");
                }

                var decoder = Encoding.UTF8.GetDecoder();
                var array = data2.ToArray();
                var charCount = decoder.GetCharCount(array, 0, array.Length);
                var chars = new char[charCount];
                decoder.Convert(array, 0, array.Length, chars, 0, charCount, true, out _, out _, out _);
                str = new string(chars);
            }

            var num1 = str.IndexOf("<!--StartFragment-->", StringComparison.CurrentCultureIgnoreCase);
            var num2 = str.IndexOf("<!--EndFragment-->", StringComparison.CurrentCultureIgnoreCase);
            if (num1 != -1 && num2 != -1 && num2 > num1)
                return str.Substring(num1 + "<!--StartFragment-->".Length, num2 - num1 - "<!--StartFragment-->".Length);
            return string.Empty;
        }
    }
}