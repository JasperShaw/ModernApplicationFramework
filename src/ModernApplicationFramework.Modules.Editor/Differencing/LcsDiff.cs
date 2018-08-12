using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class LcsDiff<T> : DiffFinder<T>
    {
        private List<int[]> _mForwardHistory;
        public List<int[]> MReverseHistory;

        public LcsDiff()
        {
            _mForwardHistory = new List<int[]>(1447);
            MReverseHistory = new List<int[]>(1447);
        }

        public override void Dispose()
        {
            base.Dispose();
            _mForwardHistory = null;
            MReverseHistory = null;
            GC.SuppressFinalize(this);
        }

        protected override IDiffChange[] ComputeDiff(int originalStart, int originalEnd, int modifiedStart, int modifiedEnd)
        {
            return ShiftChanges(ComputeDiffRecursive(originalStart, originalEnd, modifiedStart, modifiedEnd, out _));
        }

        private IDiffChange[] ComputeDiffRecursive(int originalStart, int originalEnd, int modifiedStart, int modifiedEnd, out bool quitEarly)
        {
            quitEarly = false;
            for (; originalStart <= originalEnd && modifiedStart <= modifiedEnd && ElementsAreEqual(originalStart, modifiedStart); ++modifiedStart)
                ++originalStart;
            for (; originalEnd >= originalStart && modifiedEnd >= modifiedStart && ElementsAreEqual(originalEnd, modifiedEnd); --modifiedEnd)
                --originalEnd;
            if (originalStart > originalEnd || modifiedStart > modifiedEnd)
            {
                IDiffChange[] diffChangeArray;
                if (modifiedStart <= modifiedEnd)
                    diffChangeArray = new IDiffChange[]
                    {
                        new DiffChange(originalStart, 0, modifiedStart, modifiedEnd - modifiedStart + 1)
                    };
                else if (originalStart <= originalEnd)
                    diffChangeArray = new IDiffChange[]
                    {
                        new DiffChange(originalStart, originalEnd - originalStart + 1, modifiedStart, 0)
                    };
                else
                    diffChangeArray = new IDiffChange[0];
                return diffChangeArray;
            }

            var recursionPoint = ComputeRecursionPoint(originalStart, originalEnd, modifiedStart, modifiedEnd, out var midOriginal, out var midModified, out quitEarly);
            if (recursionPoint != null)
                return recursionPoint;
            if (!quitEarly)
            {
                var diffRecursive = ComputeDiffRecursive(originalStart, midOriginal, modifiedStart, midModified, out quitEarly);
                IDiffChange[] right;
                if (!quitEarly)
                    right = ComputeDiffRecursive(midOriginal + 1, originalEnd, midModified + 1, modifiedEnd, out quitEarly);
                else
                    right = new IDiffChange[]
                    {
                        new DiffChange(midOriginal + 1, originalEnd - (midOriginal + 1) + 1, midModified + 1, modifiedEnd - (midModified + 1) + 1)
                    };
                return ConcatenateChanges(diffRecursive, right);
            }
            return new IDiffChange[]
            {
                new DiffChange(originalStart, originalEnd - originalStart + 1, modifiedStart, modifiedEnd - modifiedStart + 1)
            };
        }

        private IDiffChange[] ComputeRecursionPoint(int originalStart, int originalEnd, int modifiedStart, int modifiedEnd, out int midOriginal, out int midModified, out bool quitEarly)
        {
            var sourceIndex1 = 0;
            var num1 = 0;
            var sourceIndex2 = 0;
            var num2 = 0;
            --originalStart;
            --modifiedStart;
            midOriginal = 0;
            midModified = 0;
            _mForwardHistory.Clear();
            MReverseHistory.Clear();
            var num3 = originalEnd - originalStart + (modifiedEnd - modifiedStart);
            var numDiagonals = num3 + 1;
            var numArray1 = new int[numDiagonals];
            var numArray2 = new int[numDiagonals];
            var diagonalBaseIndex1 = modifiedEnd - modifiedStart;
            var diagonalBaseIndex2 = originalEnd - originalStart;
            var num4 = originalStart - modifiedStart;
            var num5 = originalEnd - modifiedEnd;
            var flag = (diagonalBaseIndex2 - diagonalBaseIndex1) % 2 == 0;
            numArray1[diagonalBaseIndex1] = originalStart;
            numArray2[diagonalBaseIndex2] = originalEnd;
            quitEarly = false;
            for (var numDifferences = 1; numDifferences <= num3 / 2 + 1; ++numDifferences)
            {
                var originalIndex1 = 0;
                var num6 = 0;
                sourceIndex1 = ClipDiagonalBound(diagonalBaseIndex1 - numDifferences, numDifferences, diagonalBaseIndex1, numDiagonals);
                num1 = ClipDiagonalBound(diagonalBaseIndex1 + numDifferences, numDifferences, diagonalBaseIndex1, numDiagonals);
                var index1 = sourceIndex1;
                while (index1 <= num1)
                {
                    var num7 = index1 == sourceIndex1 || index1 < num1 && numArray1[index1 - 1] < numArray1[index1 + 1] ? numArray1[index1 + 1] : numArray1[index1 - 1] + 1;
                    var num8 = num7 - (index1 - diagonalBaseIndex1) - num4;
                    var num9 = num7;
                    for (; num7 < originalEnd && num8 < modifiedEnd && ElementsAreEqual(num7 + 1, num8 + 1); ++num8)
                        ++num7;
                    numArray1[index1] = num7;
                    if (num7 + num8 > originalIndex1 + num6)
                    {
                        originalIndex1 = num7;
                        num6 = num8;
                    }
                    if (!flag && Math.Abs(index1 - diagonalBaseIndex2) <= numDifferences - 1 && num7 >= numArray2[index1])
                    {
                        midOriginal = num7;
                        midModified = num8;
                        if (num9 > numArray2[index1] || numDifferences > 1448)
                            return null;
                        goto label_28;
                    }
                    else
                        index1 += 2;
                }
                var longestMatchSoFar = (originalIndex1 - originalStart + (num6 - modifiedStart) - numDifferences) / 2;
                if (ContinueDifferencePredicate != null && !ContinueDifferencePredicate(originalIndex1, OriginalSequence, longestMatchSoFar))
                {
                    quitEarly = true;
                    midOriginal = originalIndex1;
                    midModified = num6;
                    if (longestMatchSoFar <= 0 || numDifferences > 1448)
                    {
                        ++originalStart;
                        ++modifiedStart;
                        return new IDiffChange[]
                        {
                            new DiffChange(originalStart, originalEnd - originalStart + 1, modifiedStart, modifiedEnd - modifiedStart + 1)
                        };
                    }
                    break;
                }
                sourceIndex2 = ClipDiagonalBound(diagonalBaseIndex2 - numDifferences, numDifferences, diagonalBaseIndex2, numDiagonals);
                num2 = ClipDiagonalBound(diagonalBaseIndex2 + numDifferences, numDifferences, diagonalBaseIndex2, numDiagonals);
                var index2 = sourceIndex2;
                while (index2 <= num2)
                {
                    var originalIndex2 = index2 == sourceIndex2 || index2 < num2 && numArray2[index2 - 1] >= numArray2[index2 + 1] ? numArray2[index2 + 1] - 1 : numArray2[index2 - 1];
                    var modifiedIndex = originalIndex2 - (index2 - diagonalBaseIndex2) - num5;
                    var num7 = originalIndex2;
                    for (; originalIndex2 > originalStart && modifiedIndex > modifiedStart && ElementsAreEqual(originalIndex2, modifiedIndex); --modifiedIndex)
                        --originalIndex2;
                    numArray2[index2] = originalIndex2;
                    if (flag && Math.Abs(index2 - diagonalBaseIndex1) <= numDifferences && originalIndex2 <= numArray1[index2])
                    {
                        midOriginal = originalIndex2;
                        midModified = modifiedIndex;
                        if (num7 < numArray1[index2] || numDifferences > 1448)
                            return null;
                        goto label_28;
                    }
                    else
                        index2 += 2;
                }
                if (numDifferences <= 1447)
                {
                    var numArray3 = new int[num1 - sourceIndex1 + 2];
                    numArray3[0] = diagonalBaseIndex1 - sourceIndex1 + 1;
                    Array.Copy(numArray1, sourceIndex1, numArray3, 1, num1 - sourceIndex1 + 1);
                    _mForwardHistory.Add(numArray3);
                    var numArray4 = new int[num2 - sourceIndex2 + 2];
                    numArray4[0] = diagonalBaseIndex2 - sourceIndex2 + 1;
                    Array.Copy(numArray2, sourceIndex2, numArray4, 1, num2 - sourceIndex2 + 1);
                    MReverseHistory.Add(numArray4);
                }
            }
            label_28:
            IDiffChange[] reverseChanges;
            using (var diffChangeHelper = new DiffChangeHelper())
            {
                var num6 = sourceIndex1;
                var num7 = num1;
                var num8 = midOriginal - midModified - num4;
                var num9 = int.MinValue;
                var index = _mForwardHistory.Count - 1;
                do
                {
                    var num10 = num8 + diagonalBaseIndex1;
                    if (num10 == num6 || num10 < num7 && numArray1[num10 - 1] < numArray1[num10 + 1])
                    {
                        var num11 = numArray1[num10 + 1];
                        var modifiedIndex = num11 - num8 - num4;
                        if (num11 < num9)
                            diffChangeHelper.MarkNextChange();
                        num9 = num11;
                        diffChangeHelper.AddModifiedElement(num11 + 1, modifiedIndex);
                        num8 = num10 + 1 - diagonalBaseIndex1;
                    }
                    else
                    {
                        var originalIndex = numArray1[num10 - 1] + 1;
                        var num11 = originalIndex - num8 - num4;
                        if (originalIndex < num9)
                            diffChangeHelper.MarkNextChange();
                        num9 = originalIndex - 1;
                        diffChangeHelper.AddOriginalElement(originalIndex, num11 + 1);
                        num8 = num10 - 1 - diagonalBaseIndex1;
                    }
                    if (index >= 0)
                    {
                        numArray1 = _mForwardHistory[index];
                        diagonalBaseIndex1 = numArray1[0];
                        num6 = 1;
                        num7 = numArray1.Length - 1;
                    }
                }
                while (--index >= -1);
                reverseChanges = diffChangeHelper.ReverseChanges;
            }
            IDiffChange[] right;
            if (quitEarly)
            {
                var num6 = midOriginal + 1;
                var num7 = midModified + 1;
                if (reverseChanges != null && reverseChanges.Length != 0)
                {
                    var diffChange = reverseChanges[reverseChanges.Length - 1];
                    num6 = Math.Max(num6, diffChange.OriginalEnd);
                    num7 = Math.Max(num7, diffChange.ModifiedEnd);
                }
                right = new IDiffChange[]
                {
                    new DiffChange(num6, originalEnd - num6 + 1, num7, modifiedEnd - num7 + 1)
                };
            }
            else
            {
                using (var diffChangeHelper = new DiffChangeHelper())
                {
                    var num6 = sourceIndex2;
                    var num7 = num2;
                    var num8 = midOriginal - midModified - num5;
                    var num9 = int.MaxValue;
                    var index = flag ? MReverseHistory.Count - 1 : MReverseHistory.Count - 2;
                    do
                    {
                        var num10 = num8 + diagonalBaseIndex2;
                        if (num10 == num6 || num10 < num7 && numArray2[num10 - 1] >= numArray2[num10 + 1])
                        {
                            var num11 = numArray2[num10 + 1] - 1;
                            var num12 = num11 - num8 - num5;
                            if (num11 > num9)
                                diffChangeHelper.MarkNextChange();
                            num9 = num11 + 1;
                            diffChangeHelper.AddOriginalElement(num11 + 1, num12 + 1);
                            num8 = num10 + 1 - diagonalBaseIndex2;
                        }
                        else
                        {
                            var num11 = numArray2[num10 - 1];
                            var num12 = num11 - num8 - num5;
                            if (num11 > num9)
                                diffChangeHelper.MarkNextChange();
                            num9 = num11;
                            diffChangeHelper.AddModifiedElement(num11 + 1, num12 + 1);
                            num8 = num10 - 1 - diagonalBaseIndex2;
                        }
                        if (index >= 0)
                        {
                            numArray2 = MReverseHistory[index];
                            diagonalBaseIndex2 = numArray2[0];
                            num6 = 1;
                            num7 = numArray2.Length - 1;
                        }
                    }
                    while (--index >= -1);
                    right = diffChangeHelper.Changes;
                }
            }
            return ConcatenateChanges(reverseChanges, right);
        }

        private IDiffChange[] ShiftChanges(IDiffChange[] changes)
        {
            for (var index = 0; index < changes.Length; ++index)
            {
                var change = changes[index] as DiffChange;
                var num1 = index < changes.Length - 1 ? changes[index + 1].OriginalStart : OriginalSequence.Count;
                var num2 = index < changes.Length - 1 ? changes[index + 1].ModifiedStart : ModifiedSequence.Count;
                var flag1 = change.OriginalLength > 0;
                for (var flag2 = change.ModifiedLength > 0; change.OriginalStart + change.OriginalLength < num1 && change.ModifiedStart + change.ModifiedLength < num2 && (!flag1 || OriginalElementsAreEqual(change.OriginalStart, change.OriginalStart + change.OriginalLength)) && (!flag2 || ModifiedElementsAreEqual(change.ModifiedStart, change.ModifiedStart + change.ModifiedLength)); ++change.ModifiedStart)
                    ++change.OriginalStart;
            }
            var diffChangeList = new List<IDiffChange>(changes.Length);
            for (var index = 0; index < changes.Length; ++index)
            {
                if (index < changes.Length - 1 && ChangesOverlap(changes[index], changes[index + 1], out var mergedChange))
                {
                    diffChangeList.Add(mergedChange);
                    ++index;
                }
                else
                    diffChangeList.Add(changes[index]);
            }
            return diffChangeList.ToArray();
        }

        private IDiffChange[] ConcatenateChanges(IDiffChange[] left, IDiffChange[] right)
        {
            if (left.Length == 0 || right.Length == 0)
            {
                if (right.Length == 0)
                    return left;
                return right;
            }

            if (ChangesOverlap(left[left.Length - 1], right[0], out var mergedChange))
            {
                var diffChangeArray = new IDiffChange[left.Length + right.Length - 1];
                Array.Copy(left, 0, diffChangeArray, 0, left.Length - 1);
                diffChangeArray[left.Length - 1] = mergedChange;
                Array.Copy(right, 1, diffChangeArray, left.Length, right.Length - 1);
                return diffChangeArray;
            }
            var diffChangeArray1 = new IDiffChange[left.Length + right.Length];
            Array.Copy(left, 0, diffChangeArray1, 0, left.Length);
            Array.Copy(right, 0, diffChangeArray1, left.Length, right.Length);
            return diffChangeArray1;
        }

        private bool ChangesOverlap(IDiffChange left, IDiffChange right, out IDiffChange mergedChange)
        {
            if (left.OriginalStart + left.OriginalLength >= right.OriginalStart || left.ModifiedStart + left.ModifiedLength >= right.ModifiedStart)
            {
                var originalStart = left.OriginalStart;
                var originalLength = left.OriginalLength;
                var modifiedStart = left.ModifiedStart;
                var modifiedLength = left.ModifiedLength;
                if (left.OriginalStart + left.OriginalLength >= right.OriginalStart)
                    originalLength = right.OriginalStart + right.OriginalLength - left.OriginalStart;
                if (left.ModifiedStart + left.ModifiedLength >= right.ModifiedStart)
                    modifiedLength = right.ModifiedStart + right.ModifiedLength - left.ModifiedStart;
                mergedChange = new DiffChange(originalStart, originalLength, modifiedStart, modifiedLength);
                return true;
            }
            mergedChange = null;
            return false;
        }

        private int ClipDiagonalBound(int diagonal, int numDifferences, int diagonalBaseIndex, int numDiagonals)
        {
            if (diagonal >= 0 && diagonal < numDiagonals)
                return diagonal;
            var num1 = diagonalBaseIndex;
            var num2 = numDiagonals - diagonalBaseIndex - 1;
            var flag1 = numDifferences % 2 == 0;
            if (diagonal < 0)
            {
                var flag2 = num1 % 2 == 0;
                return flag1 != flag2 ? 1 : 0;
            }
            var flag3 = num2 % 2 == 0;
            if (flag1 != flag3)
                return numDiagonals - 2;
            return numDiagonals - 1;
        }
    }
}