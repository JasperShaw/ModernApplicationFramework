using System.Collections.Generic;
using System.ComponentModel;

namespace ModernApplicationFramework.Utilities.Interfaces
{
    public interface IOrderable
    {
        string Name { get; }

        [DefaultValue(null)]
        IEnumerable<string> Before { get; }

        [DefaultValue(null)]
        IEnumerable<string> After { get; }
    }
}
