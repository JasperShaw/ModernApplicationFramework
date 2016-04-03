using System.ComponentModel;

namespace ModernApplicationFramework.Utilities.UndoRedoManager
{
    public class UndoRedoAction
    {
        private readonly object _newValue;

        private readonly object _originalValue;

        private readonly InstancePropertyHandler _propertyHandler;

        public UndoRedoAction(object sender, string propertyName, object newValue, bool hasPropertyChange = true)
        {
            var properties = TypeDescriptor.GetProperties(sender);
            _propertyHandler = new InstancePropertyHandler(sender, properties.Find(propertyName, false));
            _originalValue = _propertyHandler.Value;
            _newValue = newValue;
            HasPropertyChange = hasPropertyChange;
        }


        public bool HasPropertyChange { get; }


        public string Name => $"Change {_propertyHandler.Property.DisplayName} from {_originalValue} to {_newValue}";

        public void Execute()
        {
            _propertyHandler.Value = _newValue;
        }

        public void Undo()
        {
            _propertyHandler.Value = _originalValue;
        }
    }
}