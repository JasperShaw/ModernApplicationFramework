using System.Collections.Generic;
using System.IO;

namespace ModernApplicationFramework.TextEditor.Text
{
    internal abstract class BaseSnapshot : ITextSnapshot
    {
        internal readonly StringRebuilder Content;

        public ITextBuffer TextBuffer { get; }
        public IContentType ContentType { get; }
        public ITextVersion Version { get; }
        public int Length { get; }
        public int LineCount { get; }
        public string GetText(Span span)
        {
            throw new System.NotImplementedException();
        }

        public string GetText(int startIndex, int length)
        {
            throw new System.NotImplementedException();
        }

        public string GetText()
        {
            throw new System.NotImplementedException();
        }

        public char[] ToCharArray(int startIndex, int length)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            throw new System.NotImplementedException();
        }

        public char this[int position] => throw new System.NotImplementedException();

        public ITextSnapshotLine GetLineFromLineNumber(int lineNumber)
        {
            throw new System.NotImplementedException();
        }

        public ITextSnapshotLine GetLineFromPosition(int position)
        {
            throw new System.NotImplementedException();
        }

        public int GetLineNumberFromPosition(int position)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ITextSnapshotLine> Lines { get; }
        public void Write(TextWriter writer, Span span)
        {
            throw new System.NotImplementedException();
        }

        public void Write(TextWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}