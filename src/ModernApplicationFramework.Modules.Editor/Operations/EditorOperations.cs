using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ModernApplicationFramework.Text.Ui.Text;
using ModernApplicationFramework.Text.Utilities;

namespace ModernApplicationFramework.Modules.Editor.Operations
{
    internal class EditorOperations : IEditorOperations
    {
        private readonly IViewPrimitives _editorPrimitives;
        private readonly EditorOperationsFactoryService _factory;
        private readonly ITextDocument _textDocument;
        private ITextStructureNavigator _textStructureNavigator;
        private IMultiSelectionBroker _multiSelectionBroker;
        private ITrackingSpan _immProvisionalComposition;

        public bool CanCut
        {
            get
            {
                if (!_editorPrimitives.Selection.IsEmpty)
                    return !_editorPrimitives.Selection.AdvancedTextRange.Snapshot.TextBuffer.IsReadOnly(_editorPrimitives.Selection.AdvancedTextRange);
                if (IsPointOnBlankViewLine(_editorPrimitives.Caret) && !Options.GetOptionValue(DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId))
                    return false;
                var containingTextViewLine = _editorPrimitives.Caret.AdvancedCaret.ContainingTextViewLine;
                return !containingTextViewLine.Snapshot.TextBuffer.IsReadOnly(containingTextViewLine.ExtentIncludingLineBreak);
            }
        }

        public bool CanDelete
        {
            get
            {
                if (!_editorPrimitives.Selection.IsEmpty)
                    return !_editorPrimitives.Selection.AdvancedTextRange.Snapshot.TextBuffer.IsReadOnly(_editorPrimitives.Selection.AdvancedTextRange);
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
                    return Clipboard.ContainsText() && !TextView.TextSnapshot.TextBuffer.IsReadOnly(_editorPrimitives.Caret.CurrentPosition);
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

        internal EditorOperations(ITextView textView, EditorOperationsFactoryService factory)
        {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _editorPrimitives = factory.EditorPrimitivesProvider.GetViewPrimitives(textView);
            _multiSelectionBroker = TextView.GetMultiSelectionBroker();
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
            bool flag;
            if (WillBackspaceCreateEdit())
            {
                var selections = _multiSelectionBroker.AllSelections;
                var boxSelection = _multiSelectionBroker.BoxSelection;
                var primarySelection = _multiSelectionBroker.PrimarySelection;
                //TODO: Text
                flag = ExecuteAction("Delete Left", () =>
                {
                    using (_multiSelectionBroker.BeginBatchOperation())
                    {
                        if (TryBackspaceEdit(selections))
                            return TryPostBackspaceSelectionUpdate(selections, primarySelection, boxSelection);
                    }
                    return false;
                });
            }
            else
                flag = TryBackspaceSelections();
            if (flag)
                _multiSelectionBroker.TryEnsureVisible(_multiSelectionBroker.PrimarySelection, EnsureSpanVisibleOptions.MinimumScroll);
            return flag;
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
            if (!IsPointOnBlankViewLine(_editorPrimitives.Caret) || Options.GetOptionValue(DefaultTextViewOptions.CutOrCopyBlankLineIfNoSelectionId))
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
                var editSucceeded = true;
                var snapshot = TextView.TextViewModel.EditBuffer.CurrentSnapshot;
                using (_multiSelectionBroker.BeginBatchOperation())
                {
                    var toIndent = new HashSet<object>();
                    var edit = TextView.TextBuffer.CreateEdit();
                    try
                    {
                        if (_multiSelectionBroker.IsBoxSelection)
                            _multiSelectionBroker.BreakBoxSelection();
                        _multiSelectionBroker.PerformActionOnAllSelections(transformer =>
                        {
                            var insertionPoint = transformer.Selection.InsertionPoint;
                            var snapshotPoint = insertionPoint.Position;
                            var containingLine = snapshotPoint.GetContainingLine();
                            var containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(insertionPoint.Position);
                            var characterToInsert = TextBufferOperationHelpers.GetNewLineCharacterToInsert(containingLine, Options);
                            var flag = insertionPoint.IsInVirtualSpace || insertionPoint.Position != containingBufferPosition.Extent.Start || containingBufferPosition.Extent.Length == 0;
                            var snapshotSpan = (Span)transformer.Selection.Extent.SnapshotSpan;
                            editSucceeded = editSucceeded && edit.Replace(snapshotSpan, characterToInsert);
                            var lineFromPosition = snapshot.GetLineFromPosition(snapshotSpan.Start);
                            var start1 = snapshotSpan.Start;
                            snapshotPoint = lineFromPosition.Start;
                            var position = snapshotPoint.Position;
                            var startIndex = start1 - position;
                            if (Options.GetOptionValue(DefaultOptions.TrimTrailingWhiteSpaceOptionId))
                            {
                                var num = lineFromPosition.IndexOfPreviousNonWhiteSpaceCharacter(startIndex);
                                snapshotPoint = lineFromPosition.Start;
                                var start2 = snapshotPoint.Position + num + 1;
                                var length = startIndex - num - 1;
                                if (length != 0)
                                    edit.Delete(new Span(start2, length));
                            }
                            if (!flag)
                                return;
                            toIndent.Add(transformer);
                        });
                        editSucceeded = editSucceeded && edit.Apply() != snapshot;
                    }
                    finally
                    {
                        edit?.Dispose();
                    }
                    if (editSucceeded)
                    {
                        if (toIndent.Count > 0)
                            _multiSelectionBroker.PerformActionOnAllSelections(transformer =>
                            {
                                if (!toIndent.Contains(transformer))
                                    return;
                                if (!PositionCaretWithSmartIndent(transformer, false, false))
                                {
                                    var insertionPoint = transformer.Selection.InsertionPoint;
                                    if (insertionPoint.IsInVirtualSpace)
                                    {
                                        var selectionTransformer = transformer;
                                        insertionPoint = transformer.Selection.InsertionPoint;
                                        var point = new VirtualSnapshotPoint(insertionPoint.Position);
                                        var num1 = 0;
                                        var num2 = 1;
                                        selectionTransformer.MoveTo(point, num1 != 0, (PositionAffinity)num2);
                                    }
                                }
                                transformer.PerformAction(PredefinedSelectionTransformations.ClearSelection);
                                transformer.CapturePreferredReferencePoint();
                            });
                    }
                }
                return editSucceeded;
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
                TextView.Selection.Select(anchorPoint.TranslateTo(TextView.TextSnapshot), TextView.Caret.Position.VirtualBufferPosition);
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

        public void MoveToPreviousCharacter(bool select)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(select
                ? PredefinedSelectionTransformations.SelectToPreviousCaretPosition
                : PredefinedSelectionTransformations.MoveToPreviousCaretPosition);
            TextView.Caret.EnsureVisible();
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
            var num = anchorPoint == activePoint ? 1 : 0;
            var selection = new Selection(anchorPoint, activePoint);
            if (selectionMode == TextSelectionMode.Box)
                _multiSelectionBroker.SetBoxSelection(selection);
            else
                _multiSelectionBroker.SetSelection(selection);
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
            TextRange textRange2 = _editorPrimitives.Selection.Clone();
            if (!_editorPrimitives.Selection.IsEmpty && (textRange2.GetStartPoint().CurrentPosition == textRange1.GetStartPoint().CurrentPosition && textRange2.GetEndPoint().CurrentPosition == textRange1.GetEndPoint().CurrentPosition || textRange2.GetStartPoint().CurrentPosition == previousWord1.GetStartPoint().CurrentPosition && textRange2.GetEndPoint().CurrentPosition == previousWord1.GetEndPoint().CurrentPosition))
            {
                TextView.ViewScroller.EnsureSpanVisible(TextView.Selection.StreamSelectionSpan.SnapshotSpan, EnsureSpanVisibleOptions.MinimumScroll);
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
                var containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(TextView.Selection.AnchorPoint.Position);
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
            if (!(TextView is ITextView textView) || !textView.Roles.Contains("ZOOMABLE"))
                return;
            var num = textView.ZoomLevel * 1.1;
            if (num >= 400.0 && Math.Abs(num - 400.0) >= 1E-05)
                return;
            textView.Options.GlobalOptions.SetOptionValue(DefaultViewOptions.ZoomLevelId, num);
        }

        public void ZoomOut()
        {
            if (!(TextView is ITextView textView) || !textView.Roles.Contains("ZOOMABLE"))
                return;
            var num = textView.ZoomLevel / 1.1;
            if (num <= 20.0 && Math.Abs(num - 20.0) >= 1E-05)
                return;
            textView.Options.GlobalOptions.SetOptionValue(DefaultViewOptions.ZoomLevelId, num);
        }

        public void ZoomTo(double zoomLevel)
        {
            if (!(TextView is ITextView textView) || !textView.Roles.Contains("ZOOMABLE"))
                return;
            textView.Options.GlobalOptions.SetOptionValue(DefaultViewOptions.ZoomLevelId, zoomLevel);
        }

        internal bool DeleteHelper(NormalizedSpanCollection spans)
        {
            return EditHelper(e =>
            {
                foreach (var span in spans)
                {
                    if (!e.Delete(span) || e.Canceled)
                        return false;
                }
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
                str = new string(' ', virtualSpaces);
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
            //TODO: undo
            //using (ITextUndoTransaction transaction = this._undoHistory.CreateTransaction(undoText))
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
                        TextView.Caret.MoveTo(new VirtualSnapshotPoint(new SnapshotPoint(TextView.TextSnapshot, position.BufferPosition.Position), position.VirtualSpaces), position.Affinity);
                        TextView.Selection.Select(
                            new VirtualSnapshotPoint(
                                new SnapshotPoint(TextView.TextSnapshot, anchorPoint1.Position),
                                anchorPoint1.VirtualSpaces),
                            new VirtualSnapshotPoint(
                                new SnapshotPoint(TextView.TextSnapshot, activePoint1.Position),
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
                //if (textSnapshot != this._textView.TextSnapshot)
                //    transaction.Complete();
            //}
            return true;
        }

        private static int GetLeadingWhitespaceChars(ITextSnapshotLine line, SnapshotPoint startPosition)
        {
            var num = 0;
            for (var position = startPosition.Position; position < line.End && IsSpaceCharacter(line.Snapshot[position]); ++position)
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

        
        private bool InsertText(string text, bool final)
        {
            //TODO: Localize
            return InsertText(text, final, "Insert Text", TextView.Caret.OverwriteMode);
        }

        private bool InsertText(string text, bool final, string undoText, bool isOverwriteModeEnabled)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (text.Length == 0 && !final)
                throw new ArgumentException("Provisional TextInput cannot be zero-length");
            //TODO: undo
            //using (ITextUndoTransaction transaction = this._undoHistory.CreateTransaction(undoText))
            //{
                var allowableMergeDirections = TextTransactionMergeDirections.Forward | TextTransactionMergeDirections.Backward;
                if (!TextView.Selection.IsEmpty && !IsEmptyBoxSelection() || _textDocument != null && !_textDocument.IsDirty)
                    allowableMergeDirections = TextTransactionMergeDirections.Forward;
                //transaction.MergePolicy = (IMergeTextUndoTransactionPolicy)new TextTransactionMergePolicy(allowableMergeDirections);
                AddBeforeTextBufferChangePrimitive();
                var textEditAction = TextEditAction.Type;
                IEnumerable<VirtualSnapshotSpan> source;
                if (!TextView.Selection.IsEmpty && _immProvisionalComposition == null)
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
                                virtualSnapshotSpanList.Add(new VirtualSnapshotSpan(new SnapshotSpan(position, TextView.GetTextElementSpan(position).End)));
                        }
                        source = virtualSnapshotSpanList;
                    }
                    else
                        source = _multiSelectionBroker.VirtualSelectedSpans;
                }
                else if (_immProvisionalComposition != null)
                {
                    var span = _immProvisionalComposition.GetSpan(TextView.TextSnapshot);
                    if (IsEmptyBoxSelection() & final)
                    {
                        var virtualSnapshotSpanList = (IList<VirtualSnapshotSpan>)new List<VirtualSnapshotSpan>();
                        foreach (var end in TextView.Selection.VirtualSelectedSpans.Select(s => s.Start.Position))
                        {
                            if (end.Position - span.Length >= 0)
                                virtualSnapshotSpanList.Add(new VirtualSnapshotSpan(new SnapshotSpan(end - span.Length, end)));
                        }
                        source = virtualSnapshotSpanList;
                    }
                    else
                        source = new VirtualSnapshotSpan[1]
                        {
                            new VirtualSnapshotSpan(span)
                        };
                    textEditAction = TextEditAction.ProvisionalOverwrite;
                }
                else
                {
                    var virtualSnapshotSpanList = new List<VirtualSnapshotSpan>();
                    foreach (var selection in _multiSelectionBroker.GetSelectionsIntersectingSpan(new SnapshotSpan(_multiSelectionBroker.CurrentSnapshot, 0, _multiSelectionBroker.CurrentSnapshot.Length)))
                    {
                        var insertionPoint = selection.InsertionPoint;
                        if (isOverwriteModeEnabled && !insertionPoint.IsInVirtualSpace)
                        {
                            var position = insertionPoint.Position;
                            virtualSnapshotSpanList.Add(new VirtualSnapshotSpan(new SnapshotSpan(position, TextView.GetTextElementSpan(position).End)));
                        }
                        else
                            virtualSnapshotSpanList.Add(new VirtualSnapshotSpan(insertionPoint, insertionPoint));
                    }
                    source = virtualSnapshotSpanList;
                }
                var version = TextView.TextSnapshot.Version;
                var trackingSpan = (ITrackingSpan)null;
                var flag1 = true;
                var nullable = new int?();
                var num1 = 0;
                var trackingPoint = (ITrackingPoint)null;
                using (var edit = TextView.TextBuffer.CreateEdit(EditOptions.None, new int?(), textEditAction))
                {
                    var flag2 = true;
                    foreach (var virtualSnapshotSpan in source)
                    {
                        var replaceWith = text;
                        var textSnapshot = TextView.TextSnapshot;
                        var start = virtualSnapshotSpan.Start;
                        var position = (int)start.Position;
                        var num2 = 1;
                        trackingPoint = textSnapshot.CreateTrackingPoint(position, (PointTrackingMode)num2);
                        start = virtualSnapshotSpan.Start;
                        if (start.IsInVirtualSpace)
                        {
                            var whitespaceForVirtualSpace = GetWhitespaceForVirtualSpace(virtualSnapshotSpan.Start);
                            if (flag2)
                                TextView.TextBuffer.Properties["WhitespaceInserted"] = whitespaceForVirtualSpace.Length;
                            replaceWith = whitespaceForVirtualSpace + text;
                        }
                        if (!nullable.HasValue)
                            nullable = new int?(replaceWith.Length - text.Length);
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
                    if (_multiSelectionBroker.IsBoxSelection)
                    {
                        TextView.Caret.MoveTo(TextView.Caret.Position.BufferPosition);
                        TextView.Selection.Select(new VirtualSnapshotPoint(TextView.Selection.AnchorPoint.Position), new VirtualSnapshotPoint(TextView.Selection.ActivePoint.Position));
                        var virtualSelectedSpans = TextView.Selection.VirtualSelectedSpans;
                    bool Func(VirtualSnapshotSpan s) => !s.IsEmpty;
                    if (virtualSelectedSpans.Any(Func))
                            TextView.Selection.Clear();
                    }
                    else
                        _multiSelectionBroker.PerformActionOnAllSelections(transformer =>
                        {
                            var point = new VirtualSnapshotPoint(transformer.Selection.InsertionPoint.Position, 0);
                            transformer.MoveTo(point, false, PositionAffinity.Successor);
                        });
                    TextView.Caret.EnsureVisible();
                    AddAfterTextBufferChangePrimitive();
                    //transaction.Complete();
                    if (final)
                        trackingSpan = null;
                    else if (TextView.Selection.IsReversed)
                    {
                        var num2 = nullable ?? 0;
                        trackingSpan = version.Next.CreateTrackingSpan(new Span(source.First().Start.Position + num2, text.Length), SpanTrackingMode.EdgeExclusive);
                    }
                    else
                    {
                        var position = trackingPoint.GetPoint(TextView.TextSnapshot).Position;
                        trackingSpan = version.Next.CreateTrackingSpan(new Span(position + num1, text.Length), SpanTrackingMode.EdgeExclusive);
                    }
                }
                if (_immProvisionalComposition != trackingSpan)
                {
                    _immProvisionalComposition = trackingSpan;
                    TextView.ProvisionalTextHighlight = _immProvisionalComposition;
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

        private bool PositionCaretWithSmartIndent(ISelectionTransformer transformer, bool useOnlyVirtualSpace = true, bool extendSelection = false)
        {
            var insertionPoint = transformer.Selection.InsertionPoint;
            var containingLine = insertionPoint.Position.GetContainingLine();
            var desiredIndentation = _factory.SmartIndentationService.GetDesiredIndentation(TextView, containingLine);
            if (desiredIndentation.HasValue)
            {
                if (insertionPoint.Position == containingLine.End)
                {
                    var virtualSnapshotPoint = new VirtualSnapshotPoint(insertionPoint.Position, Math.Max(0, desiredIndentation.Value - containingLine.Length));
                    var anchorPoint = extendSelection ? transformer.Selection.AnchorPoint : virtualSnapshotPoint;
                    transformer.MoveTo(anchorPoint, virtualSnapshotPoint, virtualSnapshotPoint, PositionAffinity.Successor);
                    return true;
                }
                if (!useOnlyVirtualSpace)
                {
                    var leadingWhitespaceChars = GetLeadingWhitespaceChars(containingLine, insertionPoint.Position);
                    var positionAndVirtualSpace = GetWhiteSpaceForPositionAndVirtualSpace(insertionPoint.Position, desiredIndentation.Value);
                    return ReplaceHelper(new SnapshotSpan(insertionPoint.Position, leadingWhitespaceChars), positionAndVirtualSpace);
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
            var newCaret = (TextView.Selection.IsReversed ? containingBufferPosition1 : containingBufferPosition2).GetInsertionBufferPositionFromXCoordinate(xCoordinate);
            _multiSelectionBroker.ClearSecondarySelections();
            _multiSelectionBroker.TryPerformActionOnSelection(_multiSelectionBroker.PrimarySelection,
                transformer => transformer.MoveTo(newCaret, false, PositionAffinity.Successor), out _);
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
            //TODO: text
            using (var waitContext = WaitHelper.Wait(_factory.WaitIndicator, "Wait Title", "Wait Message"))
                return ((IRtfBuilderService2)_factory.RtfBuilderService).GenerateRtf(spans, waitContext.CancellationToken);
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

        private bool WillBackspaceCreateEdit()
        {
            VirtualSnapshotPoint virtualSnapshotPoint;
            if (_multiSelectionBroker.IsBoxSelection)
            {
                var primarySelection = _multiSelectionBroker.PrimarySelection;
                if (primarySelection.IsEmpty)
                {
                    virtualSnapshotPoint = primarySelection.Start;
                    var position = virtualSnapshotPoint.Position;
                    virtualSnapshotPoint = primarySelection.Start;
                    var start = virtualSnapshotPoint.Position.GetContainingLine().Start;
                    if (position == start)
                        return false;
                }
            }
            var allSelections = _multiSelectionBroker.AllSelections;
            foreach (var sel in allSelections)
            {
                var selection = sel;
                if (selection.Extent.SnapshotSpan.IsEmpty)
                {
                    selection = sel;
                    if (selection.IsEmpty)
                    {
                        selection = sel;
                        virtualSnapshotPoint = selection.InsertionPoint;
                        if (!virtualSnapshotPoint.IsInVirtualSpace)
                        {
                            selection = sel;
                            virtualSnapshotPoint = selection.InsertionPoint;
                            if (virtualSnapshotPoint.Position.Position == 0)
                                continue;
                        }
                        else
                            continue;
                    }
                    else
                        continue;
                }
                return true;
            }
            return false;
        }

        private bool TryBackspaceEdit(IReadOnlyList<Selection> selections)
        {
            using (var edit = TextView.TextBuffer.CreateEdit())
            {
                for (var index = selections.Count - 1; index >= 0; --index)
                {
                    var selection = selections[index];
                    VirtualSnapshotSpan extent;
                    if (selection.IsEmpty)
                    {
                        extent = selection.Extent;
                        if (!extent.IsInVirtualSpace && !TryBackspaceEmptySelection(selection, edit))
                            return false;
                    }
                    else
                    {
                        var textEdit = edit;
                        extent = selection.Extent;
                        Span snapshotSpan = extent.SnapshotSpan;
                        if (!textEdit.Delete(snapshotSpan))
                            return false;
                    }
                }
                edit.Apply();
                return !edit.Canceled;
            }
        }

        private bool TryBackspaceEmptySelection(Selection selection, ITextEdit edit)
        {
            if (selection.InsertionPoint.Position.Position == 0)
                return true;
            var textView = TextView;
            var insertionPoint = selection.InsertionPoint;
            var point = insertionPoint.Position - 1;
            var textElementSpan = textView.GetTextElementSpan(point);
            if (textElementSpan.Length > 0)
            {
                var textViewModel = TextView.TextViewModel;
                insertionPoint = selection.InsertionPoint;
                var position = insertionPoint.Position;
                var num = 1;
                if (textViewModel.IsPointInVisualBuffer(position, (PositionAffinity)num) && !TextView.TextViewModel.IsPointInVisualBuffer(textElementSpan.End - 1, PositionAffinity.Successor))
                    return edit.Delete(textElementSpan);
            }
            var snapshot = edit.Snapshot;
            insertionPoint = selection.InsertionPoint;
            var index = insertionPoint.Position.Position - 1;
            var start = index;
            var c = snapshot[index];
            if (char.GetUnicodeCategory(c) == UnicodeCategory.Surrogate)
                --start;
            if (start > 0 && c == '\n' && snapshot[index - 1] == '\r')
                --start;
            return edit.Delete(new Span(start, index - start + 1));
        }

        private bool TryPostBackspaceSelectionUpdate(IReadOnlyList<Selection> selections, Selection primarySelection, Selection boxSelection)
        {
            if (boxSelection != Selection.Invalid)
            {
                var selection1 = _multiSelectionBroker.BoxSelection;
                var virtualSnapshotPoint1 = selection1.AnchorPoint;
                selection1 = _multiSelectionBroker.BoxSelection;
                var virtualSnapshotPoint2 = selection1.ActivePoint;
                if (primarySelection.IsEmpty)
                {
                    VirtualSnapshotPoint virtualSnapshotPoint3;
                    if (boxSelection.AnchorPoint.IsInVirtualSpace)
                    {
                        ref var local = ref virtualSnapshotPoint1;
                        selection1 = _multiSelectionBroker.BoxSelection;
                        virtualSnapshotPoint3 = selection1.AnchorPoint;
                        var position = virtualSnapshotPoint3.Position;
                        virtualSnapshotPoint3 = boxSelection.AnchorPoint;
                        var virtualSpaces = virtualSnapshotPoint3.VirtualSpaces - 1;
                        local = new VirtualSnapshotPoint(position, virtualSpaces);
                    }
                    virtualSnapshotPoint3 = boxSelection.ActivePoint;
                    if (virtualSnapshotPoint3.IsInVirtualSpace)
                    {
                        ref var local = ref virtualSnapshotPoint2;
                        selection1 = _multiSelectionBroker.BoxSelection;
                        virtualSnapshotPoint3 = selection1.ActivePoint;
                        var position = virtualSnapshotPoint3.Position;
                        virtualSnapshotPoint3 = boxSelection.ActivePoint;
                        var virtualSpaces = virtualSnapshotPoint3.VirtualSpaces - 1;
                        local = new VirtualSnapshotPoint(position, virtualSpaces);
                    }
                }
                else
                {
                    selection1 = selections[boxSelection.IsReversed ? 0 : selections.Count - 1];
                    virtualSnapshotPoint2 = selection1.Start;
                    selection1 = selections[boxSelection.IsReversed ? selections.Count - 1 : 0];
                    virtualSnapshotPoint1 = selection1.Start;
                }
                var anchorPoint = virtualSnapshotPoint1.TranslateTo(TextView.TextSnapshot);
                var virtualSnapshotPoint4 = virtualSnapshotPoint2.TranslateTo(TextView.TextSnapshot);
                var selection2 = new Selection(virtualSnapshotPoint4, anchorPoint, virtualSnapshotPoint4, boxSelection.InsertionPointAffinity);
                if (_multiSelectionBroker.BoxSelection != selection2)
                    _multiSelectionBroker.SetBoxSelection(selection2);
            }
            else
            {
                foreach (var selection in selections)
                {
                    Selection after;
                    _multiSelectionBroker.TryPerformActionOnSelection(selection, transformer =>
                    {
                        VirtualSnapshotPoint virtualSnapshotPoint;
                        if (selection.IsEmpty)
                        {
                            virtualSnapshotPoint = selection.InsertionPoint;
                            if (virtualSnapshotPoint.IsInVirtualSpace)
                            {
                                var selectionTransformer = transformer;
                                virtualSnapshotPoint = transformer.Selection.InsertionPoint;
                                var position = virtualSnapshotPoint.Position;
                                virtualSnapshotPoint = selection.InsertionPoint;
                                var virtualSpaces = virtualSnapshotPoint.VirtualSpaces - 1;
                                var point = new VirtualSnapshotPoint(position, virtualSpaces);
                                var num1 = 0;
                                var num2 = 1;
                                selectionTransformer.MoveTo(point, num1 != 0, (PositionAffinity)num2);
                                return;
                            }
                        }
                        var selectionTransformer1 = transformer;
                        virtualSnapshotPoint = transformer.Selection.InsertionPoint;
                        var position1 = virtualSnapshotPoint.Position;
                        virtualSnapshotPoint = selection.Start;
                        var virtualSpaces1 = virtualSnapshotPoint.VirtualSpaces;
                        var point1 = new VirtualSnapshotPoint(position1, virtualSpaces1);
                        var num3 = 0;
                        var num4 = 1;
                        selectionTransformer1.MoveTo(point1, num3 != 0, (PositionAffinity)num4);
                    }, out after);
                }
            }
            return true;
        }

        private bool TryBackspaceSelections()
        {
            if (_multiSelectionBroker.IsBoxSelection && _multiSelectionBroker.PrimarySelection.InsertionPoint.IsInVirtualSpace)
                _multiSelectionBroker.SetSelection(new Selection(_multiSelectionBroker.PrimarySelection.Start));
            else if (!_multiSelectionBroker.IsBoxSelection)
                _multiSelectionBroker.PerformActionOnAllSelections(transformer =>
                {
                    if (transformer.Selection.IsEmpty)
                    {
                        if (transformer.Selection.InsertionPoint.IsInVirtualSpace)
                            MoveToPreviousTabStop(transformer);
                        else
                            transformer.PerformAction(PredefinedSelectionTransformations.MoveToPreviousCaretPosition);
                    }
                    else
                        transformer.MoveTo(transformer.Selection.Start, false, PositionAffinity.Successor);
                });
            return true;
        }

        private void MoveToPreviousTabStop(ISelectionTransformer transformer)
        {
            var stopInVirtualSpace = GetPreviousIndentStopInVirtualSpace(transformer.Selection.InsertionPoint);
            transformer.MoveTo(stopInVirtualSpace, false, PositionAffinity.Successor);
        }
    }
}