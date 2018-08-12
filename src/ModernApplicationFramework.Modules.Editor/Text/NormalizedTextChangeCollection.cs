using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Modules.Editor.Utilities;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Differencing;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    internal class NormalizedTextChangeCollection : INormalizedTextChangeCollection
    {
        public static readonly NormalizedTextChangeCollection Empty = new NormalizedTextChangeCollection(new TextChange[0]);
        private readonly IReadOnlyList<TextChange> _changes;

        public static INormalizedTextChangeCollection Create(IReadOnlyList<TextChange> changes)
        {
            return GetTrivialCollection(changes) ?? new NormalizedTextChangeCollection(changes);
        }

        public static INormalizedTextChangeCollection Create(IReadOnlyList<TextChange> changes, StringDifferenceOptions? differenceOptions, ITextDifferencingService textDifferencingService, ITextSnapshot before = null, ITextSnapshot after = null)
        {
            return GetTrivialCollection(changes) ?? new NormalizedTextChangeCollection(changes, differenceOptions, textDifferencingService, before, after);
        }

        private static INormalizedTextChangeCollection GetTrivialCollection(IReadOnlyList<TextChange> changes)
        {
            if (changes == null)
                throw new ArgumentNullException(nameof(changes));
            if (changes.Count == 0)
                return Empty;
            if (changes.Count == 1)
            {
                var change = changes[0];
                if (change.OldLength + change.NewLength == 1 && change.LineBreakBoundaryConditions == LineBreakBoundaryConditions.None && change.LineCountDelta == 0)
                {
                    var isInsertion = change.NewLength == 1;
                    return new TrivialNormalizedTextChangeCollection(isInsertion ? change.NewText[0] : change.OldText[0], isInsertion, change.OldPosition);
                }
            }
            return null;
        }

        private NormalizedTextChangeCollection(IReadOnlyList<TextChange> changes)
        {
            _changes = Normalize(changes, new StringDifferenceOptions?(), null, null, null);
        }

        private NormalizedTextChangeCollection(IReadOnlyList<TextChange> changes, StringDifferenceOptions? differenceOptions, ITextDifferencingService textDifferencingService, ITextSnapshot before, ITextSnapshot after)
        {
            _changes = Normalize(changes, differenceOptions, textDifferencingService, before, after);
        }

        public bool IncludesLineChanges
        {
            get { return this.Any(textChange => textChange.LineCountDelta != 0); }
        }

        private static IReadOnlyList<TextChange> Normalize(IReadOnlyList<TextChange> changes, StringDifferenceOptions? differenceOptions, ITextDifferencingService textDifferencingService, ITextSnapshot before, ITextSnapshot after)
        {
            if (changes.Count == 1 && !differenceOptions.HasValue)
                return new[]
                {
                    changes[0]
                };
            if (changes.Count == 0)
                return new TextChange[0];
            var textChangeArray = TextUtilities.StableSort(changes, TextChange.Compare);
            var num1 = 0;
            var index1 = 0;
            var index2 = 1;
            while (index2 < textChangeArray.Length)
            {
                var textChange1 = textChangeArray[index1];
                var textChange2 = textChangeArray[index2];
                var num2 = textChange2.OldPosition - textChange1.OldEnd;
                if (num2 > 0)
                {
                    textChange1.NewPosition = textChange1.OldPosition + num1;
                    num1 += textChange1.Delta;
                    index1 = index2++;
                }
                else
                {
                    var stringRebuilder1 = textChange1._newText;
                    var stringRebuilder2 = textChange1._oldText;
                    var num3 = 0;
                    do
                    {
                        stringRebuilder1 = stringRebuilder1.Append(textChange2._newText);
                        if (num2 == 0)
                        {
                            stringRebuilder2 = stringRebuilder2.Append(textChange2._oldText);
                            num3 += textChange2.OldLength;
                            textChange1.LineBreakBoundaryConditions = textChange1.LineBreakBoundaryConditions & LineBreakBoundaryConditions.PrecedingReturn | textChange2.LineBreakBoundaryConditions & LineBreakBoundaryConditions.SucceedingNewline;
                        }
                        else if (textChange1.OldEnd + num3 < textChange2.OldEnd)
                        {
                            var start = textChange1.OldEnd + num3 - textChange2.OldPosition;
                            stringRebuilder2 = stringRebuilder2.Append(textChange2._oldText.GetSubText(Span.FromBounds(start, textChange2._oldText.Length)));
                            num3 += textChange2.OldLength - start;
                            textChange1.LineBreakBoundaryConditions = textChange1.LineBreakBoundaryConditions & LineBreakBoundaryConditions.PrecedingReturn | textChange2.LineBreakBoundaryConditions & LineBreakBoundaryConditions.SucceedingNewline;
                        }
                        textChangeArray[index2] = null;
                        ++index2;
                        if (index2 != textChangeArray.Length)
                        {
                            textChange2 = textChangeArray[index2];
                            num2 = textChange2.OldPosition - textChange1.OldEnd - num3;
                        }
                        else
                            break;
                    }
                    while (num2 <= 0);
                    textChangeArray[index1]._oldText = stringRebuilder2;
                    textChangeArray[index1]._newText = stringRebuilder1;
                    if (index2 < textChangeArray.Length)
                    {
                        textChange1.NewPosition = textChange1.OldPosition + num1;
                        num1 += textChange1.Delta;
                        index1 = index2++;
                    }
                }
            }
            textChangeArray[index1].NewPosition = textChangeArray[index1].OldPosition + num1;
            var textChangeList = new List<TextChange>();
            if (differenceOptions.HasValue)
            {
                if (textDifferencingService == null)
                    throw new ArgumentNullException("stringDifferenceUtility");
                foreach (var originalChange in textChangeArray)
                {
                    if (originalChange != null)
                    {
                        if (originalChange.OldLength == 0 || originalChange.NewLength == 0)
                            textChangeList.Add(originalChange);
                        else if (originalChange.OldLength >= TextModelOptions.DiffSizeThreshold || originalChange.NewLength >= TextModelOptions.DiffSizeThreshold)
                        {
                            originalChange.IsOpaque = true;
                            textChangeList.Add(originalChange);
                        }
                        else
                        {
                            var differenceOptions1 =
                                new StringDifferenceOptions(differenceOptions.Value) {IgnoreTrimWhiteSpace = false};
                            IHierarchicalDifferenceCollection diffCollection;
                            if (before != null && after != null)
                            {
                                diffCollection = textDifferencingService.DiffSnapshotSpans(new SnapshotSpan(before, originalChange.OldSpan), new SnapshotSpan(after, originalChange.NewSpan), differenceOptions1);
                            }
                            else
                            {
                                var oldText = originalChange.OldText;
                                var newText = originalChange.NewText;
                                if (oldText != newText)
                                    diffCollection = textDifferencingService.DiffStrings(oldText, newText, differenceOptions1);
                                else
                                    continue;
                            }
                            var delta = 0;
                            textChangeList.AddRange(GetChangesFromDifferenceCollection(ref delta, originalChange, originalChange._oldText, originalChange._newText, diffCollection, 0, 0));
                        }
                    }
                }
            }
            else
            {
                foreach (var textChange in textChangeArray)
                {
                    if (textChange != null)
                        textChangeList.Add(textChange);
                }
            }
            return textChangeList;
        }

        private static IList<TextChange> GetChangesFromDifferenceCollection(ref int delta, TextChange originalChange, StringRebuilder oldText, StringRebuilder newText, IHierarchicalDifferenceCollection diffCollection, int leftOffset = 0, int rightOffset = 0)
        {
            var textChangeList = new List<TextChange>();
            for (var index = 0; index < diffCollection.Differences.Count; ++index)
            {
                var difference = diffCollection.Differences[index];
                var span1 = Translate(diffCollection.LeftDecomposition.GetSpanInOriginal(difference.Left), leftOffset);
                var span2 = Translate(diffCollection.RightDecomposition.GetSpanInOriginal(difference.Right), rightOffset);
                var containedDifferences = diffCollection.GetContainedDifferences(index);
                if (containedDifferences != null)
                {
                    textChangeList.AddRange(GetChangesFromDifferenceCollection(ref delta, originalChange, oldText, newText, containedDifferences, span1.Start, span2.Start));
                }
                else
                {
                    var textChange = new TextChange(originalChange.OldPosition + span1.Start, oldText.GetSubText(span1),
                        newText.GetSubText(span2), ComputeBoundaryConditions(originalChange, oldText, span1))
                    {
                        NewPosition = originalChange.NewPosition + span2.Start
                    };
                    if (textChange.OldLength > 0 && textChange.NewLength > 0)
                        textChange.IsOpaque = true;
                    delta += textChange.Delta;
                    textChangeList.Add(textChange);
                }
            }
            return textChangeList;
        }

        private static LineBreakBoundaryConditions ComputeBoundaryConditions(TextChange outerChange, StringRebuilder oldText, Span leftSpan)
        {
            var boundaryConditions = LineBreakBoundaryConditions.None;
            if (leftSpan.Start == 0)
                boundaryConditions = outerChange.LineBreakBoundaryConditions & LineBreakBoundaryConditions.PrecedingReturn;
            else if (oldText[leftSpan.Start - 1] == '\r')
                boundaryConditions = LineBreakBoundaryConditions.PrecedingReturn;
            if (leftSpan.End == oldText.Length)
                boundaryConditions |= outerChange.LineBreakBoundaryConditions & LineBreakBoundaryConditions.SucceedingNewline;
            else if (oldText[leftSpan.End] == '\n')
                boundaryConditions |= LineBreakBoundaryConditions.SucceedingNewline;
            return boundaryConditions;
        }

        private static Span Translate(Span span, int amount)
        {
            return new Span(span.Start + amount, span.Length);
        }

        private static bool PriorTo(ITextChange denormalizedChange, ITextChange normalizedChange, int accumulatedDelta, int accumulatedNormalizedDelta)
        {
            if (denormalizedChange.OldLength != 0 && normalizedChange.OldLength != 0)
                return denormalizedChange.OldPosition <= normalizedChange.NewPosition - accumulatedDelta - accumulatedNormalizedDelta;
            return denormalizedChange.OldPosition < normalizedChange.NewPosition - accumulatedDelta - accumulatedNormalizedDelta;
        }

        public static void Denormalize(INormalizedTextChangeCollection normalizedChanges, List<TextChange> denormChangesWithSentinel)
        {
            var index1 = 0;
            var accumulatedDelta = 0;
            var accumulatedNormalizedDelta = 0;
            var textChangeList = new List<ITextChange>(normalizedChanges);
            for (var index2 = 0; index2 < textChangeList.Count; ++index2)
            {
                var textChange1 = textChangeList[index2];
                while (PriorTo(denormChangesWithSentinel[index1], textChange1, accumulatedDelta, accumulatedNormalizedDelta))
                    accumulatedDelta += denormChangesWithSentinel[index1++].Delta;
                if (textChange1.OldEnd - accumulatedDelta > denormChangesWithSentinel[index1].OldPosition)
                {
                    var length = textChange1.OldEnd - accumulatedDelta - denormChangesWithSentinel[index1].OldPosition;
                    var num1 = textChange1.OldLength - length;
                    var num2 = textChange1.NewPosition - textChange1.OldPosition;
                    denormChangesWithSentinel.Insert(index1, new TextChange(textChange1.OldPosition - accumulatedDelta, TextChange.ChangeOldSubText(textChange1, 0, num1), TextChange.NewStringRebuilder(textChange1), LineBreakBoundaryConditions.None));
                    accumulatedNormalizedDelta += num2;
                    var textChange2 = new TextChange(textChange1.OldPosition + num1, TextChange.ChangeOldSubText(textChange1, num1, length), StringRebuilder.Empty, LineBreakBoundaryConditions.None);
                    textChange2.NewPosition += num2;
                    textChangeList.Insert(index2 + 1, textChange2);
                }
                else
                {
                    denormChangesWithSentinel.Insert(index1, new TextChange(textChange1.OldPosition - accumulatedDelta, TextChange.OldStringRebuilder(textChange1), TextChange.NewStringRebuilder(textChange1), LineBreakBoundaryConditions.None));
                    accumulatedNormalizedDelta += textChange1.Delta;
                }
                ++index1;
            }
        }

        int ICollection<ITextChange>.Count => _changes.Count;

        bool ICollection<ITextChange>.IsReadOnly => true;

        ITextChange IList<ITextChange>.this[int index]
        {
            get => _changes[index];
            set => throw new NotSupportedException();
        }

        int IList<ITextChange>.IndexOf(ITextChange item)
        {
            for (var index = 0; index < _changes.Count; ++index)
            {
                if (item.Equals(_changes[index]))
                    return index;
            }
            return -1;
        }

        void IList<ITextChange>.Insert(int index, ITextChange item)
        {
            throw new NotSupportedException();
        }

        void IList<ITextChange>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection<ITextChange>.Add(ITextChange item)
        {
            throw new NotSupportedException();
        }

        void ICollection<ITextChange>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<ITextChange>.Contains(ITextChange item)
        {
            return ((IList<ITextChange>)this).IndexOf(item) != -1;
        }

        void ICollection<ITextChange>.CopyTo(ITextChange[] array, int arrayIndex)
        {
            for (var index = 0; index < _changes.Count; ++index)
                array[index + arrayIndex] = _changes[index];
        }

        bool ICollection<ITextChange>.Remove(ITextChange item)
        {
            throw new NotSupportedException();
        }

        IEnumerator<ITextChange> IEnumerable<ITextChange>.GetEnumerator()
        {
            return _changes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _changes.GetEnumerator();
        }
    }
}