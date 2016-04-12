using System.Collections.Generic;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool
{
    public interface IInspectableObject
    {
        IEnumerable<IInspector> Inspectors { get; }
    }
}
