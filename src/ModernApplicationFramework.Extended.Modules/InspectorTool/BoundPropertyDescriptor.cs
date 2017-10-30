using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool
{
    public class BoundPropertyDescriptor
    {
        public event EventHandler ValueChanged
        {
            add => PropertyDescriptor.AddValueChanged(PropertyOwner, value);
            remove => PropertyDescriptor.RemoveValueChanged(PropertyOwner, value);
        }

        public PropertyDescriptor PropertyDescriptor { get; }
        public object PropertyOwner { get; }

        public object Value
        {
            get => PropertyDescriptor.GetValue(PropertyOwner);
            set => PropertyDescriptor.SetValue(PropertyOwner, value);
        }

        public BoundPropertyDescriptor(object propertyOwner, PropertyDescriptor propertyDescriptor)
        {
            PropertyOwner = propertyOwner;
            PropertyDescriptor = propertyDescriptor;
        }

        public static BoundPropertyDescriptor FromProperty(object propertyOwner, string propertyName)
        {
            // TODO: Cache all this.
            var properties = TypeDescriptor.GetProperties(propertyOwner);
            return new BoundPropertyDescriptor(propertyOwner, properties.Find(propertyName, false));
        }
    }
}