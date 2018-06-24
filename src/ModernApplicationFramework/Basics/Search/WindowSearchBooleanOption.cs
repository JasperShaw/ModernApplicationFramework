using System;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.Search
{
    public class WindowSearchBooleanOption : WindowSearchOption, IWindowSearchBooleanOption
    {
        private readonly Func<bool> _getter;
        private readonly Action<bool> _setter;

        public virtual bool Value
        {
            get => _getter();
            set => _setter(value);
        }

        public WindowSearchBooleanOption(string displayText, string tooltip, Func<bool> getter, Action<bool> setter) : base(displayText, tooltip)
        {
            Validate.IsNotNull(getter, nameof(getter));
            Validate.IsNotNull(setter, nameof(setter));
            _getter = getter;
            _setter = setter;
        }

        public WindowSearchBooleanOption(string displayText, string tooltip, bool initialValue)
            : this(displayText, tooltip, () => initialValue, setterValue => initialValue = setterValue)
        {
        }
    }
}