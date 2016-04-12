using System.ComponentModel;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Conventions
{
    public class StandardPropertyEditorBuilder<T, TEditor> : PropertyEditorBuilder
        where TEditor : IEditor, new()
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyType == typeof(T);
        }

        public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            return new TEditor();
        }
    }
}
