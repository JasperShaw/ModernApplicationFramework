using System;
using System.ComponentModel;
using ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Conventions
{
    public class EnumPropertyEditorBuilder : PropertyEditorBuilder
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            return typeof(Enum).IsAssignableFrom(propertyDescriptor.PropertyType);
        }

        public override IEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            return new EnumEditorViewModel(propertyDescriptor.PropertyType);
        }
    }
}
