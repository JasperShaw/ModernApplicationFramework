using System;
using System.Globalization;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class TextChange : ITextChange3
    {
        private int _masterChangeOffset = -1;
        private int _oldPosition;
        private int _newPosition;
        internal StringRebuilder _oldText;
        internal StringRebuilder _newText;
        private LineBreakBoundaryConditions _lineBreakBoundaryConditions;
        private int? _lineCountDelta;

        public TextChange(int oldPosition, StringRebuilder oldText, StringRebuilder newText, LineBreakBoundaryConditions boundaryConditions)
        {
            if (oldPosition < 0)
                throw new ArgumentOutOfRangeException(nameof(oldPosition));
            _oldPosition = oldPosition;
            _newPosition = oldPosition;
            _oldText = oldText;
            _newText = newText;
            _lineBreakBoundaryConditions = boundaryConditions;
        }

        internal TextChange(int oldPosition, string oldText, string newText, LineBreakBoundaryConditions boundaryConditions)
            : this(oldPosition, StringRebuilder.Create(oldText), StringRebuilder.Create(newText), boundaryConditions)
        {
        }

        public static TextChange Create(int oldPosition, string oldText, string newText, ITextSnapshot currentSnapshot)
        {
            return new TextChange(oldPosition, StringRebuilder.Create(oldText), StringRebuilder.Create(newText), ComputeLineBreakBoundaryConditions(currentSnapshot, oldPosition, oldText.Length));
        }

        public static TextChange Create(int oldPosition, StringRebuilder oldText, string newText, ITextSnapshot currentSnapshot)
        {
            return new TextChange(oldPosition, oldText, StringRebuilder.Create(newText), ComputeLineBreakBoundaryConditions(currentSnapshot, oldPosition, oldText.Length));
        }

        public static TextChange Create(int oldPosition, string oldText, StringRebuilder newText, ITextSnapshot currentSnapshot)
        {
            return new TextChange(oldPosition, StringRebuilder.Create(oldText), newText, ComputeLineBreakBoundaryConditions(currentSnapshot, oldPosition, oldText.Length));
        }

        public static TextChange Create(int oldPosition, StringRebuilder oldText, StringRebuilder newText, ITextSnapshot currentSnapshot)
        {
            return new TextChange(oldPosition, oldText, newText, ComputeLineBreakBoundaryConditions(currentSnapshot, oldPosition, oldText.Length));
        }

        public Span OldSpan => new Span(_oldPosition, _oldText.Length);

        public Span NewSpan => new Span(_newPosition, _newText.Length);

        public int OldPosition
        {
            get => _oldPosition;
            internal set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _oldPosition = value;
            }
        }

        public int NewPosition
        {
            get => _newPosition;
            internal set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _newPosition = value;
            }
        }

        public int Delta => _newText.Length - _oldText.Length;

        public int OldEnd => _oldPosition + _oldText.Length;

        public int NewEnd => _newPosition + _newText.Length;

        public string OldText => _oldText.GetText(new Span(0, _oldText.Length));

        public string NewText => _newText.GetText(new Span(0, _newText.Length));

        public int NewLength => _newText.Length;

        public int OldLength => _oldText.Length;

        public int LineCountDelta
        {
            get
            {
                if (!_lineCountDelta.HasValue)
                    _lineCountDelta = new int?(TextModelUtilities.ComputeLineCountDelta(_lineBreakBoundaryConditions, _oldText, _newText));
                return _lineCountDelta.Value;
            }
        }

        public bool IsOpaque { get; internal set; }

        public string GetOldText(Span span)
        {
            return _oldText.GetText(span);
        }

        public string GetNewText(Span span)
        {
            return _newText.GetText(span);
        }

        public char GetOldTextAt(int position)
        {
            if (position > OldLength)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _oldText[position];
        }

        public char GetNewTextAt(int position)
        {
            if (position > NewLength)
                throw new ArgumentOutOfRangeException(nameof(position));
            return _newText[position];
        }

        internal LineBreakBoundaryConditions LineBreakBoundaryConditions
        {
            get => _lineBreakBoundaryConditions;
            set
            {
                _lineBreakBoundaryConditions = value;
                _lineCountDelta = new int?();
            }
        }

        internal void RecordMasterChangeOffset(int masterChangeOffset)
        {
            if (masterChangeOffset < 0)
                throw new ArgumentOutOfRangeException(nameof(masterChangeOffset), "MasterChangeOffset should be non-negative.");
            if (_masterChangeOffset != -1)
                throw new InvalidOperationException("MasterChangeOffset has already been set.");
            _masterChangeOffset = masterChangeOffset;
        }

        internal int MasterChangeOffset
        {
            get
            {
                if (_masterChangeOffset != -1)
                    return _masterChangeOffset;
                return 0;
            }
        }

        internal static int Compare(TextChange x, TextChange y)
        {
            int num = x.OldPosition - y.OldPosition;
            if (num != 0)
                return num;
            return x.MasterChangeOffset - y.MasterChangeOffset;
        }

        private static LineBreakBoundaryConditions ComputeLineBreakBoundaryConditions(ITextSnapshot currentSnapshot, int position, int oldLength)
        {
            LineBreakBoundaryConditions boundaryConditions = LineBreakBoundaryConditions.None;
            if (position > 0 && currentSnapshot[position - 1] == '\r')
                boundaryConditions = LineBreakBoundaryConditions.PrecedingReturn;
            int index = position + oldLength;
            if (index < currentSnapshot.Length && currentSnapshot[index] == '\n')
                boundaryConditions |= LineBreakBoundaryConditions.SucceedingNewline;
            return boundaryConditions;
        }

        public string ToString(bool brief)
        {
            if (brief)
                return string.Format(CultureInfo.InvariantCulture, "old={0} new={1}", OldSpan, NewSpan);
            return string.Format(CultureInfo.InvariantCulture, "old={0}:'{1}' new={2}:'{3}'", OldSpan, TextUtilities.Escape(OldText), NewSpan, TextUtilities.Escape(NewText, 40));
        }

        public override string ToString()
        {
            return ToString(false);
        }

        public static StringRebuilder OldStringRebuilder(ITextChange change)
        {
            TextChange textChange = change as TextChange;
            if (textChange == null)
                return StringRebuilder.Create(change.OldText);
            return textChange._oldText;
        }

        public static StringRebuilder NewStringRebuilder(ITextChange change)
        {
            var textChange = change as TextChange;
            return textChange == null ? StringRebuilder.Create(change.NewText) : textChange._newText;
        }

        public static StringRebuilder ChangeOldSubText(ITextChange change, int start, int length)
        {
            if (change is TextChange textChange)
                return textChange._oldText.GetSubText(new Span(start, length));
            if (change is ITextChange3 textChange3)
                return StringRebuilder.Create(textChange3.GetOldText(new Span(start, length)));
            return StringRebuilder.Create(change.OldText.Substring(start, length));
        }

        public static StringRebuilder ChangeNewSubText(ITextChange change, int start, int length)
        {
            if (change is TextChange textChange)
                return textChange._newText.GetSubText(new Span(start, length));
            if (change is ITextChange3 textChange3)
                return StringRebuilder.Create(textChange3.GetNewText(new Span(start, length)));
            return StringRebuilder.Create(change.NewText.Substring(start, length));
        }

        public static string ChangeOldSubstring(ITextChange change, int start, int length)
        {
            if (change is TextChange textChange)
                return textChange._oldText.GetText(new Span(start, length));
            if (change is ITextChange3 textChange3)
                return textChange3.GetOldText(new Span(start, length));
            return change.OldText.Substring(start, length);
        }

        public static string ChangeNewSubstring(ITextChange change, int start, int length)
        {
            if (change is TextChange textChange)
                return textChange._newText.GetText(new Span(start, length));
            if (change is ITextChange3 textChange3)
                return textChange3.GetNewText(new Span(start, length));
            return change.NewText.Substring(start, length);
        }
    }
}