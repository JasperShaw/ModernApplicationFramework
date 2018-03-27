using System;
using System.ComponentModel;
using ModernApplicationFramework.Modules.Inspector.Inspectors;

namespace ModernApplicationFramework.Modules.Inspector.Conventions
{
    public class EnumPropertyEditorBuilder : PropertyEditorBuilder
    {
        public override bool IsApplicable(PropertyDescriptor propertyDescriptor)
        {
            return typeof(Enum).IsAssignableFrom(propertyDescriptor.PropertyType);
        }

        public override IInspectorEditor BuildEditor(PropertyDescriptor propertyDescriptor)
        {
            return new EnumInspectorEditorViewModel(propertyDescriptor.PropertyType);
        }
    }
}