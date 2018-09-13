using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        private bool IndentOperationShouldBeMultiLine
        {
            get
            {
                if (TextView.Selection.IsEmpty)
                    return false;
                var containingLine = TextView.Selection.Start.Position.GetContainingLine();
                var num1 = TextView.Selection.End.Position <= containingLine.End ? 1 : 0;
                var flag = containingLine.End == containingLine.EndIncludingLineBreak &&
                            TextView.Selection.Start.Position == containingLine.Start &&
                            TextView.Selection.End.Position == containingLine.End;
                return num1 == 0 | flag;
            }
        }

        public ITextView TextView { get; }

        
        internal EditorOperations(ITextView textView, EditorOperationsFactoryService factory)
        {
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _editorPrimitives = factory.EditorPrimitivesProvider.GetViewPrimitives(textView);
            _multiSelectionBroker = TextView.GetMultiSelectionBroker();
            _textStructureNavigator =
                factory.TextStructureNavigatorFactory.GetTextStructureNavigator(TextView.TextBuffer);
            //TODO: Add Undo Stuff
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
            //TODO: Capitalize
            return ExecuteAction("Capitalize", () => _editorPrimitives.Selection.Capitalize());
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
            var fullSelectedLines = GetFullLines();
            if (fullSelectedLines.TextBuffer.AdvancedTextBuffer.IsReadOnly(fullSelectedLines.AdvancedTextRange))
                return false;
            var putCopiedLineToClipboard = PrepareClipboardFullLineCopy(fullSelectedLines);
            //Todo: localize
            return ExecuteAction("CutLine", () =>
            {
                if (fullSelectedLines.Delete())
                    return putCopiedLineToClipboard();
                return false;
            });
        }

        public bool CutSelection()
        {
            if (TextView.Selection.IsEmpty && !CanCut)
                return true;

            //TODO: undo
            //using (var transaction = _undoHistory.CreateTransaction(Strings.CutSelection))
            //{
            AddBeforeTextBufferChangePrimitive();
            var flag = false;
            if (!TextView.Selection.IsEmpty)
                flag = PrepareClipboardSelectionCopy()() && Delete();
            else
            {
                var fullLines = GetFullLines();
                var left = TextView.Caret.Left;
                var func = PrepareClipboardFullLineCopy(fullLines);
                if ((fullLines).Delete() && func())
                {
                    flag = true;
                    TextView.Caret.MoveTo(TextView.Caret.ContainingTextViewLine, left);
                }
            }

            if (flag)
            {
                TextView.Caret.EnsureVisible();
                AddAfterTextBufferChangePrimitive();
                //transaction.Complete();
            }
            //else
            //    transaction.Cancel();
            return flag;
            //}
        }

        public bool DecreaseLineIndent()
        {
            //TODO: Localize
            return ExecuteAction("DecreaseLineIndent", () => PerformIndentActionOnEachBufferLine(RemoveIndentAtPoint), SelectionUpdate.Ignore, true);
        }

        public bool Delete()
        {
            bool flag;
            if (WillDeleteCreateEdit())
            {
                var selections = _multiSelectionBroker.AllSelections;
                var boxSelection = _multiSelectionBroker.BoxSelection;
                var primarySelection = _multiSelectionBroker.PrimarySelection;
                //TODO: Localize
                flag = ExecuteAction("Delete Char Right", () =>
                {
                    using (_multiSelectionBroker.BeginBatchOperation())
                    {
                        if (TryDeleteEdit(selections))
                            return TryPostDeleteSelectionUpdate(selections, primarySelection, boxSelection);
                    }
                    return false;
                });
            }
            else
                flag = TryDeleteSelections();

            if (flag)
                _multiSelectionBroker.TryEnsureVisible(_multiSelectionBroker.PrimarySelection,
                    EnsureSpanVisibleOptions.MinimumScroll);
            return flag;
        }

        public bool DeleteBlankLines()
        {
            return ExecuteAction("Delete Blank Lines", () =>
            {
                double left = TextView.Caret.Left;
                using (var edit = _editorPrimitives.Buffer.AdvancedTextBuffer.CreateEdit())
                {
                    var lineNumber1 = _editorPrimitives.Selection.GetStartPoint().LineNumber;
                    var lineNumber2 = _editorPrimitives.Selection.GetEndPoint().LineNumber;
                    if (_editorPrimitives.Selection.IsEmpty)
                    {
                        var textPoint = _editorPrimitives.Buffer.GetTextPoint(lineNumber1, 0);
                        if (IsPointOnBlankLine(textPoint) || _editorPrimitives.Caret.CurrentPosition == _editorPrimitives.Caret.EndOfLine)
                        {
                            while (lineNumber2 < _editorPrimitives.Selection.AdvancedSelection.TextView.TextSnapshot
                                       .LineCount - 1 &&
                                   IsPointOnBlankLine(_editorPrimitives.Buffer.GetTextPoint(lineNumber2 + 1, 0)))
                                ++lineNumber2;
                        }

                        if (lineNumber1 == lineNumber2 && !IsPointOnBlankLine(textPoint))
                        {
                            while (lineNumber1 > 0 && IsPointOnBlankLine(_editorPrimitives.Buffer.GetTextPoint(lineNumber1 - 1, 0)))
                                --lineNumber1;
                        }
                    }

                    for (int index = lineNumber1; index <= lineNumber2 ; ++index)
                    {
                        var textPoint1 = _editorPrimitives.Buffer.GetTextPoint(index, 0);
                        if (IsPointOnBlankLine(textPoint1))
                        {
                            var textPoint2 = textPoint1.Clone();
                            textPoint2.MoveToBeginningOfNextLine();
                            if (!edit.Delete(Span.FromBounds(textPoint1.CurrentPosition, textPoint2.CurrentPosition)))
                                return false;
                        }
                    }

                    edit.Apply();
                    if (edit.Canceled)
                        return false;
                    TextView.Caret.EnsureVisible();
                    TextView.Caret.MoveTo(TextView.Caret.ContainingTextViewLine, left);
                    return true;
                }
            });
        }

        public bool DeleteFullLine()
        {
            //TODO: localize
            return ExecuteAction("Delete Line", () => GetFullLines().Delete());
        }

        public bool DeleteHorizontalWhiteSpace()
        {
            //Todo: localize
            return ExecuteAction("Delete horizontal whitespace", DeleteHorizontalWhitespace);
        }

        public bool DeleteToBeginningOfLine()
        {
            //Todo: localize
            return ExecuteAction("DeleteToBol", () =>
            {
                TextRange textRange = _editorPrimitives.Selection.Clone();
                if (_editorPrimitives.Selection.IsReversed ||
                    _editorPrimitives.Selection.IsEmpty)
                    textRange.SetStart(_editorPrimitives.View.GetTextPoint(_editorPrimitives.Caret.StartOfLine));
                return textRange.Delete();
            });
        }

        public bool DeleteToEndOfLine()
        {
            //Todo: localize
            return ExecuteAction("DeleteToEol", () =>
            {
                TextRange textRange = _editorPrimitives.Selection.Clone();
                if (!_editorPrimitives.Selection.IsReversed ||
                    _editorPrimitives.Selection.IsEmpty)
                    textRange.SetEnd(_editorPrimitives.View.GetTextPoint(_editorPrimitives.Caret.EndOfViewLine));
                return textRange.Delete();
            });
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
            if (lineNumber < 0 || lineNumber > TextView.TextSnapshot.LineCount - 1)
                throw new ArgumentOutOfRangeException(nameof(lineNumber));
            TextView.Caret.MoveTo(TextView.TextSnapshot.GetLineFromLineNumber(lineNumber).Start);
            TextView.Selection.Clear();
            TextView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(TextView.Caret.Position.BufferPosition, 0));
        }

        public bool IncreaseLineIndent()
        {
            //TODO: Localize
            return ExecuteAction("IncreaseLineIndent", () => PerformIndentActionOnEachBufferLine(InsertSingleIndentAtPoint), SelectionUpdate.Ignore, true);
        }

        public bool Indent()
        {
            var insertTabs = TextView.Selection.Mode == TextSelectionMode.Box || !IndentOperationShouldBeMultiLine;
            //TODO: Localize
            return ExecuteAction("Insert Tab", () =>
            {
                if (insertTabs)
                {
                    return EditHelper(edit =>
                    {
                        Options.GetTabSize();
                        var indentSize = Options.GetIndentSize();
                        Options.IsConvertTabsToSpacesEnabled();
                        var num = TextView.Selection.Mode != TextSelectionMode.Box
                            ? 0
                            : (TextView.Selection.Start != TextView.Selection.End ? 1 : 0);
                        var anchorPoint = num != 0
                            ? CalculateBoxIndentForSelectionPoint(TextView.Selection.AnchorPoint, indentSize)
                            : new VirtualSnapshotPoint?();
                        var activePoint = num != 0
                            ? CalculateBoxIndentForSelectionPoint(TextView.Selection.ActivePoint, indentSize)
                            : new VirtualSnapshotPoint?();
                        foreach (var virtualSelectedSpan in TextView.Selection.VirtualSelectedSpans)
                        {
                            if (!InsertIndentForSpan(virtualSelectedSpan, edit, false))
                                return false;
                        }
                        FixUpSelectionAfterBoxOperation(anchorPoint, activePoint);
                        return true;
                    });
                }

                return PerformIndentActionOnEachBufferLine(InsertSingleIndentAtPoint);

            }, TextView.Selection.IsEmpty ? SelectionUpdate.ClearVirtualSpace : SelectionUpdate.Ignore, insertTabs);
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
                                if (!PositionCaretWithSmartIndent(transformer, false))
                                {
                                    var insertionPoint = transformer.Selection.InsertionPoint;
                                    if (insertionPoint.IsInVirtualSpace)
                                    {
                                        var selectionTransformer = transformer;
                                        insertionPoint = transformer.Selection.InsertionPoint;
                                        var point = new VirtualSnapshotPoint(insertionPoint.Position);
                                        selectionTransformer.MoveTo(point, false, (PositionAffinity)1);
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
            //TODO: localize
            return InsertTextAsBox(text, out boxStart, out boxEnd, "Insert");
        }

        public bool InsertTextAsBox(string text, out VirtualSnapshotPoint boxStart, out VirtualSnapshotPoint boxEnd,
            string undoText)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            var position = TextView.Caret.Position;
            VirtualSnapshotPoint virtualBufferPosition;
            var virtualSnapshotPoint1 = virtualBufferPosition = position.VirtualBufferPosition;
            boxEnd = virtualBufferPosition;
            var virtualSnapshotPoint2 = virtualSnapshotPoint1;
            boxStart = virtualSnapshotPoint2;
            VirtualSnapshotPoint newEnd;
            var newStart = newEnd = boxStart;
            if (text.Length == 0)
                return InsertText(text);

            bool Action()
            {
                if (!DeleteHelper(TextView.Selection.SelectedSpans)) return false;
                TextView.Selection.Mode = TextSelectionMode.Box;
                TextView.Caret.MoveTo(TextView.Selection.Start);
                var virtualSnapshotPoint3 = TextView.Caret.Position.VirtualBufferPosition;
                var left = TextView.Caret.Left;
                var caretColumn = _editorPrimitives.View.GetTextPoint(virtualSnapshotPoint3.Position).DisplayColumn + virtualSnapshotPoint3.VirtualSpaces;
                ITrackingPoint trackingPoint = null;
                var nullable = new int?();
                using (var edit = (_editorPrimitives).Buffer.AdvancedTextBuffer.CreateEdit())
                {
                    var point = virtualSnapshotPoint3;
                    var containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(point.Position);
                    var flag = false;
                    using (var stringReader = new StringReader(text))
                    {
                        var text1 = stringReader.ReadLine();
                        while (text1 != null)
                        {
                            if (text1.Length > 0)
                            {
                                trackingPoint = TextView.TextSnapshot.CreateTrackingPoint(point.Position, PointTrackingMode.Positive);
                                var num = 0;
                                if (point.IsInVirtualSpace)
                                {
                                    var whitespaceForVirtualSpace = GetWhitespaceForVirtualSpace(point);
                                    text1 = whitespaceForVirtualSpace + text1;
                                    num = whitespaceForVirtualSpace.Length;
                                }

                                if (!nullable.HasValue)
                                {
                                    nullable = num;
                                    virtualSnapshotPoint3 = point;
                                }

                                if (!edit.Insert(point.Position, text1)) return false;
                            }

                            if (!flag)
                            {
                                if (containingBufferPosition.LineBreakLength == 0)
                                {
                                    var forDisplayColumn = GetWhitespaceForDisplayColumn(caretColumn);
                                    var newLineCharacter = Options.GetNewLineCharacter();
                                    text1 = string.Empty;
                                    var empty = string.Empty;
                                    string str;
                                    while ((str = stringReader.ReadLine()) != null)
                                    {
                                        if (str.Length > 0)
                                        {
                                            text1 = text1 + empty + newLineCharacter + forDisplayColumn + str;
                                            empty = string.Empty;
                                        }
                                        else
                                            empty += newLineCharacter;
                                    }

                                    flag = true;
                                    point = new VirtualSnapshotPoint(containingBufferPosition.EndIncludingLineBreak);
                                }
                                else
                                {
                                    text1 = stringReader.ReadLine();
                                    if (text1 != null)
                                    {
                                        containingBufferPosition = TextView.GetTextViewLineContainingBufferPosition(containingBufferPosition.EndIncludingLineBreak);
                                        point = containingBufferPosition.GetInsertionBufferPositionFromXCoordinate(left);
                                    }
                                }
                            }
                            else
                                break;
                        }
                    }

                    if (trackingPoint == null)
                    {
                        edit.Cancel();
                        return false;
                    }

                    edit.Apply();
                    if (edit.Canceled) return false;
                }

                TextView.Selection.Clear();
                var num1 = nullable.HasValue ? nullable.Value : 0;
                TextView.Caret.MoveTo(new SnapshotPoint(TextView.TextSnapshot, virtualSnapshotPoint3.Position.Position + num1));
                newStart = TextView.Caret.Position.VirtualBufferPosition;
                newEnd = new VirtualSnapshotPoint(trackingPoint.GetPoint(TextView.TextSnapshot));
                return true;
            }

            var num2 = ExecuteAction(undoText, Action) ? 1 : 0;
            if (num2 == 0)
                return num2 != 0;
            boxStart = newStart;
            boxEnd = newEnd;
            return num2 != 0;
        }

        public bool MakeLowercase()
        {
            return ChangeCase(LetterCase.Lowercase);
        }

        public bool MakeUppercase()
        {
            return ChangeCase(LetterCase.Uppercase);
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
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToNextLine
                : PredefinedSelectionTransformations.MoveToNextLine);
            TextView.Caret.EnsureVisible();
        }

        public void MoveLineUp(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToPreviousLine
                : PredefinedSelectionTransformations.MoveToPreviousLine);
            TextView.Caret.EnsureVisible();
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
            var textViewLines = _editorPrimitives.View.AdvancedTextView.TextViewLines;
            var lastVisibleLine = textViewLines.LastVisibleLine;
            var indexOfTextLine = textViewLines.GetIndexOfTextLine(lastVisibleLine);
            MoveCaretToTextLine(FindFullyVisibleLine(lastVisibleLine, indexOfTextLine - 1), extendSelection);
        }

        public void MoveToEndOfDocument(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToEndOfDocument
                : PredefinedSelectionTransformations.MoveToEndOfDocument);
            TextView.Caret.EnsureVisible();
        }

        public void MoveToEndOfLine(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToEndOfLine
                : PredefinedSelectionTransformations.MoveToEndOfLine);
            TextView.Caret.EnsureVisible();
        }

        public void MoveToHome(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToHome
                : PredefinedSelectionTransformations.MoveToHome);
            TextView.Caret.EnsureVisible();
        }

        public void MoveToLastNonWhiteSpaceCharacter(bool extendSelection)
        {
            var num = _editorPrimitives.Caret.EndOfViewLine - 1;
            while (num >= _editorPrimitives.Caret.StartOfViewLine && char.IsWhiteSpace(_editorPrimitives.View.GetTextPoint(num).GetNextCharacter()[0]))
                --num;
            var position = Math.Max(num, _editorPrimitives.Caret.StartOfLine);
            if (position == _editorPrimitives.Caret.CurrentPosition)
                return;
            _editorPrimitives.Caret.MoveTo(position, extendSelection);
        }

        public void MoveToNextCharacter(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToNextCaretPosition
                : PredefinedSelectionTransformations.MoveToNextCaretPosition);
            TextView.Caret.EnsureVisible();
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
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectToStartOfDocument
                : PredefinedSelectionTransformations.MoveToStartOfDocument);
            TextView.Caret.EnsureVisible();
        }

        public void MoveToStartOfLine(bool extendSelection)
        {
            throw new NotImplementedException();
        }

        public void MoveToStartOfLineAfterWhiteSpace(bool extendSelection)
        {
            var position = _editorPrimitives.Caret.GetFirstNonWhiteSpaceCharacterOnViewLine().CurrentPosition;
            if (position == _editorPrimitives.Caret.EndOfViewLine)
                position = _editorPrimitives.Caret.StartOfViewLine;
            _editorPrimitives.Caret.MoveTo(position, extendSelection);
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
            var textViewLines = _editorPrimitives.View.AdvancedTextView.TextViewLines;
            var firstVisibleLine = textViewLines.FirstVisibleLine;
            var indexOfTextLine = textViewLines.GetIndexOfTextLine(firstVisibleLine);
            MoveCaretToTextLine(FindFullyVisibleLine(firstVisibleLine, indexOfTextLine + 1), extendSelection);
        }

        public bool NormalizeLineEndings(string replacement)
        {
            throw new NotImplementedException();
        }

        public bool OpenLineAbove()
        {
            //TODO: localize
            return ExecuteAction("open live above", () =>
            {
                var displayTextPoint = _editorPrimitives.Caret.Clone();
                bool flag;
                if (displayTextPoint.LineNumber == 0)
                {
                    _editorPrimitives.Caret.MoveTo(0);
                    flag = InsertNewLine();
                    if (flag)
                        _editorPrimitives.Caret.MoveTo(0);
                }
                else
                {
                    displayTextPoint.MoveToBeginningOfPreviousViewLine();
                    displayTextPoint.MoveToEndOfViewLine();
                    _editorPrimitives.Caret.MoveTo(displayTextPoint.CurrentPosition);
                    flag = InsertNewLine();
                }
                return flag;
            });
        }

        public bool OpenLineBelow()
        {
            //TODO: localize
            return ExecuteAction("open line below", () =>
            {
                _editorPrimitives.Caret.MoveToEndOfViewLine();
                return InsertNewLine();
            });
        }

        public void PageDown(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectPageDown
                : PredefinedSelectionTransformations.MovePageDown);
        }

        public void PageUp(bool extendSelection)
        {
            _multiSelectionBroker.PerformActionOnAllSelections(extendSelection
                ? PredefinedSelectionTransformations.SelectPageUp
                : PredefinedSelectionTransformations.MovePageUp);
        }

        public bool Paste()
        {
            string str1;
            bool dataPresent1;
            bool dataPresent2;
            try
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                if (dataObject == null || !dataObject.GetDataPresent(typeof(string)))
                    return true;
                str1 = (string)dataObject.GetData(DataFormats.UnicodeText) ?? (string)dataObject.GetData(DataFormats.Text);
                dataPresent1 = dataObject.GetDataPresent("EditorOperationsLineCutCopyClipboardTag");
                dataPresent2 = dataObject.GetDataPresent("ColumnSelect");
            }
            catch (ExternalException)
            {
                return false;
            }
            catch (OutOfMemoryException)
            {
                return false;
            }
            if (str1 == null)
                return true;
            if (dataPresent1 && TextView.Selection.IsEmpty)
            {
                //TODO: undo stuff
                //using (ITextUndoTransaction transaction = this._undoHistory.CreateTransaction(Strings.Paste))
                //{
                AddBeforeTextBufferChangePrimitive();
                SnapshotPoint start = TextView.Caret.Position.BufferPosition.GetContainingLine().Start;
                using (ITextEdit edit = TextView.TextBuffer.CreateEdit())
                {
                    if (!edit.Insert(start.Position, str1))
                        return false;
                    edit.Apply();
                }

                AddAfterTextBufferChangePrimitive();
                //transaction.Complete();
                TextView.Caret.EnsureVisible();
                return true;
                //}
            }
            else
            {
                //TODO: localize
                if (!dataPresent2)
                    return InsertText(str1, true, "Paste", false);
                if (!TextView.Selection.IsEmpty || !IsPointOnBlankViewLine(_editorPrimitives.Caret))
                {
                    VirtualSnapshotPoint boxStart;
                    VirtualSnapshotPoint boxEnd;
                    return InsertTextAsBox(str1, out boxStart, out boxEnd, "Paste");
                }
                string forDisplayColumn = GetWhitespaceForDisplayColumn(_editorPrimitives.Caret.Column + TextView.Caret.Position.VirtualBufferPosition.VirtualSpaces);
                List<string> stringList = new List<string>();
                using (StringReader stringReader = new StringReader(str1))
                {
                    for (string str2 = stringReader.ReadLine(); str2 != null; str2 = stringReader.ReadLine())
                        stringList.Add(str2);
                }

                return InsertText(
                    string.Join(Options.GetNewLineCharacter() + forDisplayColumn, stringList)
                        .ToString(CultureInfo.CurrentCulture), true, "Paste", false);
            }

            return false;
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
            TextView.ViewScroller.ScrollViewportHorizontallyByPixels(TextView.FormattedLineSource.ColumnWidth * -1.0);
        }

        public void ScrollColumnRight()
        {
            TextView.ViewScroller.ScrollViewportHorizontallyByPixels(TextView.FormattedLineSource.ColumnWidth);
        }

        public void ScrollDownAndMoveCaretIfNecessary()
        {
            ScrollByLineAndMoveCaretIfNecessary(ScrollDirection.Down);
        }

        public void ScrollLineBottom()
        {
            _editorPrimitives.View.MoveLineToBottom(_editorPrimitives.Caret.LineNumber);
        }

        public void ScrollLineCenter()
        {
            _editorPrimitives.View.Show(_editorPrimitives.Caret, HowToShow.Centered);
        }

        public void ScrollLineTop()
        {
            _editorPrimitives.View.MoveLineToTop(_editorPrimitives.Caret.LineNumber);
        }

        public void ScrollPageDown()
        {
            _editorPrimitives.View.ScrollPageDown();
        }

        public void ScrollPageUp()
        {
            _editorPrimitives.View.ScrollPageUp();
        }

        public void ScrollUpAndMoveCaretIfNecessary()
        {
            ScrollByLineAndMoveCaretIfNecessary(ScrollDirection.Up);
        }

        public void SelectAll()
        {
            _editorPrimitives.Selection.SelectAll();
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
            TextView.Selection.Select(TextView.Selection.ActivePoint, TextView.Selection.AnchorPoint);
            TextView.Caret.MoveTo(TextView.Selection.ActivePoint);
            TextView.Caret.EnsureVisible();
        }

        public bool Tabify()
        {
            //TODO: Localize
            return ConvertLeadingWhitespace("Tabify", false);
        }

        public bool ToggleCase()
        {
            //TODO: Localize
            return ExecuteAction("Toggle Case", () => _editorPrimitives.Selection.ToggleCase());
        }

        public bool TransposeCharacter()
        {
            //TODO: Localize
            return ExecuteAction("TransposeCharacter", () => _editorPrimitives.Caret.TransposeCharacter());
        }

        public bool TransposeLine()
        {
            if (TextView.TextSnapshot.LineCount < 2)
                return true;
            //TODO: Localize
            return ExecuteAction("TransposeLine", () => _editorPrimitives.Caret.TransposeLine());
        }

        public bool TransposeWord()
        {
            var currentWord = _editorPrimitives.Caret.GetCurrentWord();
            if (currentWord.IsEmpty)
                currentWord = currentWord.GetStartPoint().GetPreviousWord();
            if (currentWord.GetEndPoint().CurrentPosition == _editorPrimitives.Buffer.GetEndPoint().CurrentPosition)
                return true;
            //TODO: Localize
            return ExecuteAction("TransposeWord", () =>
            {
                var nextWord = currentWord.GetEndPoint().GetNextWord();
                bool Func(TextRange tr) => tr.GetEndPoint().CurrentPosition != _editorPrimitives.Buffer.GetEndPoint().CurrentPosition && (tr.IsEmpty || !char.IsLetterOrDigit(tr.GetText()[0]));
                while (Func(nextWord))
                    nextWord = nextWord.GetEndPoint().GetNextWord();
                var currentPosition = nextWord.GetEndPoint().CurrentPosition;
                using (var edit = _editorPrimitives.Buffer.AdvancedTextBuffer.CreateEdit())
                {
                    if (!edit.Replace(currentWord.AdvancedTextRange.Span, nextWord.GetText()) || !edit.Replace(nextWord.AdvancedTextRange.Span, currentWord.GetText()))
                        return false;
                    edit.Apply();
                    if (edit.Canceled)
                        return false;
                }
                _editorPrimitives.Caret.MoveTo(currentPosition);
                return true;
            });
        }

        public bool Unindent()
        {
            var flag = TextView.Selection.Mode == TextSelectionMode.Box &&
                       TextView.Selection.Start != TextView.Selection.End;
            if (TextView.Caret.InVirtualSpace && TextView.Selection.IsEmpty)
            {
                MoveCaretToPreviousIndentStopInVirtualSpace();
                return true;
            }
            Func<bool> action;
            if (!flag && IndentOperationShouldBeMultiLine)
                action = () => PerformIndentActionOnEachBufferLine(RemoveIndentAtPoint);
            else if (!flag)
                action = () => EditHelper(edit => RemoveIndentAtPoint(TextView.Selection.Start.Position, edit, false, false, new int?()));
            else
            {
                var columnsToRemove = DetermineMaxBoxUnindent();
                action = () => EditHelper(edit =>
                {
                    var forSelectionPoint1 = CalculateBoxUnindentForSelectionPoint(TextView.Selection.AnchorPoint, columnsToRemove);
                    var forSelectionPoint2 = CalculateBoxUnindentForSelectionPoint(TextView.Selection.ActivePoint, columnsToRemove);
                    foreach (var virtualSelectedSpan in TextView.Selection.VirtualSelectedSpans)
                    {
                        if (!RemoveIndentAtPoint(virtualSelectedSpan.Start.Position, edit, false, false, columnsToRemove))
                            return false;
                    }
                    FixUpSelectionAfterBoxOperation(forSelectionPoint1, forSelectionPoint2);
                    return true;
                });
            }
            //TODO: Localize
            return ExecuteAction("Backtab", action, SelectionUpdate.Ignore, true);
        }

        public bool Untabify()
        {
            return ConvertLeadingWhitespace("Untabify", true);
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
                            new SnapshotPoint(TextView.TextSnapshot,  anchorPoint1.Position),
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
                    {
                        ResetSelection();
                    }
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
            foreach (var t in allSelections)
            {
                var selection = t;
                if (selection.Extent.SnapshotSpan.IsEmpty)
                {
                    selection = t;
                    if (selection.IsEmpty)
                    {
                        selection = t;
                        virtualSnapshotPoint = selection.InsertionPoint;
                        if (!virtualSnapshotPoint.IsInVirtualSpace)
                        {
                            selection = t;
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
                foreach (var t in selections)
                {
                    _multiSelectionBroker.TryPerformActionOnSelection(t, transformer =>
                    {
                        VirtualSnapshotPoint virtualSnapshotPoint;
                        if (t.IsEmpty)
                        {
                            virtualSnapshotPoint = t.InsertionPoint;
                            if (virtualSnapshotPoint.IsInVirtualSpace)
                            {
                                var selectionTransformer = transformer;
                                virtualSnapshotPoint = transformer.Selection.InsertionPoint;
                                var position = virtualSnapshotPoint.Position;
                                virtualSnapshotPoint = t.InsertionPoint;
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
                        virtualSnapshotPoint = t.Start;
                        var virtualSpaces1 = virtualSnapshotPoint.VirtualSpaces;
                        var point1 = new VirtualSnapshotPoint(position1, virtualSpaces1);
                        selectionTransformer1.MoveTo(point1, false, (PositionAffinity) 1);
                    }, out _);
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

        private bool PerformIndentActionOnEachBufferLine(Func<SnapshotPoint, ITextEdit, bool> action)
        {
            bool EditAction(ITextEdit edit)
            {
                var textSnapshot = TextView.TextSnapshot;
                var numberFromPosition1 = textSnapshot.GetLineNumberFromPosition(TextView.Selection.Start.Position);
                var numberFromPosition2 = textSnapshot.GetLineNumberFromPosition(TextView.Selection.End.Position);
                for (var lineNumber = numberFromPosition1; lineNumber <= numberFromPosition2; ++lineNumber)
                {
                    var lineFromLineNumber = textSnapshot.GetLineFromLineNumber(lineNumber);
                    if (lineFromLineNumber.Length != 0 && (TextView.Selection.IsEmpty || !(lineFromLineNumber.Start == TextView.Selection.End.Position)))
                    {
                        var spaceCharacterOnLine = _editorPrimitives.Buffer.GetTextPoint(lineFromLineNumber.Start).GetFirstNonWhiteSpaceCharacterOnLine();
                        if (spaceCharacterOnLine.CurrentPosition != spaceCharacterOnLine.EndOfLine && !action(new SnapshotPoint(textSnapshot, spaceCharacterOnLine.CurrentPosition), edit))
                            return false;
                    }
                }
                return true;
            }
            var trackingPoint = TextView.TextSnapshot.CreateTrackingPoint(TextView.Selection.Start.Position, PointTrackingMode.Negative);
            int position = TextView.Selection.Start.Position;
            if (!EditHelper(EditAction))
                return false;
            var virtualSnapshotPoint = new VirtualSnapshotPoint(trackingPoint.GetPoint(TextView.TextSnapshot));
            if (virtualSnapshotPoint.Position == position)
            {
                var num = TextView.Selection.IsReversed ? 1 : 0;
                SelectAndMoveCaret(num != 0 ? TextView.Selection.End : virtualSnapshotPoint, num != 0 ? virtualSnapshotPoint : TextView.Selection.End);
            }
            return true;
        }

        private bool InsertSingleIndentAtPoint(SnapshotPoint point, ITextEdit edit)
        {
            var virtualSnapshotPoint = new VirtualSnapshotPoint(point);
            return InsertIndentForSpan(new VirtualSnapshotSpan(virtualSnapshotPoint, virtualSnapshotPoint), edit, true, true);
        }

        private bool InsertIndentForSpan(VirtualSnapshotSpan span, ITextEdit edit, bool exactlyOneIndentLevel, bool useBufferPrimitives = false)
        {
            var indentSize = TextView.Options.GetIndentSize();
            var spacesEnabled = TextView.Options.IsConvertTabsToSpacesEnabled();
            var flag = TextView.Selection.Mode == TextSelectionMode.Box && TextView.Selection.Start != TextView.Selection.End;
            var snapshot = edit.Snapshot;
            var start = span.Start;
            if (flag && start.IsInVirtualSpace)
                return true;
            var position = (int)start.Position;
            var num1 = flag ? position : span.End.Position;
            int num2;
            int num3;
            string positionAndVirtualSpace;
            if (!spacesEnabled)
            {
                while (position > 0 && snapshot[position - 1] == ' ')
                    --position;
                var column = (!useBufferPrimitives ? _editorPrimitives.View.GetTextPoint(position) : _editorPrimitives.Buffer.GetTextPoint(position)).Column;
                num2 = column + (start.Position - position) + start.VirtualSpaces;
                num3 = !exactlyOneIndentLevel ? indentSize - num2 % indentSize : indentSize;
                var virtualSpaces = num2 + num3 - column;
                positionAndVirtualSpace = GetWhiteSpaceForPositionAndVirtualSpace(new SnapshotPoint(snapshot, position), virtualSpaces, useBufferPrimitives);
            }
            else
            {
                num2 = (!useBufferPrimitives ? _editorPrimitives.View.GetTextPoint(start.Position.Position) : _editorPrimitives.Buffer.GetTextPoint(start.Position.Position)).Column + start.VirtualSpaces;
                num3 = !exactlyOneIndentLevel ? indentSize - num2 % indentSize : indentSize;
                positionAndVirtualSpace = GetWhiteSpaceForPositionAndVirtualSpace(start.Position, start.VirtualSpaces + num3, useBufferPrimitives);
            }
            if (TextView.Caret.OverwriteMode && span.IsEmpty)
            {
                var num4 = num2 + num3;
                var end = (int)start.Position.GetContainingLine().End;
                while (num1 < end && _editorPrimitives.View.GetTextPoint(num1).Column < num4)
                    ++num1;
            }
            return edit.Replace(Span.FromBounds(position, num1), positionAndVirtualSpace);
        }

        private VirtualSnapshotPoint? CalculateBoxIndentForSelectionPoint(VirtualSnapshotPoint point, int indentSize)
        {
            if (!point.IsInVirtualSpace)
                return new VirtualSnapshotPoint?();
            var num1 = _editorPrimitives.View.GetTextPoint(point.Position.Position).Column + point.VirtualSpaces;
            var num2 = indentSize - num1 % indentSize;
            return new VirtualSnapshotPoint(point.Position, point.VirtualSpaces + num2);
        }

        private void FixUpSelectionAfterBoxOperation(VirtualSnapshotPoint? anchorPoint, VirtualSnapshotPoint? activePoint)
        {
            if (!anchorPoint.HasValue && !activePoint.HasValue)
                return;
            SelectAndMoveCaret(anchorPoint?.TranslateTo(TextView.TextSnapshot) ?? TextView.Selection.AnchorPoint,
                activePoint?.TranslateTo(TextView.TextSnapshot) ?? TextView.Selection.ActivePoint,
                TextSelectionMode.Box, EnsureSpanVisibleOptions.None);
        }

        private bool RemoveIndentAtPoint(SnapshotPoint point, ITextEdit edit)
        {
            return RemoveIndentAtPoint(point, edit, true, true, new int?());
        }

        private bool RemoveIndentAtPoint(SnapshotPoint point, ITextEdit edit, bool failOnNonWhitespaceCharacter,
            bool useBufferPrimitives = false, int? columnsToRemove = null)
        {
            if (columnsToRemove.HasValue && columnsToRemove.Value == 0)
                return true;
            var snapshot = edit.Snapshot;
            var indentSize = Options.GetIndentSize();
            var textPoint = !useBufferPrimitives ? _editorPrimitives.View.GetTextPoint(point) : _editorPrimitives.Buffer.GetTextPoint(point);
            var column = textPoint.Column;
            var startOfLine = textPoint.StartOfLine;
            if (textPoint.CurrentPosition == startOfLine)
                return true;
            var num1 = !columnsToRemove.HasValue ? (column - 1) / indentSize * indentSize : Math.Max(0, column - columnsToRemove.Value);
            var index = point.Position - 1;
            var num2 = 0;
            var breakLoop = false;
            for (; index >= startOfLine; --index)
            {
                if (breakLoop)
                {
                    ++index;
                    break;
                }
                switch (snapshot[index])
                {
                    case '\t':
                    case ' ':
                        num2 = !useBufferPrimitives ? _editorPrimitives.View.GetTextPoint(index).Column : _editorPrimitives.Buffer.GetTextPoint(index).Column;
                        if (num2 > num1)
                            continue;
                        breakLoop = true;
                        break;
                    default:
                        ++index;
                        breakLoop = true;
                        break;
                }
            }
            if (index >= point)
                return true;
            if (failOnNonWhitespaceCharacter && num2 > num1)
                return false;
            var replaceWith = string.Empty;
            if (num2 < num1)
                replaceWith = GetWhiteSpaceForPositionAndVirtualSpace(new SnapshotPoint(snapshot, index), num1 - num2, useBufferPrimitives);
            return edit.Replace(Span.FromBounds(index, point.Position), replaceWith);
        }

        private int DetermineMaxBoxUnindent()
        {
            Options.GetTabSize();
            var indentSize = Options.GetIndentSize();
            foreach (var virtualSnapshotPoint in TextView.Selection.VirtualSelectedSpans.Select(s => s.Start))
            {
                var num1 = indentSize - virtualSnapshotPoint.VirtualSpaces;
                if (num1 > 0)
                {
                    TextPoint textPoint = _editorPrimitives.View.GetTextPoint(virtualSnapshotPoint.Position);
                    var num2 = textPoint.Column - num1;
                    var startOfLine = textPoint.StartOfLine;
                    for (var position = virtualSnapshotPoint.Position.Position - 1; position >= startOfLine && textPoint.Column >= num2; --position)
                    {
                        textPoint.MoveTo(position);
                        var nextCharacter = textPoint.GetNextCharacter();
                        if (string.Equals(nextCharacter, " ", StringComparison.Ordinal) || string.Equals(nextCharacter, "\t", StringComparison.Ordinal))
                        {
                            var column = textPoint.Column;
                            num1 = Math.Max(0, column - num2);
                            if (column <= num2)
                                break;
                        }
                        else
                            break;
                    }
                    indentSize -= num1;
                }
            }
            return indentSize;
        }

        private VirtualSnapshotPoint? CalculateBoxUnindentForSelectionPoint(VirtualSnapshotPoint point, int unindentAmount)
        {
            if (!point.IsInVirtualSpace)
                return new VirtualSnapshotPoint?();
            var selectionOnTextViewLine = TextView.Selection.GetSelectionOnTextViewLine(TextView.GetTextViewLineContainingBufferPosition(point.Position));
            if (!selectionOnTextViewLine.HasValue || !selectionOnTextViewLine.Value.Start.IsInVirtualSpace)
                return new VirtualSnapshotPoint?();
            var virtualSpaces = Math.Max(0, point.VirtualSpaces - unindentAmount);
            return new VirtualSnapshotPoint(point.Position, virtualSpaces);
        }

        private bool WillDeleteCreateEdit()
        {
            var allSelections = _multiSelectionBroker.AllSelections;
            if (_multiSelectionBroker.IsBoxSelection)
            {
                foreach (var selection in allSelections)
                {
                    var start = selection.Start;
                    var position = start.Position;
                    start = selection.Start;
                    var end = start.Position.GetContainingLine().End;
                    if (position < end)
                        return true;
                }
            }
            else
            {
                foreach (var t in allSelections)
                {
                    var selection = t;
                    if (selection.Extent.SnapshotSpan.IsEmpty)
                    {
                        selection = t;
                        if (selection.IsEmpty)
                        {
                            selection = t;
                            if (selection.InsertionPoint.Position.Position == _multiSelectionBroker.CurrentSnapshot.Length)
                                continue;
                        }
                        else
                            continue;
                    }
                    return true;
                }
            }
            return false;
        }

        private bool TryDeleteSelections()
        {
            if (_multiSelectionBroker.IsBoxSelection && _multiSelectionBroker.PrimarySelection.InsertionPoint.IsInVirtualSpace)
                _multiSelectionBroker.SetSelection(new Selection(_multiSelectionBroker.PrimarySelection.Start));
            else if (!_multiSelectionBroker.IsBoxSelection)
                _multiSelectionBroker.PerformActionOnAllSelections(transformer =>
                {
                    if (transformer.Selection.IsEmpty)
                        return;
                    transformer.MoveTo(transformer.Selection.Start, false, PositionAffinity.Successor);
                });
            return true;
        }

        private bool TryPostDeleteSelectionUpdate(IReadOnlyList<Selection> selections, Selection primarySelection, Selection boxSelection)
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
                        var virtualSpaces = virtualSnapshotPoint3.VirtualSpaces;
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
                        var virtualSpaces = virtualSnapshotPoint3.VirtualSpaces;
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
                                selectionTransformer.MoveTo(point, false, (PositionAffinity)1);
                                return;
                            }
                        }
                        var selectionTransformer1 = transformer;
                        virtualSnapshotPoint = transformer.Selection.InsertionPoint;
                        var position1 = virtualSnapshotPoint.Position;
                        virtualSnapshotPoint = selection.Start;
                        var virtualSpaces1 = virtualSnapshotPoint.VirtualSpaces;
                        var point1 = new VirtualSnapshotPoint(position1, virtualSpaces1);
                        selectionTransformer1.MoveTo(point1, false, (PositionAffinity)1);
                    }, out _);
                }
            }
            return true;
        }

        private bool TryDeleteEdit(IReadOnlyList<Selection> selections)
        {
            using (var edit = TextView.TextBuffer.CreateEdit())
            {
                for (var index = selections.Count - 1; index >= 0; --index)
                {
                    var selection = selections[index];
                    if (selection.IsEmpty)
                    {
                        VirtualSnapshotPoint insertionPoint;
                        if (_multiSelectionBroker.IsBoxSelection)
                        {
                            insertionPoint = selection.InsertionPoint;
                            var end = insertionPoint.Position.GetContainingLine().End;
                            insertionPoint = selection.InsertionPoint;
                            if (insertionPoint.Position == end)
                                continue;
                        }
                        insertionPoint = selection.InsertionPoint;
                        if (insertionPoint.IsInVirtualSpace)
                        {
                            var whitespaceForVirtualSpace = GetWhitespaceForVirtualSpace(selection.InsertionPoint);
                            var textView = TextView;
                            insertionPoint = selection.InsertionPoint;
                            var position = insertionPoint.Position;
                            var textElementSpan = textView.GetTextElementSpan(position);
                            if (!edit.Replace(textElementSpan, whitespaceForVirtualSpace))
                                return false;
                        }
                        else
                        {
                            var textEdit = edit;
                            var textView = TextView;
                            insertionPoint = selection.InsertionPoint;
                            var position = insertionPoint.Position;
                            var textElementSpan = textView.GetTextElementSpan(position);
                            if (!textEdit.Delete(textElementSpan))
                                return false;
                        }
                    }
                    else if (!edit.Delete(selection.Extent.SnapshotSpan))
                        return false;
                }
                edit.Apply();
                return !edit.Canceled;
            }
        }

        private void MoveCaretToTextLine(ITextViewLine textLine, bool select)
        {
            var anchorPoint = TextView.Selection.AnchorPoint;
            TextView.Caret.MoveTo(textLine);
            if (select)
                TextView.Selection.Select(anchorPoint.TranslateTo(TextView.TextSnapshot), TextView.Caret.Position.VirtualBufferPosition);
            else
                TextView.Selection.Clear();
        }

        private ITextViewLine FindFullyVisibleLine(ITextViewLine textLine, int indexOfNextLine)
        {
            if (textLine.VisibilityState != VisibilityState.FullyVisible && indexOfNextLine >= 0 && (_editorPrimitives.View.AdvancedTextView.TextViewLines.Count > indexOfNextLine && _editorPrimitives.View.AdvancedTextView.TextViewLines[indexOfNextLine].VisibilityState == VisibilityState.FullyVisible))
                return _editorPrimitives.View.AdvancedTextView.TextViewLines[indexOfNextLine];
            return textLine;
        }

        private void ScrollByLineAndMoveCaretIfNecessary(ScrollDirection direction)
        {
            TextView.ViewScroller.ScrollViewportVerticallyByPixels(direction == ScrollDirection.Up ? TextView.LineHeight : -TextView.LineHeight);
            if (TextView.Caret.ContainingTextViewLine.VisibilityState == VisibilityState.FullyVisible)
                return;
            var textViewLines = TextView.TextViewLines;
            var firstVisibleLine = textViewLines.FirstVisibleLine;
            ITextViewLine textLine;
            if (TextView.Caret.Position.BufferPosition < firstVisibleLine.EndIncludingLineBreak)
            {
                textLine = firstVisibleLine;
                if (firstVisibleLine.VisibilityState != VisibilityState.FullyVisible)
                {
                    var containingBufferPosition = textViewLines.GetTextViewLineContainingBufferPosition(firstVisibleLine.EndIncludingLineBreak);
                    if (containingBufferPosition != null && containingBufferPosition.VisibilityState == VisibilityState.FullyVisible)
                        textLine = containingBufferPosition;
                }
            }
            else
            {
                var lastVisibleLine = textViewLines.LastVisibleLine;
                textLine = lastVisibleLine;
                if (lastVisibleLine.VisibilityState != VisibilityState.FullyVisible && lastVisibleLine.Start > 0)
                {
                    var containingBufferPosition = textViewLines.GetTextViewLineContainingBufferPosition(lastVisibleLine.Start - 1);
                    if (containingBufferPosition != null && containingBufferPosition.VisibilityState == VisibilityState.FullyVisible)
                        textLine = containingBufferPosition;
                }
            }
            TextView.Selection.Clear();
            TextView.Caret.MoveTo(textLine);
            TextView.Caret.EnsureVisible();
        }

        private bool ChangeCase(LetterCase letterCase)
        {
            SelectionUpdate preserveCaretAndSelection;
            NormalizedSnapshotSpanCollection spans;
            if (TextView.Selection.IsEmpty)
            {
                if (TextView.Caret.Position.BufferPosition == TextView.TextSnapshot.Length)
                    return true;

                var virtualBufferPosition = TextView.Caret.Position.VirtualBufferPosition;
                var lineFromPosition = TextView.TextSnapshot.GetLineFromPosition(virtualBufferPosition.Position);
                if (lineFromPosition.End == virtualBufferPosition.Position)
                {
                    TextView.Caret.MoveTo(lineFromPosition.EndIncludingLineBreak);
                    return true;
                }
                spans = new NormalizedSnapshotSpanCollection(TextView.GetTextElementSpan(virtualBufferPosition.Position));
                preserveCaretAndSelection = SelectionUpdate.Ignore;
            }
            else
            {
                spans = TextView.Selection.SelectedSpans;
                preserveCaretAndSelection = SelectionUpdate.Preserve;
            }

            bool Func() => EditHelper(edit =>
            {
                foreach (var snapshotSpan in spans)
                {
                    var text = snapshotSpan.GetText();
                    var replaceWith = letterCase == LetterCase.Uppercase
                        ? text.ToUpper(CultureInfo.CurrentCulture)
                        : text.ToLower(CultureInfo.CurrentCulture);
                    if (!edit.Replace(snapshotSpan, replaceWith))
                        return false;
                }
                return true;
            });

            //TODO: Localize
            return ExecuteAction(letterCase == LetterCase.Uppercase ? "Make Uppercase" : "Make Lowercase", Func, preserveCaretAndSelection, true);
        }

        private bool ConvertLeadingWhitespace(string actionName, bool convertTabsToSpaces)
        {
            bool Action()
            {
                using (var edit = _editorPrimitives.Buffer.AdvancedTextBuffer.CreateEdit())
                {
                    var snapshot = edit.Snapshot;
                    for (var lineNumber = _editorPrimitives.Selection.GetStartPoint().LineNumber; lineNumber <= _editorPrimitives.Selection.GetEndPoint().LineNumber; ++lineNumber)
                    {
                        var lineFromLineNumber = snapshot.GetLineFromLineNumber(lineNumber);
                        var spaceCharacterOnLine = _editorPrimitives.Buffer.GetTextPoint(lineFromLineNumber.Start.Position).GetFirstNonWhiteSpaceCharacterOnLine();
                        var column = spaceCharacterOnLine.Column;
                        var positionAndVirtualSpace = GetWhiteSpaceForPositionAndVirtualSpace(lineFromLineNumber.Start, column, true, convertTabsToSpaces);
                        var snapshotSpan = new SnapshotSpan(lineFromLineNumber.Start, spaceCharacterOnLine.AdvancedTextPoint);
                        if (!string.Equals(positionAndVirtualSpace, snapshotSpan.GetText(), StringComparison.Ordinal) && !edit.Replace(snapshotSpan, positionAndVirtualSpace))
                            return false;
                    }

                    edit.Apply();
                    if (edit.Canceled) return false;
                }

                if (_editorPrimitives.Selection.IsEmpty) _editorPrimitives.Caret.MoveTo(_editorPrimitives.Caret.StartOfLine);
                return true;
            }

            return ExecuteAction(actionName, Action);
        }

        private bool DeleteHorizontalWhitespace()
        {
            var spanList1 = new List<Span>();
            var spanList2 = new List<Span>();
            var spanList3 = new List<Span>();
            var textSnapshot = _editorPrimitives.View.AdvancedTextView.TextSnapshot;
            var start = -1;
            var trackingPoint = textSnapshot.CreateTrackingPoint(_editorPrimitives.Caret.CurrentPosition, PointTrackingMode.Positive);
            using (var edit = textSnapshot.TextBuffer.CreateEdit())
            {
                var horizontalWhitespace1 = GetStartPointOfSpanForDeleteHorizontalWhitespace(textSnapshot);
                var horizontalWhitespace2 = GetEndPointOfSpanForDeleteHorizontalWhitespace(textSnapshot);
                for (var index = horizontalWhitespace1; index <= horizontalWhitespace2 && index < textSnapshot.Length; ++index)
                {
                    if (IsSpaceCharacter(textSnapshot[index]))
                    {
                        if (start == -1)
                            start = index;
                    }
                    else
                    {
                        if (start != -1)
                        {
                            if (index - start == 1 && !_editorPrimitives.Selection.IsEmpty)
                            {
                                spanList1.Add(Span.FromBounds(start, index));
                                if (textSnapshot[index - 1] == '\t')
                                {
                                    trackingPoint = textSnapshot.CreateTrackingPoint(index, PointTrackingMode.Positive);
                                    spanList3.Add(Span.FromBounds(start, index));
                                }
                            }
                            else
                            {
                                trackingPoint = textSnapshot.CreateTrackingPoint(index, PointTrackingMode.Positive);
                                spanList2.Add(Span.FromBounds(start, index));
                            }
                        }
                        start = -1;
                    }
                }
                if (start != -1)
                {
                    trackingPoint = textSnapshot.CreateTrackingPoint(horizontalWhitespace2, PointTrackingMode.Positive);
                    if (horizontalWhitespace2 - start == 1 && !_editorPrimitives.Selection.IsEmpty)
                        spanList1.Add(Span.FromBounds(start, horizontalWhitespace2));
                    else
                        spanList2.Add(Span.FromBounds(start, horizontalWhitespace2));
                }
                if (!DeleteHorizontalWhitespace(edit, spanList2, spanList1, spanList3))
                    return false;
                edit.Apply();
                if (edit.Canceled)
                    return false;
                if (_editorPrimitives.Selection.IsEmpty)
                    _editorPrimitives.Caret.MoveTo(trackingPoint.GetPosition(_editorPrimitives.View.AdvancedTextView.TextSnapshot));
            }
            return true;
        }

        private int GetStartPointOfSpanForDeleteHorizontalWhitespace(ITextSnapshot snapshot)
        {
            int currentPosition = _editorPrimitives.Selection.GetStartPoint().CurrentPosition;
            if (_editorPrimitives.Selection.IsEmpty)
            {
                int startOfViewLine = _editorPrimitives.Caret.StartOfViewLine;
                while (currentPosition > startOfViewLine && IsSpaceCharacter(snapshot[currentPosition - 1]))
                    --currentPosition;
            }
            return currentPosition;
        }

        private int GetEndPointOfSpanForDeleteHorizontalWhitespace(ITextSnapshot snapshot)
        {
            int currentPosition = _editorPrimitives.Selection.GetEndPoint().CurrentPosition;
            if (_editorPrimitives.Selection.IsEmpty)
            {
                int endOfViewLine = _editorPrimitives.Caret.EndOfViewLine;
                while (currentPosition < endOfViewLine && IsSpaceCharacter(snapshot[currentPosition]))
                    ++currentPosition;
            }
            return currentPosition;
        }

        private static bool DeleteHorizontalWhitespace(ITextEdit textEdit, ICollection<Span> largeSpansToDelete, IEnumerable<Span> singleSpansToDelete, ICollection<Span> singleTabsToReplace)
        {
            var snapshot = textEdit.Snapshot;
            bool Func1(int p) => snapshot.GetLineFromPosition(p).Start == p;
            bool Func2(int p) => snapshot.GetLineFromPosition(p).End == p;
            if (largeSpansToDelete.Count == 0)
            {
                foreach (var deleteSpan in singleSpansToDelete)
                {
                    if (!textEdit.Delete(deleteSpan))
                        return false;
                }
            }
            else
            {
                foreach (var span in largeSpansToDelete)
                {
                    if (Func1(span.Start) || Func2(span.End))
                    {
                        if (!textEdit.Delete(span))
                            return false;
                    }
                    else if (IsSpaceCharacter(snapshot[span.Start - 1]) || IsSpaceCharacter(snapshot[span.End]))
                    {
                        if (!textEdit.Delete(span))
                            return false;
                    }
                    else if (!textEdit.Replace(span, " "))
                        return false;
                }
                if (singleTabsToReplace.Any(replaceSpan => !Func1(replaceSpan.Start) && !Func2(replaceSpan.End) && !textEdit.Replace(replaceSpan, " ")))
                    return false;
                return singleSpansToDelete.All(deleteSpan => !Func1(deleteSpan.Start) && !Func2(deleteSpan.End) || textEdit.Delete(deleteSpan));
            }
            return true;
        }

        private static bool IsPointOnBlankLine(TextPoint textPoint)
        {
            return textPoint.GetFirstNonWhiteSpaceCharacterOnLine().CurrentPosition == textPoint.EndOfLine;
        }


        internal string GetWhitespaceForDisplayColumn(int caretColumn)
        {
            string str;
            if (!TextView.Options.IsConvertTabsToSpacesEnabled())
            {
                int tabSize = TextView.Options.GetTabSize();
                int count1 = caretColumn % tabSize;
                str = new string(' ', count1);
                int count2 = (caretColumn - count1 + tabSize - 1) / tabSize;
                if (count2 > 0)
                    str = new string('\t', count2) + str;
            }
            else
                str = new string(' ', caretColumn);
            return str;
        }

        private enum LetterCase
        {
            Uppercase,
            Lowercase,
        }
    }
}