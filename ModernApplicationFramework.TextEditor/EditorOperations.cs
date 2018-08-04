using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    internal class EditorOperations : IEditorOperations
    {
        private readonly IViewPrimitives _editorPrimitives;
        private readonly EditorOperationsFactoryService _factory;
        private ITextDocument _textDocument;
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

        public ITrackingSpan ProvisionalCompositionSpan { get; }

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

        public bool MoveSelectedLinesUp()
        {
            throw new NotImplementedException();
        }

        public bool MoveSelectedLinesDown()
        {
            throw new NotImplementedException();
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

        public void AddAfterTextBufferChangePrimitive()
        {
            throw new NotImplementedException();
        }

        public void AddBeforeTextBufferChangePrimitive()
        {
            throw new NotImplementedException();
        }

        public bool Backspace()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool InsertProvisionalText(string text)
        {
            throw new NotImplementedException();
        }

        public bool InsertText(string text)
        {
            throw new NotImplementedException();
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
            if ((TextView.Caret.InVirtualSpace ? 0 : (TextView.Caret.Position.BufferPosition == TextView.Caret.ContainingTextViewLine.Start ? 1 : 0)) != 0 && (Options.IsVirtualSpaceEnabled() || extendSelection && TextView.Selection.Mode == TextSelectionMode.Box))
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
            throw new NotImplementedException();
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
                TextView.Caret.MoveTo(TextView.Selection.IsEmpty ? activePoint.TranslateTo(TextView.TextSnapshot) : TextView.Selection.ActivePoint);
            }
            if (!scrollOptions.HasValue)
                return;
            scrollOptions = !TextView.Selection.IsReversed ? scrollOptions.Value & ~EnsureSpanVisibleOptions.ShowStart : scrollOptions.Value | EnsureSpanVisibleOptions.ShowStart;
            TextView.ViewScroller.EnsureSpanVisible(TextView.Selection.StreamSelectionSpan, scrollOptions.Value);
        }

        public void SelectCurrentWord()
        {
            var textRange1 = _editorPrimitives.Caret.GetCurrentWord();
            var previousWord1 = _editorPrimitives.Caret.GetPreviousWord();
            var textRange2 = _editorPrimitives.Selection.Clone();

            if (!_editorPrimitives.Selection.IsEmpty && (textRange2.GetStartPoint().CurrentPosition == textRange1.GetStartPoint().CurrentPosition && textRange2.GetEndPoint().CurrentPosition == textRange1.GetEndPoint().CurrentPosition || textRange2.GetStartPoint().CurrentPosition == previousWord1.GetStartPoint().CurrentPosition && textRange2.GetEndPoint().CurrentPosition == previousWord1.GetEndPoint().CurrentPosition))
                TextView.ViewScroller.EnsureSpanVisible(TextView.Selection.StreamSelectionSpan.SnapshotSpan, EnsureSpanVisibleOptions.MinimumScroll);
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
                ITextViewLine containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(TextView.Selection.AnchorPoint.Position);
                if (TextView.Selection.IsReversed && !TextView.Selection.AnchorPoint.IsInVirtualSpace && (TextView.Selection.AnchorPoint.Position == containingBufferPosition.Start && containingBufferPosition.Start.Position > 0))
                    containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(containingBufferPosition.Start - 1);
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
            SelectAndMoveCaret(new VirtualSnapshotPoint(position1), new VirtualSnapshotPoint(position2), TextSelectionMode.Stream, new EnsureSpanVisibleOptions?());
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

        private static bool IsPointOnBlankViewLine(DisplayTextPoint displayTextPoint)
        {
            return displayTextPoint.GetFirstNonWhiteSpaceCharacterOnViewLine().CurrentPosition ==
                   displayTextPoint.EndOfViewLine;
        }
    }
}