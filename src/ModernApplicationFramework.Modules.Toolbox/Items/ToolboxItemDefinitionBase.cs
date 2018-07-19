using System;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract toolbox item definition class
    /// </summary>
    /// <remarks>
    /// Requires <see cref="T:System.SerializableAttribute" />
    /// </remarks>
    /// <seealso cref="!:System.IEquatable{ModernApplicationFramework.Modules.Toolbox.Items.ToolboxItemDefinitionBase}" />
    [Serializable]
    public abstract class ToolboxItemDefinitionBase : IEquatable<ToolboxItemDefinitionBase>
    {
        private bool _enabled;

        /// <summary>
        /// Occurs when the enabled state was changed.
        /// </summary>
        public event EventHandler EnabledChanged;

        /// <summary>
        /// The compatible types of the definition.
        /// </summary>
        public abstract TypeArray<ILayoutItem> CompatibleTypes { get; }

        /// <summary>
        /// The data of the attribute
        /// </summary>
        public abstract ToolboxItemData Data { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ToolboxItemDefinitionBase"/> is enabled.
        /// </summary>
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

        /// <summary>
        /// The ID.
        /// </summary>
        public abstract Guid Id { get; }

        /// <summary>
        /// The image moniker.
        /// </summary>
        public virtual ImageMoniker ImageMoniker => default;

        /// <summary>
        /// The localized name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Value indicating whether this <see cref="ToolboxItemDefinitionBase" /> is serializable by an <see cref="IToolboxStateSerializer"/>.
        /// </summary>
        public virtual bool Serializable => true;

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

        protected virtual void OnEnabledChanged()
        {
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}