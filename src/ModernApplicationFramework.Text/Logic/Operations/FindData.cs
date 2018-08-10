using System;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Text.Logic.Operations
{
    public struct FindData
    {
        private string _searchString;
        private ITextSnapshot _textSnapshotToSearch;

        public FindData(string searchPattern, ITextSnapshot textSnapshot, FindOptions findOptions, ITextStructureNavigator textStructureNavigator)
        {
            if (searchPattern == null)
                throw new ArgumentNullException(nameof(searchPattern));
            if (searchPattern.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(searchPattern));
            _searchString = searchPattern;
            _textSnapshotToSearch = textSnapshot ?? throw new ArgumentNullException(nameof(textSnapshot));
            FindOptions = findOptions;
            TextStructureNavigator = textStructureNavigator;
        }

        public FindData(string searchPattern, ITextSnapshot textSnapshot)
        {
            this = new FindData(searchPattern, textSnapshot, FindOptions.None, null);
        }

        internal FindData(ITextSnapshot textSnapshot)
        {
            _searchString = null;
            _textSnapshotToSearch = textSnapshot;
            FindOptions = FindOptions.None;
            TextStructureNavigator = null;
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _searchString = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FindData))
                return false;
            FindData findData = (FindData)obj;
            if (_searchString == findData._searchString && FindOptions == findData.FindOptions && _textSnapshotToSearch == findData._textSnapshotToSearch)
                return TextStructureNavigator == findData.TextStructureNavigator;
            return false;
        }

        public override int GetHashCode()
        {
            return _searchString.GetHashCode();
        }

        public static bool operator ==(FindData data1, FindData data2)
        {
            return data1.Equals(data2);
        }

        public static bool operator !=(FindData data1, FindData data2)
        {
            return data1.Equals(data2);
        }

        public FindOptions FindOptions { get; set; }

        public ITextSnapshot TextSnapshotToSearch
        {
            get => _textSnapshotToSearch;
            set => _textSnapshotToSearch = value ?? throw new ArgumentNullException(nameof(value));
        }

        public ITextStructureNavigator TextStructureNavigator { get; set; }
    }
}