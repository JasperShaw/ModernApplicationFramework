using System.Collections.Generic;
using ModernApplicationFramework.Modules.Inspector.Inspectors;

namespace ModernApplicationFramework.Modules.Inspector
{
    public interface IInspectableObject
    {
        IEnumerable<IInspector> Inspectors { get; }
    }
}