using System.ComponentModel;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Conventions
{
    public class StandardPropertyEditorBuilder<T, TEditor> : PropertyEditorBuilder
        where TEditor : IInspectorEditor, new()
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyType == typeof(T);
        }

        public override IInspectorEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            return new TEditor();
        }
    }
}