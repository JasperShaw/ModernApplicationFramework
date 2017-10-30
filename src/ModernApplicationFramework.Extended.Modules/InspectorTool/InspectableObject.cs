using System.Collections.Generic;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public class InspectableObject : IInspectableObject
    {
        public InspectableObject(IEnumerable<IInspector> inspectors)
        {
            Inspectors = inspectors;
        }

        public IEnumerable<IInspector> Inspectors { get; set; }
    }
}