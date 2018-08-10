using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Logic.Editor;
using ModernApplicationFramework.Text.Logic.Operations;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.TextEditor.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class DefaultTextRangePrimitive : TextRange
    {
        private TextPoint _startPoint;
        private TextPoint _endPoint;
        private readonly IEditorOptions _editorOptions;
        private readonly IEditorOptionsFactoryService _editorOptionsProvider;

        internal DefaultTextRangePrimitive(TextPoint startPoint, TextPoint endPoint, IEditorOptionsFactoryService editorOptionsProvider)
        {
            if (startPoint.CurrentPosition < endPoint.CurrentPosition)
            {
                _startPoint = startPoint.Clone();
                _endPoint = endPoint.Clone();
            }
            else
            {
                _endPoint = startPoint.Clone();
                _startPoint = endPoint.Clone();
            }
            _editorOptionsProvider = editorOptionsProvider;
            _editorOptions = _editorOptionsProvider.GetOptions(_startPoint.TextBuffer.AdvancedTextBuffer);
        }

        public override TextPoint GetStartPoint()
        {
            return _startPoint.Clone();
        }

        public override TextPoint GetEndPoint()
        {
            return _endPoint.Clone();
        }

        public override PrimitiveTextBuffer TextBuffer => _startPoint.TextBuffer;

        public override SnapshotSpan AdvancedTextRange => new SnapshotSpan(TextBuffer.AdvancedTextBuffer.CurrentSnapshot, Span.FromBounds(_startPoint.CurrentPosition, _endPoint.CurrentPosition));

        public override bool IsEmpty => _startPoint.CurrentPosition == _endPoint.CurrentPosition;

        public override bool MakeUppercase()
        {
            return ReplaceText(GetText().ToUpper(CultureInfo.CurrentCulture));
        }

        public override bool MakeLowercase()
        {
            return ReplaceText(GetText().ToLower(CultureInfo.CurrentCulture));
        }

        public override bool Capitalize()
        {
            int currentPosition1 = _startPoint.CurrentPosition;
            if (IsEmpty)
            {
                int currentPosition2 = _endPoint.CurrentPosition;
                TextRange currentWord = _startPoint.GetCurrentWord();
                string nextCharacter = _startPoint.GetNextCharacter();
                string replacement = _startPoint.CurrentPosition != currentWord.GetStartPoint().CurrentPosition ? nextCharacter.ToLower(CultureInfo.CurrentCulture) : nextCharacter.ToUpper(CultureInfo.CurrentCulture);
                if (!PrimitivesUtilities.Replace(TextBuffer.AdvancedTextBuffer, new Span(_startPoint.CurrentPosition, replacement.Length), replacement))
                    return false;
                _endPoint.MoveTo(currentPosition2);
            }
            else
            {
                using (ITextEdit edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
                {
                    TextRange textRange = _startPoint.GetCurrentWord();
                    if (textRange.GetStartPoint().CurrentPosition < _startPoint.CurrentPosition)
                        textRange = textRange.GetEndPoint().GetNextWord();
                    for (; textRange.GetStartPoint().CurrentPosition < _endPoint.CurrentPosition; textRange = textRange.GetEndPoint().GetNextWord())
                    {
                        string text = textRange.GetText();
                        string nextTextElement = StringInfo.GetNextTextElement(text);
                        string replaceWith = nextTextElement.ToUpper(CultureInfo.CurrentCulture) + text.Substring(nextTextElement.Length).ToLower(CultureInfo.CurrentCulture);
                        if (!edit.Replace(textRange.AdvancedTextRange.Span, replaceWith))
                        {
                            edit.Cancel();
                            return false;
                        }
                    }
                    edit.Apply();
                    if (edit.Canceled)
                        return false;
                }
            }
            _startPoint.MoveTo(currentPosition1);
            return true;
        }

        public override bool ToggleCase()
        {
            if (IsEmpty)
            {
                TextPoint otherPoint = _startPoint.Clone();
                otherPoint.MoveToNextCharacter();
                TextRange textRange = _startPoint.GetTextRange(otherPoint);
                string text = textRange.GetText();
                return textRange.ReplaceText(!char.IsUpper(text, 0) ? text.ToUpper(CultureInfo.CurrentCulture) : text.ToLower(CultureInfo.CurrentCulture));
            }
            int currentPosition1 = _startPoint.CurrentPosition;
            using (ITextEdit edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
            {
                for (int currentPosition2 = _startPoint.CurrentPosition; currentPosition2 < _endPoint.CurrentPosition; ++currentPosition2)
                {
                    char c = edit.Snapshot[currentPosition2];
                    char ch = !char.IsUpper(c) ? char.ToUpper(c, CultureInfo.CurrentCulture) : char.ToLower(c, CultureInfo.CurrentCulture);
                    if (!edit.Replace(currentPosition2, 1, ch.ToString()))
                    {
                        edit.Cancel();
                        return false;
                    }
                }
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            _startPoint.MoveTo(currentPosition1);
            return true;
        }

        public override bool Delete()
        {
            return PrimitivesUtilities.Delete(TextBuffer.AdvancedTextBuffer, Span.FromBounds(_startPoint.CurrentPosition, _endPoint.CurrentPosition));
        }

        public override bool Indent()
        {
            string text = _editorOptions.IsConvertTabsToSpacesEnabled() ? new string(' ', _editorOptions.GetTabSize()) : "\t";
            if (_startPoint.LineNumber == _endPoint.LineNumber)
                return _startPoint.InsertIndent();
            using (ITextEdit edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
            {
                ITextSnapshot currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
                for (int lineNumber = _startPoint.LineNumber; lineNumber <= _endPoint.LineNumber; ++lineNumber)
                {
                    ITextSnapshotLine lineFromLineNumber = currentSnapshot.GetLineFromLineNumber(lineNumber);
                    if (lineFromLineNumber.Length > 0 && lineFromLineNumber.Start != _endPoint.CurrentPosition && !edit.Insert(lineFromLineNumber.Start, text))
                        return false;
                }
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            return true;
        }

        public override bool Unindent()
        {
            if (_startPoint.LineNumber == _endPoint.LineNumber)
                return _startPoint.RemovePreviousIndent();
            using (ITextEdit edit = TextBuffer.AdvancedTextBuffer.CreateEdit())
            {
                ITextSnapshot currentSnapshot = TextBuffer.AdvancedTextBuffer.CurrentSnapshot;
                for (int lineNumber = _startPoint.LineNumber; lineNumber <= _endPoint.LineNumber; ++lineNumber)
                {
                    ITextSnapshotLine lineFromLineNumber = currentSnapshot.GetLineFromLineNumber(lineNumber);
                    if (lineFromLineNumber.Length > 0 && _endPoint.CurrentPosition != lineFromLineNumber.Start)
                    {
                        if (currentSnapshot[lineFromLineNumber.Start] == '\t')
                        {
                            if (!edit.Delete(new Span(lineFromLineNumber.Start, 1)))
                                return false;
                        }
                        else
                        {
                            int length = 0;
                            while (lineFromLineNumber.Start + length < currentSnapshot.Length && length < _editorOptions.GetTabSize() && currentSnapshot[lineFromLineNumber.Start + length] == ' ')
                                ++length;
                            if (length > 0 && !edit.Delete(new Span(lineFromLineNumber.Start, length)))
                                return false;
                        }
                    }
                }
                edit.Apply();
                if (edit.Canceled)
                    return false;
            }
            return true;
        }

        public override TextRange Find(string pattern)
        {
            return Find(pattern, FindOptions.None);
        }

        public override TextRange Find(string pattern, FindOptions findOptions)
        {
            return _startPoint.Find(pattern, findOptions, _endPoint);
        }

        public override Collection<TextRange> FindAll(string pattern)
        {
            return FindAll(pattern, FindOptions.None);
        }

        public override Collection<TextRange> FindAll(string pattern, FindOptions findOptions)
        {
            return _startPoint.FindAll(pattern, findOptions, _endPoint);
        }

        public override bool ReplaceText(string newText)
        {
            if (string.IsNullOrEmpty(newText))
                throw new ArgumentNullException(nameof(newText));
            int currentPosition = _startPoint.CurrentPosition;
            if (!PrimitivesUtilities.Replace(TextBuffer.AdvancedTextBuffer, Span.FromBounds(_startPoint.CurrentPosition, _endPoint.CurrentPosition), newText))
                return false;
            _startPoint.MoveTo(currentPosition);
            return true;
        }

        public override string GetText()
        {
            return TextBuffer.AdvancedTextBuffer.CurrentSnapshot.GetText(Span.FromBounds(_startPoint.CurrentPosition, _endPoint.CurrentPosition));
        }

        protected override TextRange CloneInternal()
        {
            return new DefaultTextRangePrimitive(_startPoint, _endPoint, _editorOptionsProvider);
        }

        public override void SetStart(TextPoint startPoint)
        {
            if (startPoint.TextBuffer != TextBuffer)
                throw new ArgumentException();
            if (startPoint.CurrentPosition > _endPoint.CurrentPosition)
            {
                _startPoint = _endPoint;
                _endPoint = startPoint.Clone();
            }
            else
                _startPoint = startPoint.Clone();
        }

        public override void SetEnd(TextPoint endPoint)
        {
            if (endPoint.TextBuffer != TextBuffer)
                throw new ArgumentException("startPoint");
            if (endPoint.CurrentPosition < _startPoint.CurrentPosition)
            {
                _endPoint = _startPoint;
                _startPoint = endPoint.Clone();
            }
            else
                _endPoint = endPoint.Clone();
        }

        public override void MoveTo(TextRange newRange)
        {
            if (newRange.TextBuffer != TextBuffer)
                throw new ArgumentException();
            _startPoint = newRange.GetStartPoint();
            _endPoint = newRange.GetEndPoint();
        }

        protected override IEnumerator<TextPoint> GetEnumeratorInternal()
        {
            for (int position = _startPoint.CurrentPosition; position <= _endPoint.CurrentPosition; ++position)
            {
                TextPoint textPoint = _startPoint.Clone();
                textPoint.MoveTo(position);
                yield return textPoint;
            }
        }
    }
}