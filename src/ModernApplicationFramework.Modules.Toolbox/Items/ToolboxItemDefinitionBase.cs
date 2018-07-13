using System;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    [Serializable]
    public abstract class ToolboxItemDefinitionBase : IEquatable<ToolboxItemDefinitionBase>
    {
        private bool _enabled;
        public event EventHandler EnabledChanged;

        public abstract Guid Id { get; }

        public abstract string Name { get; }

        public virtual ImageMoniker ImageMoniker => default;

        public virtual bool Serializable => true;

        public abstract TypeArray<ILayoutItem> CompatibleTypes { get; }

        public abstract ToolboxItemData Data { get; }

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                    return;
                _enabled = value;
                OnEnabledChanged();
            }
        }

        protected virtual void OnEnabledChanged()
        {
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool Equals(ToolboxItemDefinitionBase other)
        {
            if (other == null)
                return false;
            if (!Id.Equals(other.Id))
                return false;
            if (Id != Guid.Empty)
                return true;
            return Name.Equals(other.Name, StringComparison.CurrentCulture);
        }
    }
}
