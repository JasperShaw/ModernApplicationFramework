using System;
using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.EditorBase.FileSupport;

namespace ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters
{
    public class FilterData
    {
        public string Filter => ToString();

        public int MaxIndex => Filters.Count;

        public bool AddFilterAnyFileAtEnd { get; set; } = false;

        private FilterDataEntry AnyFilter { get; set; }

        private IList<FilterDataEntry> Filters { get; }

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
            Filters.Add(AnyFilter);
        }

        public void AddFilterAnyFile()
        {
            AddFilterAnyFile(FileSupportResources.OpenSaveFileFilterAnyText);
        }

        public void RemoveFileAnyFile()
        {
            AnyFilter = null;
        }
        
        public FilterDataEntry EntryAt(uint index)
        {
            if (index <= 0)
                throw new ArgumentException("Index starts at 1");
            return Filters.ElementAt((int) index - 1);
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
                newList.Remove(AnyFilter);
                newList.Add(AnyFilter);
                return newList;
            }
            return Filters;
        }

        public static FilterData CreateFromFilter(string filter)
        {
            var fd = new FilterData();
            if (filter != null)
            {
                var strArray = filter.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray.Length % 2 != 0)
                    return fd;
                for (var i = 0; i < strArray.Length; i += 2)
                {
                    var indexBraceOpen = strArray[i].IndexOf('(');
                    var indexBraceClose = strArray[i].IndexOf(')');
                    var text = strArray[i].Remove(indexBraceOpen, indexBraceClose - indexBraceOpen + 1).Trim();

                    var extensions = strArray[i + 1].Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);

                    if (extensions.Length == 1 && extensions[0].Equals("*.*", StringComparison.CurrentCultureIgnoreCase))
                        fd.AddFilterAnyFile(text);
                    else
                    {
                        foreach (var extension in extensions)
                        {
                            var realExtension = extension.Substring(1);
                            fd.AddFilter(new FilterDataEntry(text, realExtension));
                        }
                    }
                }
            }
            return fd;
        }
    }
}