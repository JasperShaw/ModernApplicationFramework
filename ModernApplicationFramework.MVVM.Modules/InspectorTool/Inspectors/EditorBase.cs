using System;
using System.Reflection;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Modules.InspectorTool.Inspectors
{
    public abstract class EditorBase<TValue> : InspectorBase, IEditor, IDisposable
    {
        private BoundPropertyDescriptor _boundPropertyDescriptor;

        public override string Name => BoundPropertyDescriptor.PropertyDescriptor.DisplayName;

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(BoundPropertyDescriptor.PropertyDescriptor.Description))
                    return BoundPropertyDescriptor.PropertyDescriptor.Description;
                return Name;
            }
        }

        public BoundPropertyDescriptor BoundPropertyDescriptor
        {
            get { return _boundPropertyDescriptor; }
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

        public TValue Value
        {
            get { return (TValue)BoundPropertyDescriptor.Value; }
            set
            {
                Type type = IoC.Get<IDockingHostViewModel>().ActiveItem.GetType();
                PropertyInfo prop = type.GetProperty(BoundPropertyDescriptor.PropertyDescriptor.Name);
                if (prop.CanWrite)
                    prop.SetValue(IoC.Get<IDockingHostViewModel>().ActiveItem, value, null);
            }
        }

        public void Dispose()
        {
            if (_boundPropertyDescriptor != null)
                _boundPropertyDescriptor.ValueChanged -= OnValueChanged;
        }
    }
}
