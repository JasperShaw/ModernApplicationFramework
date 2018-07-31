namespace ModernApplicationFramework.TextEditor
{
    internal class EditorOperations : IEditorOperations
    {
        public EditorOperations(ITextView textView, EditorOperationsFactoryService factory)
        {
            
        }

        public void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint)
        {
            throw new System.NotImplementedException();
        }

        public void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint,
            TextSelectionMode selectionMode)
        {
            throw new System.NotImplementedException();
        }

        public void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint,
            TextSelectionMode selectionMode, EnsureSpanVisibleOptions? scrollOptions)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToNextCharacter(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToPreviousCharacter(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToNextWord(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToPreviousWord(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveLineUp(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveLineDown(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void PageUp(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void PageDown(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToEndOfLine(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToStartOfLine(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToHome(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void GotoLine(int lineNumber)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToStartOfDocument(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToEndOfDocument(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCurrentLineToTop()
        {
            throw new System.NotImplementedException();
        }

        public void MoveCurrentLineToBottom()
        {
            throw new System.NotImplementedException();
        }

        public void MoveToStartOfLineAfterWhiteSpace(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToStartOfNextLineAfterWhiteSpace(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToStartOfPreviousLineAfterWhiteSpace(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToLastNonWhiteSpaceCharacter(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToTopOfView(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void MoveToBottomOfView(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void SwapCaretAndAnchor()
        {
            throw new System.NotImplementedException();
        }

        public bool Backspace()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteWordToRight()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteWordToLeft()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteToEndOfLine()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteToBeginningOfLine()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteBlankLines()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteHorizontalWhiteSpace()
        {
            throw new System.NotImplementedException();
        }

        public bool InsertNewLine()
        {
            throw new System.NotImplementedException();
        }

        public bool OpenLineAbove()
        {
            throw new System.NotImplementedException();
        }

        public bool OpenLineBelow()
        {
            throw new System.NotImplementedException();
        }

        public bool Indent()
        {
            throw new System.NotImplementedException();
        }

        public bool Unindent()
        {
            throw new System.NotImplementedException();
        }

        public bool IncreaseLineIndent()
        {
            throw new System.NotImplementedException();
        }

        public bool DecreaseLineIndent()
        {
            throw new System.NotImplementedException();
        }

        public bool InsertText(string text)
        {
            throw new System.NotImplementedException();
        }

        public bool InsertTextAsBox(string text, out VirtualSnapshotPoint boxStart, out VirtualSnapshotPoint boxEnd)
        {
            throw new System.NotImplementedException();
        }

        public bool InsertProvisionalText(string text)
        {
            throw new System.NotImplementedException();
        }

        public bool Delete()
        {
            throw new System.NotImplementedException();
        }

        public bool DeleteFullLine()
        {
            throw new System.NotImplementedException();
        }

        public bool ReplaceSelection(string text)
        {
            throw new System.NotImplementedException();
        }

        public bool TransposeCharacter()
        {
            throw new System.NotImplementedException();
        }

        public bool TransposeLine()
        {
            throw new System.NotImplementedException();
        }

        public bool TransposeWord()
        {
            throw new System.NotImplementedException();
        }

        public bool MakeLowercase()
        {
            throw new System.NotImplementedException();
        }

        public bool MakeUppercase()
        {
            throw new System.NotImplementedException();
        }

        public bool ToggleCase()
        {
            throw new System.NotImplementedException();
        }

        public bool Capitalize()
        {
            throw new System.NotImplementedException();
        }

        public bool ReplaceText(Span replaceSpan, string text)
        {
            throw new System.NotImplementedException();
        }

        public int ReplaceAllMatches(string searchText, string replaceText, bool matchCase, bool matchWholeWord,
            bool useRegularExpressions)
        {
            throw new System.NotImplementedException();
        }

        public bool InsertFile(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public bool Tabify()
        {
            throw new System.NotImplementedException();
        }

        public bool Untabify()
        {
            throw new System.NotImplementedException();
        }

        public bool ConvertSpacesToTabs()
        {
            throw new System.NotImplementedException();
        }

        public bool ConvertTabsToSpaces()
        {
            throw new System.NotImplementedException();
        }

        public bool NormalizeLineEndings(string replacement)
        {
            throw new System.NotImplementedException();
        }

        public void SelectCurrentWord()
        {
            throw new System.NotImplementedException();
        }

        public void SelectEnclosing()
        {
            throw new System.NotImplementedException();
        }

        public void SelectFirstChild()
        {
            throw new System.NotImplementedException();
        }

        public void SelectNextSibling(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void SelectPreviousSibling(bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void SelectLine(ITextViewLine viewLine, bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void SelectAll()
        {
            throw new System.NotImplementedException();
        }

        public void ExtendSelection(int newEnd)
        {
            throw new System.NotImplementedException();
        }

        public void MoveCaret(ITextViewLine textLine, double horizontalOffset, bool extendSelection)
        {
            throw new System.NotImplementedException();
        }

        public void ResetSelection()
        {
            throw new System.NotImplementedException();
        }

        public bool CopySelection()
        {
            throw new System.NotImplementedException();
        }

        public bool CutSelection()
        {
            throw new System.NotImplementedException();
        }

        public bool Paste()
        {
            throw new System.NotImplementedException();
        }

        public bool CutFullLine()
        {
            throw new System.NotImplementedException();
        }

        public bool CanPaste { get; }
        public bool CanDelete { get; }
        public bool CanCut { get; }
        public void ScrollUpAndMoveCaretIfNecessary()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollDownAndMoveCaretIfNecessary()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollPageUp()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollPageDown()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollColumnLeft()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollColumnRight()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollLineBottom()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollLineTop()
        {
            throw new System.NotImplementedException();
        }

        public void ScrollLineCenter()
        {
            throw new System.NotImplementedException();
        }

        public void AddBeforeTextBufferChangePrimitive()
        {
            throw new System.NotImplementedException();
        }

        public void AddAfterTextBufferChangePrimitive()
        {
            throw new System.NotImplementedException();
        }

        public void ZoomIn()
        {
            throw new System.NotImplementedException();
        }

        public void ZoomOut()
        {
            throw new System.NotImplementedException();
        }

        public void ZoomTo(double zoomLevel)
        {
            throw new System.NotImplementedException();
        }

        public string GetWhitespaceForVirtualSpace(VirtualSnapshotPoint point)
        {
            throw new System.NotImplementedException();
        }

        public ITextView TextView { get; }
        public IEditorOptions Options { get; }
        public ITrackingSpan ProvisionalCompositionSpan { get; }
        public string SelectedText { get; }
    }
}