using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.EditorBase.FileSupport;

namespace ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters
{
    public class FilterData
    {
        public string Filter => ToString();

        public int MaxIndex
        {
            get
            {
                if (AnyFilter != null)
                    return Filters.Count + 1;
                return Filters.Count;
            }
        }

        public bool AddFilterAnyFileAtEnd { get; set; } = false;

        private FilterDataEntry AnyFilter { get; set; }

        private IList<FilterDataEntry> Filters { get; }

        private int _anyFilterOriginalIndex;

        public FilterData()
        {
            Filters = new List<FilterDataEntry>();
        }


        public void AddFilter(FilterDataEntry entry)
        {
            Filters.Add(entry);
        }

        public void AddFilterAnyFile(string text)
        {
            AnyFilter = new FilterDataEntry(text, ".*");
            //if (MaxIndex == 0)
            //    _anyFilterOriginalIndex = 0;
            //else
            _anyFilterOriginalIndex = MaxIndex;
        }

        public void AddFilterAnyFile()
        {
            AddFilterAnyFile(FileSupportResources.OpenSaveFileFilterAnyText);
        }

        public void RemoveFileAnyFile()
        {
            AnyFilter = null;
            _anyFilterOriginalIndex = -1;
        }


        public override string ToString()
        {
            if (!Filters.Any() && AnyFilter == null)
                return string.Empty;
            if (!Filters.Any() && AnyFilter != null)
                return AnyFilter.ToString();
            var realFilterList = InternalBuildFilterList();
            var filter = string.Empty;
            foreach (var entry in realFilterList)
            {
                var entryString = entry.ToString();
                if (string.IsNullOrEmpty(entryString))
                {
                    if (MaxIndex == 1)
                        return string.Empty;
                }
                else
                    filter += $"|{entryString}";
            }
            filter = filter.Remove(0, 1);
            return filter;
        }

        private IEnumerable<FilterDataEntry> InternalBuildFilterList()
        {
            if (AnyFilter == null)
                return Filters;
            if (AnyFilter != null && AddFilterAnyFileAtEnd)
            {
                var newList = Filters.ToList();
                newList.Add(AnyFilter);
                return newList;
            }
            else
            {
                var newList = Filters.ToList();
                newList.Insert(_anyFilterOriginalIndex -1, AnyFilter);
                return newList;
            }
        }
    }
}