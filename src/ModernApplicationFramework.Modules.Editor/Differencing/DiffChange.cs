using System;

namespace ModernApplicationFramework.Modules.Editor.Differencing
{
    internal class DiffChange : IDiffChange
    {
        private DiffChangeType _mChangeType;
        private int _mOriginalStart;
        private int _mOriginalLength;
        private int _mModifiedStart;
        private int _mModifiedLength;
        private bool _mUpdateChangeType;

        internal DiffChange(int originalStart, int originalLength, int modifiedStart, int modifiedLength)
        {
            _mOriginalStart = originalStart;
            _mOriginalLength = originalLength;
            _mModifiedStart = modifiedStart;
            _mModifiedLength = modifiedLength;
            UpdateChangeType();
        }

        private void UpdateChangeType()
        {
            if (_mOriginalLength > 0)
                _mChangeType = _mModifiedLength <= 0 ? DiffChangeType.Delete : DiffChangeType.Change;
            else if (_mModifiedLength > 0)
                _mChangeType = DiffChangeType.Insert;
            _mUpdateChangeType = false;
        }

        public DiffChangeType ChangeType
        {
            get
            {
                if (_mUpdateChangeType)
                    UpdateChangeType();
                return _mChangeType;
            }
        }

        public int OriginalStart
        {
            get => _mOriginalStart;
            set
            {
                _mOriginalStart = value;
                _mUpdateChangeType = true;
            }
        }

        public int OriginalLength
        {
            get => _mOriginalLength;
            set
            {
                _mOriginalLength = value;
                _mUpdateChangeType = true;
            }
        }

        public int OriginalEnd => OriginalStart + OriginalLength;

        public int ModifiedStart
        {
            get => _mModifiedStart;
            set
            {
                _mModifiedStart = value;
                _mUpdateChangeType = true;
            }
        }

        public int ModifiedLength
        {
            get => _mModifiedLength;
            set
            {
                _mModifiedLength = value;
                _mUpdateChangeType = true;
            }
        }

        public int ModifiedEnd => ModifiedStart + ModifiedLength;

        public IDiffChange Add(IDiffChange diffChange)
        {
            if (diffChange == null)
                return this;
            var originalStart = Math.Min(OriginalStart, diffChange.OriginalStart);
            var num1 = Math.Max(OriginalEnd, diffChange.OriginalEnd);
            var modifiedStart = Math.Min(ModifiedStart, diffChange.ModifiedStart);
            var num2 = Math.Max(ModifiedEnd, diffChange.ModifiedEnd);
            return new DiffChange(originalStart, num1 - originalStart, modifiedStart, num2 - modifiedStart);
        }
    }
}