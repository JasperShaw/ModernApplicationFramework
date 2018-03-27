using System.ComponentModel;
using ModernApplicationFramework.Modules.Inspector.Inspectors;

namespace ModernApplicationFramework.Modules.Inspector.Conventions
{
    public abstract class PropertyEditorBuilder
    {
        public abstract bool IsApplicable(PropertyDescriptor propertyDescriptor);
        public abstract IInspectorEditor BuildEditor(PropertyDescriptor propertyDescriptor);
    }
}