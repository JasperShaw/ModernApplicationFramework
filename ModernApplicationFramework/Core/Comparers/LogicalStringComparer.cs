﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Core.Comparers
{
    class LogicalStringComparer : IComparer<String>
    {

        private readonly int _order = 1;

        public LogicalStringComparer() : this(ListSortDirection.Ascending)
        {
            
        }

        public LogicalStringComparer(ListSortDirection direction)
        {
            if (direction == ListSortDirection.Descending)
                _order = -1;
        }

        public int Compare(string x, string y)
        {
            var emptyX = string.IsNullOrWhiteSpace(x);
            var emptyY = string.IsNullOrWhiteSpace(y);

            if (emptyY && emptyX)
                return 0;
            if (emptyX)
                return 1*_order;
            if (emptyY)
                return -1*_order;
            return NativeMethods.NativeMethods.StrCmpLogicalW(x, y)*_order;
        }
    }
}