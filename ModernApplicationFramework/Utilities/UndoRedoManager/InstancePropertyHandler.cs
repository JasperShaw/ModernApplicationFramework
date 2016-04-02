using System.ComponentModel;

namespace ModernApplicationFramework.Utilities.UndoRedoManager
{
    public class InstancePropertyHandler
    {
        public object Instance { get; }
        public PropertyDescriptor Property { get; }

        public object Value
        {
            get { return Property.GetValue(Instance); }
            set { Property.SetValue(Instance, value); }
        }

        public InstancePropertyHandler(object instance, PropertyDescriptor property)
        {
            Instance = instance;
            Property = property;
        }
    }
}
