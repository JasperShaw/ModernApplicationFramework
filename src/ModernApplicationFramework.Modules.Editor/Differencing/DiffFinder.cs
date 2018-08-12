using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    public delegate bool ContinueDifferencePredicate<T>(int originalIndex, IList<T> originalSequence,
        int longestMatchSoFar);

    public abstract class DiffFinder<T> : IDisposable
    {
        private int[] _mModifiedIds;
        private int[] _mOriginalIds;

        public static DiffFinder<T> LcsDiff => new LcsDiff<T>();

        protected ContinueDifferencePredicate<T> ContinueDifferencePredicate { get; private set; }

        protected IEqualityComparer<T> ElementComparer { get; private set; }

        protected IList<T> ModifiedSequence { get; private set; }

        protected IList<T> OriginalSequence { get; private set; }

        public IDiffChange[] Diff(IList<T> original, IList<T> modified, IEqualityComparer<T> elementComparer)
        {
            return Diff(original, modified, elementComparer, null);
        }

        public IDiffChange[] Diff(IList<T> original, IList<T> modified, IEqualityComparer<T> elementComparer,
            ContinueDifferencePredicate<T> predicate)
        {
            OriginalSequence = original;
            ModifiedSequence = modified;
            ElementComparer = elementComparer;
            ContinueDifferencePredicate = predicate;
            _mOriginalIds = new int[OriginalSequence.Count];
            _mModifiedIds = new int[ModifiedSequence.Count];
            var num1 = 0;
            var num2 = OriginalSequence.Count - 1;
            var num3 = 0;
            int num4;
            for (num4 = ModifiedSequence.Count - 1;
                num1 <= num2 && num3 <= num4 && ElementsAreEqual(num1, num3);
                ++num3)
                ++num1;
            for (; num2 >= num1 && num4 >= num3 && ElementsAreEqual(num2, num4); --num4)
                --num2;
            if (num1 > num2 || num3 > num4)
            {
                IDiffChange[] diffChangeArray;
                if (num3 <= num4)
                    diffChangeArray = new IDiffChange[]
                    {
                        new DiffChange(num1, 0, num3, num4 - num3 + 1)
                    };
                else if (num1 <= num2)
                    diffChangeArray = new IDiffChange[]
                    {
                        new DiffChange(num1, num2 - num1 + 1, num3, 0)
                    };
                else
                    diffChangeArray = new IDiffChange[0];
                return diffChangeArray;
            }

            ComputeUniqueIdentifiers(num1, num2, num3, num4);
            return ComputeDiff(num1, num2, num3, num4);
        }

        public virtual void Dispose()
        {
            _mOriginalIds = null;
            _mModifiedIds = null;
            GC.SuppressFinalize(this);
        }

        protected abstract IDiffChange[] ComputeDiff(int originalStart, int originalEnd, int modifiedStart,
            int modifiedEnd);

        protected bool ElementsAreEqual(int originalIndex, int modifiedIndex)
        {
            return ElementsAreEqual(originalIndex, true, modifiedIndex, false);
        }

        protected bool ModifiedElementsAreEqual(int firstIndex, int secondIndex)
        {
            return ElementsAreEqual(firstIndex, false, secondIndex, false);
        }

        protected bool OriginalElementsAreEqual(int firstIndex, int secondIndex)
        {
            return ElementsAreEqual(firstIndex, true, secondIndex, true);
        }

        private void ComputeUniqueIdentifiers(int originalStart, int originalEnd, int modifiedStart, int modifiedEnd)
        {
            var dictionary = new Dictionary<T, int>(OriginalSequence.Count + ModifiedSequence.Count, ElementComparer);
            var num = 1;
            for (var index = originalStart; index <= originalEnd; ++index)
            {
                var key = OriginalSequence[index];
                if (!dictionary.TryGetValue(key, out _mOriginalIds[index]))
                {
                    _mOriginalIds[index] = num++;
                    dictionary.Add(key, _mOriginalIds[index]);
                }
            }

            for (var index = modifiedStart; index <= modifiedEnd; ++index)
            {
                var key = ModifiedSequence[index];
                if (!dictionary.TryGetValue(key, out _mModifiedIds[index]))
                {
                    _mModifiedIds[index] = num++;
                    dictionary.Add(key, _mModifiedIds[index]);
                }
            }
        }

        private bool ElementsAreEqual(int firstIndex, bool firstIsOriginal, int secondIndex, bool secondIsOriginal)
        {
            var num1 = firstIsOriginal ? _mOriginalIds[firstIndex] : _mModifiedIds[firstIndex];
            var num2 = secondIsOriginal ? _mOriginalIds[secondIndex] : _mModifiedIds[secondIndex];
            if (num1 != 0 && num2 != 0)
                return num1 == num2;
            return ElementComparer.Equals(firstIsOriginal ? OriginalSequence[firstIndex] : ModifiedSequence[firstIndex],
                secondIsOriginal ? OriginalSequence[secondIndex] : ModifiedSequence[secondIndex]);
        }
    }
}