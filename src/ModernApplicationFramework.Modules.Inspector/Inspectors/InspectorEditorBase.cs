using System;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Modules.Inspector.Inspectors
{
    public abstract class InspectorEditorBase<TValue> : InspectorBase, IInspectorEditor, IDisposable
    {
        private BoundPropertyDescriptor _boundPropertyDescriptor;

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(BoundPropertyDescriptor.PropertyDescriptor.Description))
                    return BoundPropertyDescriptor.PropertyDescriptor.Description;
                return Name;
            }
        }

        public TValue Value
        {
            get => (TValue) BoundPropertyDescriptor.Value;
            set
            {
                var type = IoC.Get<IDockingHostViewModel>().ActiveItem.GetType();
                var prop = type.GetProperty(BoundPropertyDescriptor.PropertyDescriptor.Name);
                if (prop.CanWrite)
                    prop.SetValue(IoC.Get<IDockingHostViewModel>().ActiveItem, value, null);
            }
        }

        public void Dispose()
        {
            if (_boundPropertyDescriptor != null)
                _boundPropertyDescriptor.ValueChanged -= OnValueChanged;
        }

        public override string Name => BoundPropertyDescriptor.PropertyDescriptor.DisplayName;

        public BoundPropertyDescriptor BoundPropertyDescriptor
        {
            get => _boundPropertyDescriptor;
            set
            {
                if (_boundPropertyDescriptor != null)
                    _boundPropertyDescriptor.ValueChanged -= OnValueChanged;
                _boundPropertyDescriptor = value;
                value.ValueChanged += OnValueChanged;
            }
        }

        public override bool IsReadOnly => BoundPropertyDescriptor.PropertyDescriptor.IsReadOnly;

        private void OnValueChanged(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => Value);
        }
    }
}