using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Formatting;

namespace ModernApplicationFramework.Text.Ui.Operations
{
    public interface IEditorOperations
    {
        bool CanCut { get; }

        bool CanDelete { get; }

        bool CanPaste { get; }

        IEditorOptions Options { get; }

        ITrackingSpan ProvisionalCompositionSpan { get; }

        string SelectedText { get; }

        ITextView TextView { get; }

        void AddAfterTextBufferChangePrimitive();

        void AddBeforeTextBufferChangePrimitive();

        bool Backspace();

        bool Capitalize();

        bool ConvertSpacesToTabs();

        bool ConvertTabsToSpaces();

        bool CopySelection();

        bool CutFullLine();

        bool CutSelection();

        bool DecreaseLineIndent();

        bool Delete();

        bool DeleteBlankLines();

        bool DeleteFullLine();

        bool DeleteHorizontalWhiteSpace();

        bool DeleteToBeginningOfLine();

        bool DeleteToEndOfLine();

        bool DeleteWordToLeft();

        bool DeleteWordToRight();

        void ExtendSelection(int newEnd);

        string GetWhitespaceForVirtualSpace(VirtualSnapshotPoint point);

        void GotoLine(int lineNumber);

        bool IncreaseLineIndent();

        bool Indent();

        bool InsertFile(string filePath);

        bool InsertNewLine();

        bool InsertProvisionalText(string text);

        bool InsertText(string text);

        bool InsertTextAsBox(string text, out VirtualSnapshotPoint boxStart, out VirtualSnapshotPoint boxEnd);

        bool MakeLowercase();

        bool MakeUppercase();

        void MoveCaret(ITextViewLine textLine, double horizontalOffset, bool extendSelection);

        void MoveCurrentLineToBottom();

        void MoveCurrentLineToTop();

        void MoveLineDown(bool extendSelection);

        void MoveLineUp(bool extendSelection);

        bool MoveSelectedLinesDown();

        bool MoveSelectedLinesUp();

        void MoveToBottomOfView(bool extendSelection);

        void MoveToEndOfDocument(bool extendSelection);

        void MoveToEndOfLine(bool extendSelection);

        void MoveToHome(bool extendSelection);

        void MoveToLastNonWhiteSpaceCharacter(bool extendSelection);

        void MoveToNextCharacter(bool extendSelection);

        void MoveToNextWord(bool extendSelection);

        void MoveToPreviousCharacter(bool extendSelection);

        void MoveToPreviousWord(bool extendSelection);

        void MoveToStartOfDocument(bool extendSelection);

        void MoveToStartOfLine(bool extendSelection);

        void MoveToStartOfLineAfterWhiteSpace(bool extendSelection);

        void MoveToStartOfNextLineAfterWhiteSpace(bool extendSelection);

        void MoveToStartOfPreviousLineAfterWhiteSpace(bool extendSelection);

        void MoveToTopOfView(bool extendSelection);

        bool NormalizeLineEndings(string replacement);

        bool OpenLineAbove();

        bool OpenLineBelow();

        void PageDown(bool extendSelection);

        void PageUp(bool extendSelection);

        bool Paste();

        int ReplaceAllMatches(string searchText, string replaceText, bool matchCase, bool matchWholeWord,
            bool useRegularExpressions);

        bool ReplaceSelection(string text);

        bool ReplaceText(Span replaceSpan, string text);

        void ResetSelection();

        void ScrollColumnLeft();

        void ScrollColumnRight();

        void ScrollDownAndMoveCaretIfNecessary();

        void ScrollLineBottom();

        void ScrollLineCenter();

        void ScrollLineTop();

        void ScrollPageDown();

        void ScrollPageUp();

        void ScrollUpAndMoveCaretIfNecessary();

        void SelectAll();
        void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint);

        void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint,
            TextSelectionMode selectionMode);

        void SelectAndMoveCaret(VirtualSnapshotPoint anchorPoint, VirtualSnapshotPoint activePoint,
            TextSelectionMode selectionMode, EnsureSpanVisibleOptions? scrollOptions);

        void SelectCurrentWord();

        void SelectEnclosing();

        void SelectFirstChild();

        void SelectLine(ITextViewLine viewLine, bool extendSelection);

        void SelectNextSibling(bool extendSelection);

        void SelectPreviousSibling(bool extendSelection);

        void SwapCaretAndAnchor();

        bool Tabify();

        bool ToggleCase();

        bool TransposeCharacter();

        bool TransposeLine();

        bool TransposeWord();

        bool Unindent();

        bool Untabify();

        void ZoomIn();

        void ZoomOut();

        void ZoomTo(double zoomLevel);
    }
}