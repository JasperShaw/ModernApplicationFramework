using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DiffChangeHelper : IDisposable
    {
        private int _mOriginalCount;
        private int _mModifiedCount;
        private List<IDiffChange> _mChanges;
        private int _mOriginalStart;
        private int _mModifiedStart;

        public DiffChangeHelper()
        {
            _mChanges = new List<IDiffChange>();
            _mOriginalStart = int.MaxValue;
            _mModifiedStart = int.MaxValue;
        }

        public void Dispose()
        {
            _mChanges = null;
            GC.SuppressFinalize(this);
        }

        public void MarkNextChange()
        {
            if (_mOriginalCount > 0 || _mModifiedCount > 0)
                _mChanges.Add(new DiffChange(_mOriginalStart, _mOriginalCount, _mModifiedStart, _mModifiedCount));
            _mOriginalCount = 0;
            _mModifiedCount = 0;
            _mOriginalStart = int.MaxValue;
            _mModifiedStart = int.MaxValue;
        }

        public void AddOriginalElement(int originalIndex, int modifiedIndex)
        {
            _mOriginalStart = Math.Min(_mOriginalStart, originalIndex);
            _mModifiedStart = Math.Min(_mModifiedStart, modifiedIndex);
            ++_mOriginalCount;
        }

        public void AddModifiedElement(int originalIndex, int modifiedIndex)
        {
            _mOriginalStart = Math.Min(_mOriginalStart, originalIndex);
            _mModifiedStart = Math.Min(_mModifiedStart, modifiedIndex);
            ++_mModifiedCount;
        }

        public IDiffChange[] Changes
        {
            get
            {
                if (_mOriginalCount > 0 || _mModifiedCount > 0)
                    MarkNextChange();
                return _mChanges.ToArray();
            }
        }

        public IDiffChange[] ReverseChanges
        {
            get
            {
                if (_mOriginalCount > 0 || _mModifiedCount > 0)
                    MarkNextChange();
                _mChanges.Reverse();
                return _mChanges.ToArray();
            }
        }
    }
}