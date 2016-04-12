using System.Collections.Generic;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool
{
    public class InspectableObject : IInspectableObject
    {
        public IEnumerable<IInspector> Inspectors { get; set; }

        public InspectableObject(IEnumerable<IInspector> inspectors)
        {
            Inspectors = inspectors;
        }
    }
}
