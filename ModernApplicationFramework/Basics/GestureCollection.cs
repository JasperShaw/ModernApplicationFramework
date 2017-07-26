using System;
using System.Collections.Generic;
using System.Windows.Input;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics
{
    public class GestureCollection : BaseDictionary<CommandGestureCategory, KeyGesture>
    {

        private Dictionary<CommandGestureCategory, KeyGesture> _innerDictionary;

        public delegate void GestursChangedEventHandler(object sender, GestureChangedEventArgs e);

        public event GestursChangedEventHandler GestursChanged;

        public override int Count => _innerDictionary?.Count ?? 0;

        public override void Clear()
        {
            if (_innerDictionary == null)
                return;
            _innerDictionary.Clear();
            _innerDictionary = null;
        }

        public override void Add(CommandGestureCategory key, KeyGesture value)
        {
            if (_innerDictionary == null)
                _innerDictionary = new Dictionary<CommandGestureCategory, KeyGesture>(1);
            _innerDictionary.Add(key, value);
        }

        public override bool Remove(CommandGestureCategory key)
        {
            if (_innerDictionary == null)
                return false;
            return _innerDictionary.Remove(key);
        }

        public override bool TryGetValue(CommandGestureCategory key, out KeyGesture value)
        {
            value = default(KeyGesture);
            return _innerDictionary != null && _innerDictionary.TryGetValue(key, out value);
        }

        public override IEnumerator<KeyValuePair<CommandGestureCategory, KeyGesture>> GetEnumerator()
        {
            if (_innerDictionary != null)
                return _innerDictionary.GetEnumerator();
            return new Dictionary<CommandGestureCategory, KeyGesture>(0).GetEnumerator();
        }

        protected override void SetValue(CommandGestureCategory key, KeyGesture value)
        {      
            if(_innerDictionary == null || !_innerDictionary.ContainsKey(key))
                return;
            _innerDictionary[key] = value;
        }

        public override bool ContainsKey(CommandGestureCategory key)
        {
            if (_innerDictionary == null)
                return false;
            return _innerDictionary.ContainsKey(key);
        }

        protected void OnGestureAdded(GestureChangedEventArgs e)
        {
            GestursChanged?.Invoke(this, e);
        }

        protected void OnGestureRemoved(GestureChangedEventArgs e)
        {
            GestursChanged?.Invoke(this, e);
        }
    }

    public class GestureChangedEventArgs : EventArgs
    {
        public GestureChangedEventArgs(GestureChangedType type)
        {
            Type = type;
        }

        public GestureChangedType Type { get; }
    }

    public enum GestureChangedType
    {
        Added,
        Removed,
        ValueChanged
    }
}
