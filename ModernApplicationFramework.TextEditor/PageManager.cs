using System;
using System.Collections.Generic;
using System.Threading;

namespace ModernApplicationFramework.TextEditor
{
    internal class PageManager
    {
        private Tuple<Page, List<Tuple<Page, char[]>>> _mru;
        private readonly int _maxPages;

        public PageManager()
        {
            _maxPages = TextModelOptions.CompressedStorageMaxLoadedPages;
            _mru = Tuple.Create((Page)null, new List<Tuple<Page, char[]>>(_maxPages));
        }

        public void UpdateMRU(Page page, char[] contents)
        {
            Tuple<Page, List<Tuple<Page, char[]>>> tuple;
            for (var comparand = Volatile.Read(ref _mru); comparand.Item1 != page; comparand = tuple)
            {
                var index = comparand.Item2.Count - 1;
                do
                {
                }
                while (--index >= 0 && comparand.Item2[index].Item1 != page);
                var tupleList = new List<Tuple<Page, char[]>>(_maxPages);
                tupleList.AddRange(comparand.Item2);
                if (index >= 0)
                    tupleList.RemoveAt(index);
                else if (tupleList.Count >= _maxPages)
                    tupleList.RemoveAt(0);
                tupleList.Add(Tuple.Create(page, contents));
                tuple = Interlocked.CompareExchange(ref _mru, Tuple.Create(page, tupleList), comparand);
                if (tuple == comparand)
                    break;
            }
        }
    }
}