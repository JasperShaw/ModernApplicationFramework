using System.ComponentModel;
using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Core.Comparers;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarItemDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private bool _isVisible;
        private bool _precededBySeparator;
        private CommandBarGroupDefinition _group;
        private string _internalName;
        private bool _isVeryFirst;
        private string _text;
        private uint _sortOrder;

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

        public override string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
                UpdateInternalName();
            }
        }

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

        protected void UpdateGroup(CommandBarGroupDefinition value, CommandBarGroupDefinition oldGroup)
        {
            if (CommandDefinition.ControlType == CommandControlTypes.Separator)
                return;
            if (!value.Items.Contains(this))
                value.Items.AddSorted(this, new SortOrderComparer<CommandBarDefinitionBase>());
            oldGroup?.Items.Remove(this);
        }

        protected void ReSortGroupItems()
        {
            Group.Items.Sort(new SortOrderComparer<CommandBarDefinitionBase>());
        }

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

        private void InternalNameParent_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IHasInternalName.InternalName))
                UpdateInternalName();
        }
    }
}