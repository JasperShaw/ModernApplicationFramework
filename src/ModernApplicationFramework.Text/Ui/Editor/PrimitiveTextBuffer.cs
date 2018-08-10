using System.Collections.Generic;
using System.ComponentModel;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public abstract class PrimitiveTextBuffer
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public abstract ITextBuffer AdvancedTextBuffer { get; }

        public abstract IEnumerable<TextRange> Lines { get; }

        public abstract TextPoint GetEndPoint();

        public abstract TextRange GetLine(int line);

        public abstract TextPoint GetStartPoint();
        public abstract TextPoint GetTextPoint(int position);

        public abstract TextPoint GetTextPoint(int line, int column);

        public abstract TextRange GetTextRange(TextPoint startPoint, TextPoint endPoint);

        public abstract TextRange GetTextRange(int startPosition, int endPosition);
    }
}