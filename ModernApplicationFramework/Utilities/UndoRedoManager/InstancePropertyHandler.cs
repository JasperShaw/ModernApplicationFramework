using System;
using System.ComponentModel;

namespace ModernApplicationFramework.Utilities.UndoRedoManager
{
    public class InstancePropertyHandler
    {
        public InstancePropertyHandler(object instance, PropertyDescriptor property)
        {
            Instance = instance;
            Property = property;
        }

        public event EventHandler ValueChanged
        {
            add => Property.AddValueChanged(Instance, value);
            remove => Property.RemoveValueChanged(Instance, value);
        }

        public object Instance { get; }
        public PropertyDescriptor Property { get; }

        public object Value
        {
            get => Property.GetValue(Instance);
            set => Property.SetValue(Instance, value);
        }
    }
}