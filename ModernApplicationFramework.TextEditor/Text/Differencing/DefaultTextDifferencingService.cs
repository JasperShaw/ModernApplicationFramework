using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.TextEditor.Text.Differencing
{
    internal sealed class DefaultTextDifferencingService : ITextDifferencingService
    {
        public IHierarchicalDifferenceCollection DiffSnapshotSpans(SnapshotSpan leftSpan, SnapshotSpan rightSpan,
            StringDifferenceOptions differenceOptions)
        {
            return DiffSnapshotSpans(leftSpan, rightSpan, differenceOptions, DefaultGetLineTextCallback);
        }

        public IHierarchicalDifferenceCollection DiffSnapshotSpans(SnapshotSpan leftSpan, SnapshotSpan rightSpan,
            StringDifferenceOptions differenceOptions, Func<ITextSnapshotLine, string> getLineTextCallback)
        {
            StringDifferenceTypes type;
            ITokenizedStringListInternal left;
            ITokenizedStringListInternal right;
            if (differenceOptions.DifferenceType.HasFlag(StringDifferenceTypes.Line))
            {
                type = StringDifferenceTypes.Line;
                left = new SnapshotLineList(leftSpan, getLineTextCallback, differenceOptions);
                right = new SnapshotLineList(rightSpan, getLineTextCallback, differenceOptions);
            }
            else if (differenceOptions.DifferenceType.HasFlag(StringDifferenceTypes.Word))
            {
                type = StringDifferenceTypes.Word;
                left = new WordDecompositionList(leftSpan, differenceOptions);
                right = new WordDecompositionList(rightSpan, differenceOptions);
            }
            else
            {
                if (!differenceOptions.DifferenceType.HasFlag(StringDifferenceTypes.Character))
                    throw new ArgumentOutOfRangeException(nameof(differenceOptions));
                type = StringDifferenceTypes.Character;
                left = new CharacterDecompositionList(leftSpan);
                right = new CharacterDecompositionList(rightSpan);
            }

            return DiffText(left, right, type, differenceOptions);
        }

        public IHierarchicalDifferenceCollection DiffStrings(string leftString, string rightString,
            StringDifferenceOptions differenceOptions)
        {
            if (leftString == null)
                throw new ArgumentNullException(nameof(leftString));
            if (rightString == null)
                throw new ArgumentNullException(nameof(rightString));
            StringDifferenceTypes type;
            ITokenizedStringListInternal left;
            ITokenizedStringListInternal right;
            if (differenceOptions.DifferenceType.HasFlag(StringDifferenceTypes.Line))
            {
                type = StringDifferenceTypes.Line;
                left = new LineDecompositionList(leftString, differenceOptions.IgnoreTrimWhiteSpace);
                right = new LineDecompositionList(rightString, differenceOptions.IgnoreTrimWhiteSpace);
            }
            else if (differenceOptions.DifferenceType.HasFlag(StringDifferenceTypes.Word))
            {
                type = StringDifferenceTypes.Word;
                left = new WordDecompositionList(leftString, differenceOptions);
                right = new WordDecompositionList(rightString, differenceOptions);
            }
            else
            {
                if (!differenceOptions.DifferenceType.HasFlag(StringDifferenceTypes.Character))
                    throw new ArgumentOutOfRangeException(nameof(differenceOptions));
                type = StringDifferenceTypes.Character;
                left = new CharacterDecompositionList(leftString);
                right = new CharacterDecompositionList(rightString);
            }

            return DiffText(left, right, type, differenceOptions);
        }

        private IHierarchicalDifferenceCollection DiffText(ITokenizedStringListInternal left,
            ITokenizedStringListInternal right, StringDifferenceTypes type, StringDifferenceOptions differenceOptions)
        {
            StringDifferenceOptions options = new StringDifferenceOptions(differenceOptions);
            options.DifferenceType &= ~type;
            return new HierarchicalDifferenceCollection(
                ComputeMatches(differenceOptions, left, right), left, right,
                this, options);
        }

        internal static List<Span> GetContiguousSpans(Span span, ITokenizedStringListInternal tokens)
        {
            List<Span> spanList = new List<Span>();
            int start1 = span.Start;
            for (int index = span.Start + 1; index < span.End; ++index)
            {
                Span elementInOriginal = tokens.GetElementInOriginal(index - 1);
                int end = elementInOriginal.End;
                elementInOriginal = tokens.GetElementInOriginal(index);
                int start2 = elementInOriginal.Start;
                if (end != start2)
                {
                    spanList.Add(Span.FromBounds(start1, index));
                    start1 = index;
                }
            }

            if (start1 < span.End)
                spanList.Add(Span.FromBounds(start1, span.End));
            return spanList;
        }

        internal static string DefaultGetLineTextCallback(ITextSnapshotLine line)
        {
            return line.GetTextIncludingLineBreak();
        }

        private static IDifferenceCollection<string> ComputeMatches(StringDifferenceOptions differenceOptions, IList<string> leftSequence, IList<string> rightSequence)
        {
            return ComputeMatches(differenceOptions, leftSequence,
                rightSequence, leftSequence, rightSequence);
        }

        private static IDifferenceCollection<string> ComputeMatches(StringDifferenceOptions differenceOptions, IList<string> leftSequence, IList<string> rightSequence,
            IList<string> originalLeftSequence, IList<string> originalRightSequence)
        {
            return (IDifferenceCollection<string>) MaximalSubsequenceAlgorithm.DifferenceSequences(leftSequence,
                rightSequence, originalLeftSequence, originalRightSequence,
                differenceOptions.ContinueProcessingPredicate);
        }
    }
}

    