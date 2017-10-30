using System.ComponentModel;

namespace ModernApplicationFramework.Basics.UndoRedoManager
{
    /// <summary>
    /// An <see cref="UndoRedoAction"/> holds information to undo or redo a property change
    /// </summary>
    public class UndoRedoAction
    {
        private readonly object _newValue;
        private readonly object _originalValue;
        private readonly InstancePropertyHandler _propertyHandler;

        public UndoRedoAction(object sender, string propertyName, object newValue)
        {
            var properties = TypeDescriptor.GetProperties(sender);
            _propertyHandler = new InstancePropertyHandler(sender, properties.Find(propertyName, false));
            _originalValue = _propertyHandler.Value;
            _newValue = newValue;
        }

        //Todo: Localize
        /// <summary>
        /// The localized description what this action does
        /// </summary>
        public string Description => $"Change {_propertyHandler.Property.DisplayName} from {_originalValue} to {_newValue}";

        /// <summary>
        /// Executes the redo action.
        /// </summary>
        public void Execute()
        {
            _propertyHandler.Value = _newValue;
        }

        /// <summary>
        /// Executes the undo action.
        /// </summary>
        public void Undo()
        {
            _propertyHandler.Value = _originalValue;
        }
    }
}