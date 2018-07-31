using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class PrimitiveTextBuffer
    {
        public abstract TextPoint GetTextPoint(int position);

        public abstract TextPoint GetTextPoint(int line, int column);

        public abstract TextRange GetLine(int line);

        public abstract TextRange GetTextRange(TextPoint startPoint, TextPoint endPoint);

        public abstract TextRange GetTextRange(int startPosition, int endPosition);

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextBuffer AdvancedTextBuffer { get; }

        public abstract TextPoint GetStartPoint();

        public abstract TextPoint GetEndPoint();

        public abstract IEnumerable<TextRange> Lines { get; }
    }
}