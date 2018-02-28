using System.ComponentModel;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Conventions
{
    public abstract class PropertyEditorBuilder
    {
        public abstract bool IsApplicable(PropertyDescriptor propertyDescriptor);
        public abstract IInspectorEditor BuildEditor(PropertyDescriptor propertyDescriptor);
    }
}