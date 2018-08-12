using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using ModernApplicationFramework.Text;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    internal class EditorOperations : IEditorOperations
    {
        private readonly IViewPrimitives _editorPrimitives;
        private readonly EditorOperationsFactoryService _factory;
        private readonly ITextDocument _textDocument;
        private ITextStructureNavigator _textStructureNavigator;

        public bool CanCut
        {
            get
            {
                if (!_editorPrimitives.Selection.IsEmpty)
                    return !_editorPrimitives.Selection.AdvancedTextRange.Snapshot.TextBuffer.IsReadOnly(
                        _editorPrimitives.Selection.AdvancedTextRange);
                if (IsPointOnBlankViewLine(_editorPrimitives.Caret) &&
                    !Options.GetOptionValue(DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId))
                    return false;
                var containingTextViewLine = _editorPrimitives.Caret.AdvancedCaret.ContainingTextViewLine;
                return !containingTextViewLine.Snapshot.TextBuffer.IsReadOnly(containingTextViewLine
                    .ExtentIncludingLineBreak);
            }
        }

        public bool CanDelete
        {
            get
            {
                if (!_editorPrimitives.Selection.IsEmpty)
                    return !_editorPrimitives.Selection.AdvancedTextRange.Snapshot.TextBuffer.IsReadOnly(
                        _editorPrimitives.Selection.AdvancedTextRange);
                if (_editorPrimitives.Caret.CurrentPosition < TextView.TextSnapshot.Length)
                    return !TextView.TextSnapshot.TextBuffer.IsReadOnly(_editorPrimitives.Caret.CurrentPosition);
                return false;
            }
        }

        public bool CanPaste
        {
            get
            {
                try
                {
                    return Clipboard.ContainsText() &&
                           !TextView.TextSnapshot.TextBuffer.IsReadOnly(_editorPrimitives.Caret.CurrentPosition);
                }
                catch (ExternalException)
                {
                    return false;
                }
            }
        }

        public IEditorOptions Options { get; }

        public ITrackingSpan ProvisionalCompositionSpan { get; private set; }

        public string SelectedText
        {
            get
            {
                return TextView.Selection.SelectedSpans.Count <= 1
                    ? TextView.Selection.StreamSelectionSpan.GetText()
                    : string.Join(Options.GetNewLineCharacter(),
                          TextView.Selection.SelectedSpans
                              .Select(span => span.GetText())
                              .ToArray()) + Options.GetNewLineCharacter();
            }
        }

        public ITextView TextView { get; }

        //TODO: Add Undo Stuff

        public EditorOperations(ITextView textView, EditorOperationsFactoryService factory)
        {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _editorPrimitives = factory.EditorPrimitivesProvider.GetViewPrimitives(textView);
            _textStructureNavigator =
                factory.TextStructureNavigatorFactory.GetTextStructureNavigator(TextView.TextBuffer);
            //_undoHistory = factory.UndoHistoryRegistry.RegisterHistory((object)this._textView.TextBuffer);
            //factory.TextBufferUndoManagerProvider.GetTextBufferUndoManager(this._textView.TextBuffer);
            Options = factory.EditorOptionsProvider.GetOptions(textView);
            _factory.TextDocumentFactoryService.TryGetTextDocument(TextView.TextDataModel.DocumentBuffer,
                out _textDocument);
        }

        private enum SelectionUpdate
        {
            Preserve,
            Reset,
            ResetUnlessEmptyBox,
            Ignore,
            ClearVirtualSpace
        }

        public void AddAfterTextBufferChangePrimitive()
        {
            //TODO: Undo stuff
        }

        public void AddBeforeTextBufferChangePrimitive()
        {
            //TODO: Undo stuff
        }

        public bool Backspace()
        {
            var emptyBox = IsEmptyBoxSelection();
            NormalizedSnapshotSpanCollection boxDeletions = null;
            if (TextView.Selection.IsEmpty)
            {
                if (TextView.Caret.InVirtualSpace)
                {
                    MoveCaretToPreviousIndentStopInVirtualSpace();
                    TextView.Caret.EnsureVisible();
                    return true;
                }

                if (TextView.Caret.Position.BufferPosition.Position == 0)
                    return true;
            }
            else
            {
                var virtualSelectedSpans = TextView.Selection.VirtualSelectedSpans;

                bool Func(VirtualSnapshotSpan s)
                {
                    if (s.SnapshotSpan.IsEmpty) return s.IsInVirtualSpace;
                    return false;
                }

                if (virtualSelectedSpans.All(Func))
                {
                    ResetVirtualSelection();
                    TextView.Caret.EnsureVisible();
                    return true;
                }

                if (emptyBox)
                {
                    var snapshotSpanList =
                        (from snapshotSpan in TextView.Selection.VirtualSelectedSpans.Where(s => !s.IsInVirtualSpace)
                                .Select(s => s.SnapshotSpan)
                            let containingLine = snapshotSpan.Start.GetContainingLine()
                            where snapshotSpan.Start > containingLine.Start
                            select TextView.GetTextElementSpan(snapshotSpan.Start - 1)).ToList();
                    if (snapshotSpanList.Count == 0)
                    {
                        TextView.Caret.MoveTo(TextView.Selection.Start);
                        TextView.Selection.Clear();
                        TextView.Caret.EnsureVisible();
                        return true;
                    }

                    boxDeletions = new NormalizedSnapshotSpanCollection(snapshotSpanList);
                }
            }

            //TODO Text
            return ExecuteAction("Delete Left", () =>
            {
                if (TextView.Selection.IsEmpty)
                    return _editorPrimitives.Caret.DeletePrevious();
                var nullable1 = new VirtualSnapshotPoint?();
                var nullable2 = new VirtualSnapshotPoint?();
                VirtualSnapshotPoint virtualSnapshotPoint1;
                if (emptyBox)
                {
                    virtualSnapshotPoint1 = TextView.Selection.AnchorPoint;
                    if (virtualSnapshotPoint1.IsInVirtualSpace)
                    {
                        virtualSnapshotPoint1 = TextView.Selection.AnchorPoint;
                        var position = virtualSnapshotPoint1.Position;
                        virtualSnapshotPoint1 = TextView.Selection.AnchorPoint;
                        var virtualSpaces = virtualSnapshotPoint1.VirtualSpaces - 1;
                        var virtualSnapshotPoint2 = new VirtualSnapshotPoint(position, virtualSpaces);
                        nullable1 = virtualSnapshotPoint2;
                    }

                    virtualSnapshotPoint1 = TextView.Selection.ActivePoint;
                    if (virtualSnapshotPoint1.IsInVirtualSpace)
                    {
                        virtualSnapshotPoint1 = TextView.Selection.ActivePoint;
                        var position = virtualSnapshotPoint1.Position;
                        virtualSnapshotPoint1 = TextView.Selection.ActivePoint;
                        var virtualSpaces = virtualSnapshotPoint1.VirtualSpaces - 1;
                        var virtualSnapshotPoint2 = new VirtualSnapshotPoint(position, virtualSpaces);
                        nullable2 = virtualSnapshotPoint2;
                    }
                }

                var snapshotCollection1 = boxDeletions;
                if (snapshotCollection1 == null)
                    snapshotCollection1 = TextView.Selection.SelectedSpans;
                var snapshotSpanCollection2 = snapshotCollection1;
                virtualSnapshotPoint1 = TextView.Selection.Start;
                var virtualSpaces1 = virtualSnapshotPoint1.VirtualSpaces;
                if (!DeleteHelper(snapshotSpanCollection2))
                    return false;
                if (emptyBox && nullable1.HasValue || nullable2.HasValue)
                {
                    VirtualSnapshotPoint anchorPoint1;
                    if (!nullable1.HasValue)
                    {
                        anchorPoint1 = TextView.Selection.AnchorPoint;
                    }
                    else
                    {
                        virtualSnapshotPoint1 = nullable1.Value;
                        anchorPoint1 = virtualSnapshotPoint1.TranslateTo(TextView.TextSnapshot);
                    }

                    var anchorPoint2 = anchorPoint1;
                    VirtualSnapshotPoint activePoint1;
                    if (!nullable2.HasValue)
                    {
                        activePoint1 = TextView.Selection.ActivePoint;
                    }
                    else
                    {
                        virtualSnapshotPoint1 = nullable2.Value;
                        activePoint1 = virtualSnapshotPoint1.TranslateTo(TextView.TextSnapshot);
                    }

                    var activePoint2 = activePoint1;
                    TextView.Caret.MoveTo(TextView.Selection.ActivePoint);
                    TextView.Selection.Select(anchorPoint2, activePoint2);
                }
                else if (TextView.Selection.Mode != TextSelectionMode.Box)
                {
                    var caret = TextView.Caret;
                    virtualSnapshotPoint1 = TextView.Selection.Start;
                    var bufferPosition = new VirtualSnapshotPoint(virtualSnapshotPoint1.Position, virtualSpaces1);
                    caret.MoveTo(bufferPosition);
                    TextView.Selection.Clear();
                }

                TextView.Caret.EnsureVisible();
                return true;
            }, SelectionUpdate.ResetUnlessEmptyBox, true);
        }

        public bool Capitalize()
        {
            throw new NotImplementedException();
        }

        public bool ConvertSpacesToTabs()
        {
            throw new NotImplementedException();
        }

        public bool ConvertTabsToSpaces()
        {
            throw new NotImplementedException();
        }

        public bool CopySelection()
        {
            if (!TextView.Selection.IsEmpty)
                return PrepareClipboardSelectionCopy()();
            if (!IsPointOnBlankViewLine(_editorPrimitives.Caret) ||
                Options.GetOptionValue(DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId))
                return PrepareClipboardFullLineCopy(GetFullLines())();
            return true;
        }

        public bool CutFullLine()
        {
            throw new NotImplementedException();
        }

        public bool CutSelection()
        {
            throw new NotImplementedException();
        }

        public bool DecreaseLineIndent()
        {
            throw new NotImplementedException();
        }

        public bool Delete()
        {
            throw new NotImplementedException();
        }

        public bool DeleteBlankLines()
        {
            throw new NotImplementedException();
        }

        public bool DeleteFullLine()
        {
            throw new NotImplementedException();
        }

        public bool DeleteHorizontalWhiteSpace()
        {
            throw new NotImplementedException();
        }

        public bool DeleteToBeginningOfLine()
        {
            throw new NotImplementedException();
        }

        public bool DeleteToEndOfLine()
        {
            throw new NotImplementedException();
        }

        public bool DeleteWordToLeft()
        {
            throw new NotImplementedException();
        }

        public bool DeleteWordToRight()
        {
            throw new NotImplementedException();
        }

        public void ExtendSelection(int newEnd)
        {
            throw new NotImplementedException();
        }

        public string GetWhitespaceForVirtualSpace(VirtualSnapshotPoint point)
        {
            throw new NotImplementedException();
        }

        public void GotoLine(int lineNumber)
        {
            throw new NotImplementedException();
        }

        public bool IncreaseLineIndent()
        {
            throw new NotImplementedException();
        }

        public bool Indent()
        {
            throw new NotImplementedException();
        }

        public bool InsertFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool InsertNewLine()
        {
            return ExecuteAction("New Line", () =>
            {
                var virtualBufferPosition1 = TextView.Caret.Position.VirtualBufferPosition;
                var containingLine1 = virtualBufferPosition1.Position.GetContainingLine();
                var snapshot = containingLine1.Snapshot;
                var charToInsert = TextBufferOperationHelpers.GetNewLineCharacterToInsert(containingLine1, Options);
                bool flag1;
                var caretMoved = false;
                var eventHandler = (EventHandler<CaretPositionChangedEventArgs>) ((sender, e) => caretMoved = true);
                var flag2 = virtualBufferPosition1.IsInVirtualSpace ||
                            virtualBufferPosition1.Position != TextView.Caret.ContainingTextViewLine.Start ||
                            TextView.Caret.ContainingTextViewLine.Extent.Length == 0;
                try
                {
                    using (var edit = TextView.TextBuffer.CreateEdit())
                    {
                        TextView.Caret.PositionChanged += eventHandler;
                        var line = containingLine1;
                        int startIndex;
                        if (TextView.Selection.Mode == TextSelectionMode.Stream)
                        {
                            var snapshotSpan = TextView.Selection.StreamSelectionSpan.SnapshotSpan;
                            edit.Replace(snapshotSpan, charToInsert);
                            line = snapshot.GetLineFromPosition(snapshotSpan.Start);
                            startIndex = snapshotSpan.Start - line.Start.Position;
                        }
                        else
                        {
                            var flag3 = true;
                            startIndex = virtualBufferPosition1.Position.Position - containingLine1.Start.Position;
                            foreach (var selectedSpan in TextView.Selection.SelectedSpans)
                            {
                                var snapshotPoint = selectedSpan.End;
                                var position1 = snapshotPoint.Position;
                                snapshotPoint = virtualBufferPosition1.Position;
                                var position2 = snapshotPoint.Position;
                                if (position1 == position2)
                                {
                                    snapshotPoint = selectedSpan.Start;
                                    var position3 = snapshotPoint.Position;
                                    snapshotPoint = containingLine1.Start;
                                    var position4 = snapshotPoint.Position;
                                    startIndex = position3 - position4;
                                }

                                if (!edit.Delete(selectedSpan))
                                    flag3 = false;
                            }

                            if (!flag3)
                                return false;
                            edit.Replace(new SnapshotSpan(TextView.Caret.Position.BufferPosition, 0), charToInsert);
                        }

                        if (Options.GetOptionValue(DefaultOptions.TrimTrailingWhiteSpaceOptionId))
                        {
                            var num = line.IndexOfPreviousNonWhiteSpaceCharacter(startIndex);
                            var start = line.Start.Position + num + 1;
                            var length = startIndex - num - 1;
                            if (length != 0)
                                edit.Delete(new Span(start, length));
                        }

                        flag1 = edit.Apply() != snapshot;
                    }
                }
                finally
                {
                    TextView.Caret.PositionChanged -= eventHandler;
                }

                if (flag1)
                {
                    if (flag2)
                    {
                        var virtualBufferPosition2 = TextView.Caret.Position.VirtualBufferPosition;
                        var containingLine2 = virtualBufferPosition2.Position.GetContainingLine();
                        if (!caretMoved && virtualBufferPosition2.Position == containingLine2.Start)
                        {
                            caretMoved = PositionCaretWithSmartIndent(false);
                            if (!caretMoved && virtualBufferPosition2.IsInVirtualSpace)
                                TextView.Caret.MoveTo(virtualBufferPosition2.Position);
                        }
                    }

                    ResetSelection();
                }

                return flag1;
            }, SelectionUpdate.Ignore, true);
        }

        public bool InsertProvisionalText(string text)
        {
            throw new NotImplementedException();
        }

        public bool InsertText(string text)
        {
            return InsertText(text, true);
        }

        public bool InsertTextAsBox(string text, out VirtualSnapshotPoint boxStart, out VirtualSnapshotPoint boxEnd)
        {
            throw new NotImplementedException();
        }

        public bool MakeLowercase()
        {
            throw new NotImplementedException();
        }

        public bool MakeUppercase()
        {
            throw new NotImplementedException();
        }

        public void MoveCaret(ITextViewLine textLine, double horizontalOffset, bool extendSelection)
        {
            if (textLine == null)
                throw new ArgumentNullException(nameof(textLine));
            if (extendSelection)
            {
                var anchorPoint = TextView.Selection.AnchorPoint;
                TextView.Caret.MoveTo(textLine, horizontalOffset);
                TextView.Selection.Select(anchorPoint.TranslateTo(TextView.TextSnapshot),
                    TextView.Caret.Position.VirtualBufferPosition);
            }
            else
            {
                var num = TextView.Selection.Mode == TextSelectionMode.Box ? 1 : 0;
                TextView.Selection.Clear();
                if (num != 0)
                    TextView.Selection.Mode = TextSelectionMode.Box;
                TextView.Caret.MoveTo(textLine, horizontalOffset);
            }
        }

        public void MoveCurrentLineToBottom()
        {
            throw new NotImplementedException();
        }

        public void MoveCurrentLineToTop()
        {
            throw new NotImplementedException();
        }

        public void MoveLineDown(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveLineUp(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public bool MoveSelectedLinesDown()
        {
            throw new NotImplementedException();
        }

        public bool MoveSelectedLinesUp()
        {
            throw new NotImplementedException();
        }

        public void MoveToBottomOfView(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToEndOfDocument(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToEndOfLine(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToHome(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToLastNonWhiteSpaceCharacter(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToNextCharacter(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToNextWord(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToPreviousCharacter(bool extendSelection)
        {
            if ((TextView.Caret.InVirtualSpace
                    ? 0
                    : (TextView.Caret.Position.BufferPosition == TextView.Caret.ContainingTextViewLine.Start ? 1 : 0)
                ) != 0 && (Options.IsVirtualSpaceEnabled() ||
                           extendSelection && TextView.Selection.Mode == TextSelectionMode.Box))
                return;
            _editorPrimitives.Caret.MoveToPreviousCharacter(extendSelection);
        }

        public void MoveToPreviousWord(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToStartOfDocument(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToStartOfLine(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToStartOfLineAfterWhiteSpace(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToStartOfNextLineAfterWhiteSpace(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToStartOfPreviousLineAfterWhiteSpace(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToTopOfView(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public bool NormalizeLineEndings(string replacement)
        {
            throw new NotImplementedException();
        }

        public bool OpenLineAbove()
        {
            throw new NotImplementedException();
        }

        public bool OpenLineBelow()
        {
            throw new NotImplementedException();
        }

        public void PageDown(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void PageUp(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public bool Paste()
        {
            throw new NotImplementedException();
        }

        public int ReplaceAllMatches(string searchText, string replaceText, bool matchCase, bool matchWholeWord,
            bool useRegularExpressions)
        {
            throw new NotImplementedException();
        }

        public bool ReplaceSelection(string text)
        {
            throw new NotImplementedException();
        }

        public bool ReplaceText(Span replaceSpan, string text)
        {
            throw new NotImplementedException();
        }

        public void ResetSelection()
        {
            TextView.Selection.Clear();
        }

        public void ScrollColumnLeft()
        {
            throw new NotImplementedException();
        }

        public void ScrollColumnRight()
        {
            throw new NotImplementedException();
        }

        public void ScrollDownAndMoveCaretIfNecessary()
        {
            throw new NotImplementedException();
        }

        public void ScrollLineBottom()
        {
            throw new NotImplementedException();
        }

        public void ScrollLineCenter()
        {
            throw new NotImplementedException();
        }

        public void ScrollLineTop()
        {
            throw new NotImplementedException();
        }

        public void ScrollPageDown()
        {
            throw new NotImplementedException();
        }

        public void ScrollPageUp()
        {
            throw new NotImplementedException();
        }

        public void ScrollUpAndMoveCaretIfNecessary()
        {
            throw new NotImplementedException();
        }

        public void SelectAll()
        {
            throw new NotImplementedException();
        }

        public void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint)
        {
            throw new NotImplementedException();
        }

        public void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint,
            TextSelectionMode selectionMode)
        {
            throw new NotImplementedException();
        }

        public void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint,
            TextSelectionMode selectionMode, EnsureSpanVisibleOptions? scrollOptions)
        {
            if (anchorPoint == activePoint)
            {
                TextView.Selection.Clear();
                TextView.Selection.Mode = selectionMode;
                TextView.Caret.MoveTo(activePoint.TranslateTo(TextView.TextSnapshot));
            }
            else
            {
                TextView.Selection.Select(anchorPoint, activePoint);
                TextView.Selection.Mode = selectionMode;
                TextView.Caret.MoveTo(TextView.Selection.IsEmpty
                    ? activePoint.TranslateTo(TextView.TextSnapshot)
                    : TextView.Selection.ActivePoint);
            }

            if (!scrollOptions.HasValue)
                return;
            scrollOptions = !TextView.Selection.IsReversed
                ? scrollOptions.Value & ~EnsureSpanVisibleOptions.ShowStart
                : scrollOptions.Value | EnsureSpanVisibleOptions.ShowStart;
            TextView.ViewScroller.EnsureSpanVisible(TextView.Selection.StreamSelectionSpan, scrollOptions.Value);
        }

        public void SelectCurrentWord()
        {
            var textRange1 = _editorPrimitives.Caret.GetCurrentWord();
            var previousWord1 = _editorPrimitives.Caret.GetPreviousWord();
            var textRange2 = _editorPrimitives.Selection.Clone();

            if (!_editorPrimitives.Selection.IsEmpty &&
                (textRange2.GetStartPoint().CurrentPosition == textRange1.GetStartPoint().CurrentPosition &&
                 textRange2.GetEndPoint().CurrentPosition == textRange1.GetEndPoint().CurrentPosition ||
                 textRange2.GetStartPoint().CurrentPosition == previousWord1.GetStartPoint().CurrentPosition &&
                 textRange2.GetEndPoint().CurrentPosition == previousWord1.GetEndPoint().CurrentPosition))
            {
                TextView.ViewScroller.EnsureSpanVisible(TextView.Selection.StreamSelectionSpan.SnapshotSpan,
                    EnsureSpanVisibleOptions.MinimumScroll);
            }
            else
            {
                if (textRange1.IsEmpty)
                {
                    var previousWord2 = textRange1.GetStartPoint().GetPreviousWord();
                    if (previousWord2.GetStartPoint().LineNumber == textRange1.GetStartPoint().LineNumber)
                        textRange1 = previousWord2;
                }

                _editorPrimitives.Selection.SelectRange(textRange1);
            }
        }

        public void SelectEnclosing()
        {
            throw new NotImplementedException();
        }

        public void SelectFirstChild()
        {
            throw new NotImplementedException();
        }

        public void SelectLine(ITextViewLine viewLine, bool extendSelection)
        {
            if (viewLine == null)
                throw new ArgumentNullException(nameof(viewLine));
            SnapshotPoint position1;
            SnapshotPoint position2;
            if (!extendSelection || TextView.Selection.IsEmpty)
            {
                position1 = viewLine.Start;
                position2 = viewLine.EndIncludingLineBreak;
            }
            else
            {
                var containingBufferPosition =
                    TextView.GetTextViewLineContainingBufferPosition(TextView.Selection.AnchorPoint.Position);
                if (TextView.Selection.IsReversed && !TextView.Selection.AnchorPoint.IsInVirtualSpace &&
                    TextView.Selection.AnchorPoint.Position == containingBufferPosition.Start &&
                    containingBufferPosition.Start.Position > 0)
                    containingBufferPosition =
                        TextView.GetTextViewLineContainingBufferPosition(containingBufferPosition.Start - 1);
                if (viewLine.Start < containingBufferPosition.Start)
                {
                    position1 = containingBufferPosition.EndIncludingLineBreak;
                    position2 = viewLine.Start;
                }
                else
                {
                    position1 = containingBufferPosition.Start;
                    position2 = viewLine.EndIncludingLineBreak;
                }
            }

            SelectAndMoveCaret(new VirtualSnapshotPoint(position1), new VirtualSnapshotPoint(position2),
                TextSelectionMode.Stream, new EnsureSpanVisibleOptions?());
            TextView.Caret.EnsureVisible();
        }

        public void SelectNextSibling(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void SelectPreviousSibling(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void SwapCaretAndAnchor()
        {
            throw new NotImplementedException();
        }

        public bool Tabify()
        {
            throw new NotImplementedException();
        }

        public bool ToggleCase()
        {
            throw new NotImplementedException();
        }

        public bool TransposeCharacter()
        {
            throw new NotImplementedException();
        }

        public bool TransposeLine()
        {
            throw new NotImplementedException();
        }

        public bool TransposeWord()
        {
            throw new NotImplementedException();
        }

        public bool Unindent()
        {
            throw new NotImplementedException();
        }

        public bool Untabify()
        {
            throw new NotImplementedException();
        }

        public void ZoomIn()
        {
            if (TextView == null || !TextView.Roles.Contains("ZOOMABLE"))
                return;
            var num = TextView.ZoomLevel * 1.1;
            if (num >= 400.0 && Math.Abs(num - 400.0) >= 1E-05)
                return;
            TextView.ZoomLevel = num;
        }

        public void ZoomOut()
        {
            if (TextView == null || !TextView.Roles.Contains("ZOOMABLE"))
                return;
            var num = TextView.ZoomLevel / 1.1;
            if (num <= 20.0 && Math.Abs(num - 20.0) >= 1E-05)
                return;
            TextView.ZoomLevel = num;
        }

        public void ZoomTo(double zoomLevel)
        {
            if (TextView == null || !TextView.Roles.Contains("ZOOMABLE"))
                return;
            TextView.ZoomLevel = zoomLevel;
        }

        internal bool DeleteHelper(NormalizedSpanCollection spans)
        {
            return EditHelper(e =>
            {
                foreach (var span in spans)
                    if (!e.Delete(span) || e.Canceled)
                        return false;
                return true;
            });
        }

        internal bool EditHelper(Func<ITextEdit, bool> editAction)
        {
            using (var edit = TextView.TextBuffer.CreateEdit())
            {
                if (!editAction(edit))
                    return false;
                edit.Apply();
                return !edit.Canceled;
            }
        }

        internal string GetWhiteSpaceForPositionAndVirtualSpace(SnapshotPoint position, int virtualSpaces,
            bool useBufferPrimitives)
        {
            return GetWhiteSpaceForPositionAndVirtualSpace(position, virtualSpaces, useBufferPrimitives,
                TextView.Options.IsConvertTabsToSpacesEnabled());
        }

        internal string GetWhiteSpaceForPositionAndVirtualSpace(SnapshotPoint position, int virtualSpaces,
            bool useBufferPrimitives, bool convertTabsToSpaces)
        {
            string str;
            if (!convertTabsToSpaces)
            {
                var tabSize = TextView.Options.GetTabSize();
                var num1 = !useBufferPrimitives
                    ? _editorPrimitives.View.GetTextPoint(position).DisplayColumn
                    : _editorPrimitives.Buffer.GetTextPoint(position).Column;
                var num2 = num1 + virtualSpaces;
                var count1 = num2 % tabSize;
                var count2 = (num2 - count1 - num1 + tabSize - 1) / tabSize;
                str = count2 <= 0 ? new string(' ', virtualSpaces) : new string('\t', count2) + new string(' ', count1);
            }
            else
            {
                str = new string(' ', virtualSpaces);
            }

            return str;
        }

        internal string GetWhiteSpaceForPositionAndVirtualSpace(SnapshotPoint position, int virtualSpaces)
        {
            return GetWhiteSpaceForPositionAndVirtualSpace(position, virtualSpaces, false);
        }

        internal bool IsEmptyBoxSelection()
        {
            if (!TextView.Selection.IsEmpty)
                return TextView.Selection.VirtualSelectedSpans.All(s => s.IsEmpty);
            return false;
        }

        internal bool ReplaceHelper(Span span, string text)
        {
            return EditHelper(e => e.Replace(span, text));
        }

        private static bool IsPointOnBlankViewLine(DisplayTextPoint displayTextPoint)
        {
            return displayTextPoint.GetFirstNonWhiteSpaceCharacterOnViewLine().CurrentPosition ==
                   displayTextPoint.EndOfViewLine;
        }

        private static bool IsSpaceCharacter(char c)
        {
            if (c != ' ' && c != '\t' && c != '\x200B')
                return char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
            return true;
        }

        private bool ExecuteAction(string undoText, Func<bool> action,
            SelectionUpdate preserveCaretAndSelection = SelectionUpdate.Ignore,
            bool ensureVisible = false)
        {
            //TODO:
            //using (var transaction = this._undoHistory.CreateTransaction(undoText))
            //{
            var textSnapshot = TextView.TextSnapshot;
            AddBeforeTextBufferChangePrimitive();
            var position = TextView.Caret.Position;
            var anchorPoint1 = TextView.Selection.AnchorPoint;
            var activePoint1 = TextView.Selection.ActivePoint;
            if (!action())
                return false;
            switch (preserveCaretAndSelection)
            {
                case SelectionUpdate.Preserve:
                    TextView.Caret.MoveTo(
                        new VirtualSnapshotPoint(
                            new SnapshotPoint(TextView.TextSnapshot, position.BufferPosition.Position),
                            position.VirtualSpaces), position.Affinity);
                    TextView.Selection.Select(
                        new VirtualSnapshotPoint(new SnapshotPoint(TextView.TextSnapshot, anchorPoint1.Position),
                            anchorPoint1.VirtualSpaces),
                        new VirtualSnapshotPoint(new SnapshotPoint(TextView.TextSnapshot, activePoint1.Position),
                            activePoint1.VirtualSpaces));
                    break;
                case SelectionUpdate.Reset:
                    ResetSelection();
                    break;
                case SelectionUpdate.ResetUnlessEmptyBox:
                    if (!IsEmptyBoxSelection())
                        ResetSelection();
                    break;
                case SelectionUpdate.ClearVirtualSpace:
                    TextView.Caret.MoveTo(TextView.Caret.Position.BufferPosition);
                    var selection = TextView.Selection;
                    var virtualSnapshotPoint = TextView.Selection.AnchorPoint;
                    var anchorPoint2 = new VirtualSnapshotPoint(virtualSnapshotPoint.Position);
                    virtualSnapshotPoint = TextView.Selection.ActivePoint;
                    var activePoint2 = new VirtualSnapshotPoint(virtualSnapshotPoint.Position);
                    selection.Select(anchorPoint2, activePoint2);
                    break;
            }

            if (ensureVisible)
                TextView.Caret.EnsureVisible();
            AddAfterTextBufferChangePrimitive();
            //TODO:
            //if (textSnapshot != TextView.TextSnapshot)
            //    transaction.Complete();
            //}
            return true;
        }

        private int GetLeadingWhitespaceChars(ITextSnapshotLine line, SnapshotPoint startPosition)
        {
            var num = 0;
            for (var position = startPosition.Position;
                position < line.End && IsSpaceCharacter(line.Snapshot[position]);
                ++position)
                ++num;
            return num;
        }

        private VirtualSnapshotPoint GetPreviousIndentStopInVirtualSpace(VirtualSnapshotPoint point)
        {
            var column = _editorPrimitives.Buffer.GetTextPoint(point.Position).Column;
            var num1 = column + point.VirtualSpaces;
            var indentSize = Options.GetIndentSize();
            var num2 = 1;
            var num3 = (num1 - num2) / indentSize * indentSize;
            if (num3 > column)
                return new VirtualSnapshotPoint(point.Position, num3 - column);
            return new VirtualSnapshotPoint(point.Position);
        }

        //TODO: Localize Undo string "Insert Text"
        private bool InsertText(string text, bool final)
        {
            return InsertText(text, final, "Insert Text", TextView.Caret.OverwriteMode);
        }

        private bool InsertText(string text, bool final, string undoText, bool isOverwriteModeEnabled)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (text.Length == 0 && !final)
                throw new ArgumentException("Provisional TextInput cannot be zero-length");
            //using (ITextUndoTransaction transaction = this._undoHistory.CreateTransaction(undoText))
            //{
            var allowableMergeDirections =
                TextTransactionMergeDirections.Forward | TextTransactionMergeDirections.Backward;
            if (!TextView.Selection.IsEmpty && !IsEmptyBoxSelection() ||
                _textDocument != null && !_textDocument.IsDirty)
                allowableMergeDirections = TextTransactionMergeDirections.Forward;
            //transaction.MergePolicy = (IMergeTextUndoTransactionPolicy)new TextTransactionMergePolicy(allowableMergeDirections);
            AddBeforeTextBufferChangePrimitive();
            var textEditAction = TextEditAction.Type;
            IEnumerable<VirtualSnapshotSpan> source;
            if (!TextView.Selection.IsEmpty && ProvisionalCompositionSpan == null)
            {
                if (TextView.Options.IsOverwriteModeEnabled() && IsEmptyBoxSelection())
                {
                    var virtualSnapshotSpanList = new List<VirtualSnapshotSpan>();
                    foreach (var virtualSelectedSpan in TextView.Selection.VirtualSelectedSpans)
                    {
                        var start = virtualSelectedSpan.Start;
                        var position = start.Position;
                        start = virtualSelectedSpan.Start;
                        if (start.IsInVirtualSpace || position.GetContainingLine().End == position)
                            virtualSnapshotSpanList.Add(virtualSelectedSpan);
                        else
                            virtualSnapshotSpanList.Add(new VirtualSnapshotSpan(new SnapshotSpan(position,
                                TextView.GetTextElementSpan(position).End)));
                    }

                    source = virtualSnapshotSpanList;
                }
                else
                {
                    source = TextView.Selection.VirtualSelectedSpans;
                }
            }
            else if (ProvisionalCompositionSpan != null)
            {
                var span = ProvisionalCompositionSpan.GetSpan(TextView.TextSnapshot);
                if (IsEmptyBoxSelection() & final)
                {
                    var virtualSnapshotSpanList = (IList<VirtualSnapshotSpan>) new List<VirtualSnapshotSpan>();
                    foreach (var end in TextView.Selection.VirtualSelectedSpans.Select(s => s.Start.Position))
                        if (end.Position - span.Length >= 0)
                            virtualSnapshotSpanList.Add(
                                new VirtualSnapshotSpan(new SnapshotSpan(end - span.Length, end)));
                    source = virtualSnapshotSpanList;
                }
                else
                {
                    source = new VirtualSnapshotSpan[1]
                    {
                        new VirtualSnapshotSpan(span)
                    };
                }

                textEditAction = TextEditAction.ProvisionalOverwrite;
            }
            else
            {
                var virtualBufferPosition = TextView.Caret.Position.VirtualBufferPosition;
                if (isOverwriteModeEnabled && !virtualBufferPosition.IsInVirtualSpace)
                {
                    var position = virtualBufferPosition.Position;
                    source = new[]
                    {
                        new VirtualSnapshotSpan(new SnapshotSpan(position, TextView.GetTextElementSpan(position).End))
                    };
                }
                else
                {
                    source = new[]
                    {
                        new VirtualSnapshotSpan(virtualBufferPosition, virtualBufferPosition)
                    };
                }
            }

            var version = TextView.TextSnapshot.Version;
            var trackingSpan = (ITrackingSpan) null;
            var flag1 = true;
            var nullable = new int?();
            var num1 = 0;
            var trackingPoint = (ITrackingPoint) null;
            using (var edit = TextView.TextBuffer.CreateEdit(EditOptions.None, new int?(), textEditAction))
            {
                var flag2 = true;
                foreach (var virtualSnapshotSpan in source)
                {
                    var replaceWith = text;
                    var textSnapshot = TextView.TextSnapshot;
                    var start = virtualSnapshotSpan.Start;
                    var position = (int) start.Position;
                    trackingPoint = textSnapshot.CreateTrackingPoint(position, (PointTrackingMode) 1);
                    start = virtualSnapshotSpan.Start;
                    if (start.IsInVirtualSpace)
                    {
                        var whitespaceForVirtualSpace = GetWhitespaceForVirtualSpace(virtualSnapshotSpan.Start);
                        if (flag2)
                            TextView.TextBuffer.Properties["WhitespaceInserted"] = whitespaceForVirtualSpace.Length;
                        replaceWith = whitespaceForVirtualSpace + text;
                    }

                    if (!nullable.HasValue)
                        nullable = replaceWith.Length - text.Length;
                    num1 = replaceWith.Length - text.Length;
                    if (!edit.Replace(virtualSnapshotSpan.SnapshotSpan, replaceWith) || edit.Canceled)
                    {
                        flag1 = false;
                        break;
                    }

                    flag2 = false;
                }

                if (flag1)
                {
                    edit.Apply();
                    flag1 = !edit.Canceled;
                }
            }

            if (flag1)
            {
                TextView.Caret.MoveTo(TextView.Caret.Position.BufferPosition);
                TextView.Selection.Select(new VirtualSnapshotPoint(TextView.Selection.AnchorPoint.Position),
                    new VirtualSnapshotPoint(TextView.Selection.ActivePoint.Position));
                var virtualSelectedSpans = TextView.Selection.VirtualSelectedSpans;
                var func = (Func<VirtualSnapshotSpan, bool>) (s => !s.IsEmpty);
                if (virtualSelectedSpans.Any(func))
                    TextView.Selection.Clear();
                TextView.Caret.EnsureVisible();
                AddAfterTextBufferChangePrimitive();
                //transaction.Complete();
                if (final)
                {
                    trackingSpan = null;
                }
                else if (TextView.Selection.IsReversed)
                {
                    var num2 = nullable ?? 0;
                    trackingSpan = version.Next.CreateTrackingSpan(
                        new Span(source.First().Start.Position + num2, text.Length), SpanTrackingMode.EdgeExclusive);
                }
                else
                {
                    var position = trackingPoint.GetPoint(TextView.TextSnapshot).Position;
                    trackingSpan = version.Next.CreateTrackingSpan(new Span(position + num1, text.Length),
                        SpanTrackingMode.EdgeExclusive);
                }
            }

            if (ProvisionalCompositionSpan != trackingSpan)
            {
                ProvisionalCompositionSpan = trackingSpan;
                TextView.ProvisionalTextHighlight = ProvisionalCompositionSpan;
            }

            return flag1;
            //}
        }

        private void MoveCaretToPreviousIndentStopInVirtualSpace()
        {
            TextView.Caret.MoveTo(GetPreviousIndentStopInVirtualSpace(TextView.Caret.Position.VirtualBufferPosition));
        }

        private DisplayTextRange GetFullLines()
        {
            var displayTextRange = _editorPrimitives.Selection.Clone();
            var displayStartPoint = displayTextRange.GetDisplayStartPoint();
            var displayEndPoint = displayTextRange.GetDisplayEndPoint();
            displayStartPoint.MoveTo(displayStartPoint.StartOfViewLine);
            displayEndPoint.MoveToBeginningOfNextViewLine();
            return displayStartPoint.GetDisplayTextRange(displayEndPoint);
        }

        private bool PositionCaretWithSmartIndent(bool useOnlyVirtualSpace = true, bool extendSelection = false)
        {
            var virtualBufferPosition = TextView.Caret.Position.VirtualBufferPosition;
            var containingLine = virtualBufferPosition.Position.GetContainingLine();
            var desiredIndentation = _factory.SmartIndentationService.GetDesiredIndentation(TextView, containingLine);
            if (desiredIndentation.HasValue)
            {
                if (virtualBufferPosition.Position == containingLine.End)
                {
                    var activePoint = new VirtualSnapshotPoint(virtualBufferPosition.Position,
                        Math.Max(0, desiredIndentation.Value - containingLine.Length));
                    SelectAndMoveCaret(extendSelection ? TextView.Selection.AnchorPoint : activePoint, activePoint,
                        TextSelectionMode.Stream, new EnsureSpanVisibleOptions?());
                    return true;
                }

                if (!useOnlyVirtualSpace)
                {
                    var leadingWhitespaceChars =
                        GetLeadingWhitespaceChars(containingLine, virtualBufferPosition.Position);
                    var positionAndVirtualSpace =
                        GetWhiteSpaceForPositionAndVirtualSpace(virtualBufferPosition.Position,
                            desiredIndentation.Value);
                    return ReplaceHelper(new SnapshotSpan(virtualBufferPosition.Position, leadingWhitespaceChars),
                        positionAndVirtualSpace);
                }
            }

            return false;
        }

        private void ResetVirtualSelection()
        {
            var start = TextView.Selection.Start;
            var containingBufferPosition1 = TextView.GetTextViewLineContainingBufferPosition(start.Position);
            var end = TextView.Selection.End;
            var containingBufferPosition2 = TextView.GetTextViewLineContainingBufferPosition(end.Position);
            var extendedCharacterBounds = containingBufferPosition1.GetExtendedCharacterBounds(start);
            var left1 = extendedCharacterBounds.Left;
            extendedCharacterBounds = containingBufferPosition2.GetExtendedCharacterBounds(end);
            var left2 = extendedCharacterBounds.Left;
            var xCoordinate = Math.Min(left1, left2);
            TextView.Caret.MoveTo(
                (TextView.Selection.IsReversed ? containingBufferPosition1 : containingBufferPosition2)
                .GetInsertionBufferPositionFromXCoordinate(xCoordinate));
            TextView.Selection.Clear();
        }

        private Func<bool> PrepareClipboardSelectionCopy()
        {
            var selectedSpans = TextView.Selection.SelectedSpans;
            var text = SelectedText;
            string rtfText = null;
            try
            {
                rtfText = GenerateRtf(selectedSpans);
            }
            catch (OperationCanceledException)
            {
            }

            var isBox = TextView.Selection.Mode == TextSelectionMode.Box;
            return () => CopyToClipboard(text, rtfText, false, isBox);
        }

        private Func<bool> PrepareClipboardFullLineCopy(DisplayTextRange textRange)
        {
            var text = textRange.GetText();
            string rtfText = null;
            try
            {
                rtfText = GenerateRtf(textRange.AdvancedTextRange);
            }
            catch (OperationCanceledException)
            {
            }
            return () => CopyToClipboard(text, rtfText, true, false);
        }

        private string GenerateRtf(SnapshotSpan span)
        {
            return GenerateRtf(new NormalizedSnapshotSpanCollection(span));
        }

        private string GenerateRtf(NormalizedSnapshotSpanCollection spans)
        {
            var source = spans;
            int Func(SnapshotSpan span) => span.Length;
            if (source.Sum(Func) >= TextView.Options.GetOptionValue(MaxRtfCopyLength.OptionKey))
                return null;
            if (!TextView.Options.GetOptionValue(UseAccurateClassificationForRtfCopy.OptionKey))
                return _factory.RtfBuilderService.GenerateRtf(spans);
            //TODO: Text
            using (var waitContext = WaitHelper.Wait(_factory.WaitIndicator, "Wait Title", "Wait Message"))
                return ((IRtfBuilderService2) _factory.RtfBuilderService).GenerateRtf(spans, waitContext.CancellationToken);
        }

        private static bool CopyToClipboard(string textData, string rtfData, bool lineCutCopyTag, bool boxCutCopyTag)
        {
            try
            {
                var dataObject = new DataObject();
                dataObject.SetText(textData);
                if (rtfData != null)
                    dataObject.SetData(DataFormats.Rtf, rtfData);
                if (lineCutCopyTag)
                    dataObject.SetData("EditorOperationsLineCutCopyClipboardTag", true);
                if (boxCutCopyTag)
                    dataObject.SetData("ColumnSelect", true);
                Clipboard.SetDataObject(dataObject, false);
                return true;
            }
            catch (ExternalException)
            {
                return false;
            }
        }
    }
}