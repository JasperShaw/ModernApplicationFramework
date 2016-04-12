using System.ComponentModel;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Conventions
{
    public abstract class PropertyEditorBuilder
    {
        public abstract bool IsApplicable(PropertyDescriptor propertyDescriptor);
        public abstract IEditor BuildEditor(PropertyDescriptor propertyDescriptor);
    }
}
