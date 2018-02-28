﻿using System;
using System.ComponentModel;
using ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Conventions
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