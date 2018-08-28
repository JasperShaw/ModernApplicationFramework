using System.ComponentModel;
using System.Globalization;
using ModernApplicationFramework.Basics.CommandBar.ItemDefinitions;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.CommandBar.DataSources
{
    /// <inheritdoc cref="CommandBarDataSource" />
    /// <summary>
    /// Fundamental command bar item definition
    /// </summary>
    /// <seealso cref="T:ModernApplicationFramework.Basics.Definitions.CommandBar.CommandBarDefinitionBase" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.IHasInternalName" />
    public abstract class CommandBarItemDataSource : CommandBarDataSource, ISortable
    {
        private bool _precededBySeparator;
        private CommandBarGroup _group;
        private bool _isVeryFirst;
        private string _text;
        private uint _sortOrder;
        private bool _acquireFocus;

        /// <summary>
        /// Indicates whether this preceded by a separator item
        /// </summary>
        internal virtual bool PrecededBySeparator
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
        /// Indicates whether this definition can be modified
        /// </summary>
        public virtual bool IsCustomizable { get; }

        public virtual bool AcquireFocus
        {
            get => _acquireFocus;
            set
            {
                if (value == _acquireFocus)
                    return;
                _acquireFocus = value;
                OnPropertyChanged();
            }
        }

        internal CommandDefinition InternalCommandDefinition { get; set; }

        public virtual CommandBarItemDefinition ItemDefinition { get; }

        /// <summary>
        /// Indicates whether this item is first of any other in this sub-tree
        /// </summary>
        internal virtual bool IsVeryFirst
        {
            get => _isVeryFirst;
            set
            {
                if (value == _isVeryFirst) return;
                _isVeryFirst = value;
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
                if (value == _text)
                    return;
                _text = value;
                IsTextModified = true;
                OnPropertyChanged();
                UpdateInternalName();
                UpdateName();
            }
        }

        /// <summary>
        /// The sorting order of the definition
        /// </summary>
        public uint SortOrder
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
        public CommandBarGroup Group
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

        protected CommandBarItemDataSource(string text, uint sortOrder, CommandBarGroup group,
            CommandBarItemDefinition definition, bool isCustom, CommandBarFlags flags = CommandBarFlags.CommandFlagNone)
            : base(text, isCustom, flags)
        {
            _group = group;
            _sortOrder = sortOrder;
            _text = text ?? definition?.Text;

            IsCustomizable = !flags.HasFlag(CommandBarFlags.CommandNoCustomize);

            var internalName = new AccessKeyRemovingConverter()
                .Convert(text, typeof(string), null, CultureInfo.CurrentCulture)
                ?.ToString();

            if (group?.Parent is IHasInternalName internalNameParent && internalNameParent.InheritInternalName)
            {
                if (string.IsNullOrEmpty(internalNameParent.InternalName))
                    return;
                InternalName = internalNameParent.InternalName + " | " + internalName;
                internalNameParent.PropertyChanged += InternalNameParent_PropertyChanged;
            }
            else
                InternalName = internalName;

            ItemDefinition = definition;
            if (definition != null)
                definition.PropertyChanged += Definition_PropertyChanged;
        }

        public override void Reset()
        {
            IsTextModified = false;
            _text = OriginalText;
            UpdateInternalName();
            UpdateName();
            OnPropertyChanged(nameof(Text));
            Flags.EnableStyleFlags(OriginalFlagStore.AllFlags);
        }

        /// <summary>
        /// Updates the group.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldGroup">The old group.</param>
        protected void UpdateGroup(CommandBarGroup value, CommandBarGroup oldGroup)
        {
            if (UiType == CommandControlTypes.Separator)
                return;
            if (!value.Items.Contains(this))
                value.Items.AddSorted(this, new SortOrderComparer<CommandBarDataSource>());
            oldGroup?.Items.Remove(this);
        }

        /// <summary>
        /// Re-sort group items.
        /// </summary>
        protected void ReSortGroupItems()
        {
            Group.Items.Sort(new SortOrderComparer<CommandBarDataSource>());
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

        private void Definition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemDefinition.Text))
                UpdateText();
        }

        protected virtual void UpdateText()
        {
            Text = ItemDefinition?.Text;
        }
    }
}