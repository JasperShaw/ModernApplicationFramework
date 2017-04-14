using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Core.Converters.AccessKey;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarItemDefinition : CommandBarDefinitionBase, IHasInternalName
    {
        private bool _isVisible;
        private bool _precededBySeparator;
        private CommandBarGroupDefinition _group;
        private string _internalName;

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

        public CommandBarGroupDefinition Group
        {
            get => _group;
            set
            {
                if (Equals(value, _group)) return;
                _group = value;
                OnPropertyChanged();
            }
        }

        protected CommandBarItemDefinition(string text, uint sortOrder, CommandBarGroupDefinition group,
            DefinitionBase definition, bool visible,
            bool isChecked, bool isCustom, bool isCustomizable)
            : base(text, sortOrder, definition, isCustom, isCustomizable, isChecked)
        {
            _isVisible = visible;
            _group = group;

            var internalName = new AccessKeyRemovingConverter().Convert(text, typeof(string), null, CultureInfo.CurrentCulture)?.ToString();

            if (group?.Parent is IHasInternalName internalNameParent)
            {
                if (!string.IsNullOrEmpty(internalNameParent.InternalName))
                    _internalName = internalNameParent.InternalName + " | " + internalName;
            }
            else
            {
                _internalName = internalName;
            }
        }
    }
}