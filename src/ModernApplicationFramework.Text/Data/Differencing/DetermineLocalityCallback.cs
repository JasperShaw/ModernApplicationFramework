using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Text.Data.Differencing
{
    [Obsolete("Methods that use this callback are now deprecated, and instances of this callback will not be used.")]
    public delegate int? DetermineLocalityCallback(StringDifferenceTypes differenceType, IList<string> leftStrings,
        IList<string> rightStrings);
}