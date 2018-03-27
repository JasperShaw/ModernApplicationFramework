using System.Collections.Generic;
using ModernApplicationFramework.Modules.Inspector.Inspectors;

namespace ModernApplicationFramework.Modules.Inspector
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