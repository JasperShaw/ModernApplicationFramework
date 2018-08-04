namespace ModernApplicationFramework.TextEditor
{
    public interface IEditorOperations
    {
        void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint);

        void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint, TextSelectionMode selectionMode);

        void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint, TextSelectionMode selectionMode, EnsureSpanVisibleOptions? scrollOptions);

        void MoveToNextCharacter(bool extendSelection);

        void MoveToPreviousCharacter(bool extendSelection);

        void MoveToNextWord(bool extendSelection);

        void MoveToPreviousWord(bool extendSelection);

        void MoveLineUp(bool extendSelection);

        void MoveLineDown(bool extendSelection);

        void PageUp(bool extendSelection);

        void PageDown(bool extendSelection);

        void MoveToEndOfLine(bool extendSelection);

        void MoveToStartOfLine(bool extendSelection);

        void MoveToHome(bool extendSelection);

        void GotoLine(int lineNumber);

        void MoveToStartOfDocument(bool extendSelection);

        void MoveToEndOfDocument(bool extendSelection);

        void MoveCurrentLineToTop();

        void MoveCurrentLineToBottom();

        void MoveToStartOfLineAfterWhiteSpace(bool extendSelection);

        void MoveToStartOfNextLineAfterWhiteSpace(bool extendSelection);

        void MoveToStartOfPreviousLineAfterWhiteSpace(bool extendSelection);

        void MoveToLastNonWhiteSpaceCharacter(bool extendSelection);

        void MoveToTopOfView(bool extendSelection);

        void MoveToBottomOfView(bool extendSelection);

        void SwapCaretAndAnchor();

        bool Backspace();

        bool DeleteWordToRight();

        bool DeleteWordToLeft();

        bool DeleteToEndOfLine();

        bool DeleteToBeginningOfLine();

        bool DeleteBlankLines();

        bool DeleteHorizontalWhiteSpace();

        bool InsertNewLine();

        bool OpenLineAbove();

        bool OpenLineBelow();

        bool Indent();

        bool Unindent();

        bool IncreaseLineIndent();

        bool DecreaseLineIndent();

        bool InsertText(string text);

        bool InsertTextAsBox(string text, out VirtualSnapshotPoint boxStart, out VirtualSnapshotPoint boxEnd);

        bool InsertProvisionalText(string text);

        bool Delete();

        bool DeleteFullLine();

        bool ReplaceSelection(string text);

        bool TransposeCharacter();

        bool TransposeLine();

        bool TransposeWord();

        bool MakeLowercase();

        bool MakeUppercase();

        bool ToggleCase();

        bool Capitalize();

        bool ReplaceText(Span replaceSpan, string text);

        int ReplaceAllMatches(string searchText, string replaceText, bool matchCase, bool matchWholeWord, bool useRegularExpressions);

        bool InsertFile(string filePath);

        bool Tabify();

        bool Untabify();

        bool ConvertSpacesToTabs();

        bool ConvertTabsToSpaces();

        bool NormalizeLineEndings(string replacement);

        void SelectCurrentWord();

        void SelectEnclosing();

        void SelectFirstChild();

        void SelectNextSibling(bool extendSelection);

        void SelectPreviousSibling(bool extendSelection);

        void SelectLine(ITextViewLine viewLine, bool extendSelection);

        void SelectAll();

        void ExtendSelection(int newEnd);

        void MoveCaret(ITextViewLine textLine, double horizontalOffset, bool extendSelection);

        void ResetSelection();

        bool CopySelection();

        bool CutSelection();

        bool Paste();

        bool CutFullLine();

        bool CanPaste { get; }

        bool CanDelete { get; }

        bool CanCut { get; }

        void ScrollUpAndMoveCaretIfNecessary();

        void ScrollDownAndMoveCaretIfNecessary();

        void ScrollPageUp();

        void ScrollPageDown();

        void ScrollColumnLeft();

        void ScrollColumnRight();

        void ScrollLineBottom();

        void ScrollLineTop();

        void ScrollLineCenter();

        void AddBeforeTextBufferChangePrimitive();

        void AddAfterTextBufferChangePrimitive();

        void ZoomIn();

        void ZoomOut();

        void ZoomTo(double zoomLevel);

        string GetWhitespaceForVirtualSpace(VirtualSnapshotPoint point);

        ITextView TextView { get; }

        IEditorOptions Options { get; }

        ITrackingSpan ProvisionalCompositionSpan { get; }

        string SelectedText { get; }

        bool MoveSelectedLinesUp();

        bool MoveSelectedLinesDown();
    }
}