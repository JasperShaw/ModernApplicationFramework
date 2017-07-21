using System.ComponentModel;
using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    /// <inheritdoc cref="CommandBarDefinitionBase" />
    /// <summary>
    /// Fundamental command bar item definition
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    public abstract class CommandBarItemDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private bool _isVisible;
        private bool _precededBySeparator;
        private CommandBarGroupDefinition _group;
        private string _internalName;
        private bool _isVeryFirst;
        private string _text;
        private uint _sortOrder;

        /// <summary>
        /// Indicates whether this item is visible
        /// </summary>
        public virtual bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates whether this preceded by a separator item
        /// </summary>
        public virtual bool PrecededBySeparator
        {
            get => _precededBySeparator;
            set
            {
                if (value == _precededBySeparator)
                    return;
                _precededBySeparator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indicates whether this item is first of any other in this sub-tree
        /// </summary>
        public virtual bool IsVeryFirst
        {
            get => _isVeryFirst;
            set
            {
                if (value == _isVeryFirst) return;
                _isVeryFirst = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The unlocalized internal name of the object
        /// </summary>
        public virtual string InternalName
        {
            get => _internalName;
            set
            {
                if (value == _internalName) return;
                _internalName = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The localized definition's text
        /// </summary>
        /// <inheritdoc />
        public override string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
                UpdateInternalName();
                UpdateName();
            }
        }

        /// <summary>
        /// The sorting order of the definition
        /// </summary>
        public override uint SortOrder
        {
            get => _sortOrder;
            set
            {
                if (value == _sortOrder) return;
                _sortOrder = value;
                OnPropertyChanged();
                ReSortGroupItems();
            }
        }

        /// <summary>
        /// The current group of the item
        /// </summary>
        public CommandBarGroupDefinition Group
        {
            get => _group;
            set
            {
                if (Equals(value, _group))
                    return;
                if (_group != null && _group.Parent is IHasInternalName oldinternalNameParent)
                    oldinternalNameParent.PropertyChanged -= InternalNameParent_PropertyChanged;
                var oldGroup = _group;
                _group = value;
                OnPropertyChanged();
                if (value?.Parent is IHasInternalName newinternalNameParent)
                    newinternalNameParent.PropertyChanged += InternalNameParent_PropertyChanged;
                UpdateInternalName();
                UpdateGroup(value, oldGroup);
            }
        }

        protected CommandBarItemDefinition(string text, uint sortOrder, CommandBarGroupDefinition group,
            DefinitionBase definition, bool visible,
            bool isChecked, bool isCustom, bool isCustomizable)
            : base(text, sortOrder, definition, isCustom, isCustomizable, isChecked)
        {
            _isVisible = visible;
            _group = group;
            _sortOrder = sortOrder;
            _text = text;

            var internalName = new AccessKeyRemovingConverter()
                .Convert(text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();

            if (group?.Parent is IHasInternalName internalNameParent)
            {
                if (string.IsNullOrEmpty(internalNameParent.InternalName))
                    return;
                _internalName = internalNameParent.InternalName + " | " + internalName;
                internalNameParent.PropertyChanged += InternalNameParent_PropertyChanged;
            }
            else
            {
                _internalName = internalName;
            }
        }

        /// <summary>
        /// Updates the group.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldGroup">The old group.</param>
        protected void UpdateGroup(CommandBarGroupDefinition value, CommandBarGroupDefinition oldGroup)
        {
            if (CommandDefinition.ControlType == CommandControlTypes.Separator)
                return;
            if (!value.Items.Contains(this))
                value.Items.AddSorted(this, new SortOrderComparer<CommandBarDefinitionBase>());
            oldGroup?.Items.Remove(this);
        }

        /// <summary>
        /// Re-sort group items.
        /// </summary>
        protected void ReSortGroupItems()
        {
            Group.Items.Sort(new SortOrderComparer<CommandBarDefinitionBase>());
        }

        /// <summary>
        /// Updates the internal name of the item
        /// </summary>
        protected void UpdateInternalName()
        {
            var internalName = new AccessKeyRemovingConverter()
                .Convert(Text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();

            if (Group?.Parent is IHasInternalName internalNameParent)
            {
                if (!string.IsNullOrEmpty(internalNameParent.InternalName))
                    InternalName = internalNameParent.InternalName + " | " + internalName;
            }
            else
            {
                InternalName = internalName;
            }
        }

	    private void UpdateName()
	    {
			var name = new AccessKeyRemovingConverter()
				.Convert(Text, typeof(string), null, CultureInfo.CurrentCulture)
				?.ToString();
		    Name = name;
	    }

		private void InternalNameParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IHasInternalName.InternalName))
                UpdateInternalName();
        }
    }
}