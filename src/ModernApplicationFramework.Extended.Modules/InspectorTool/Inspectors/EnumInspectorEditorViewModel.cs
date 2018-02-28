using System;
using System.Collections.Generic;
using System.Linq;

namespace ModernApplicationFramework.Extended.Modules.InspectorTool.Inspectors
{
    public class EnumValueViewModel<TEnum>
    {
        public TEnum Value { get; set; }
        public string Text { get; set; }
    }

    public class EnumInspectorEditorViewModel<TEnum> : InspectorEditorBase<TEnum>, ILabelledInspector
    {
        private readonly List<EnumValueViewModel<TEnum>> _items;
        public IEnumerable<EnumValueViewModel<TEnum>> Items => _items;

        public EnumInspectorEditorViewModel()
        {
            _items = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(x => new EnumValueViewModel<TEnum>
                {
                    Value = x,
                    Text = Enum.GetName(typeof(TEnum), x)
                })
                .ToList();
        }
    }

    public class EnumValueViewModel
    {
        public object Value { get; set; }
        public string Text { get; set; }
    }

    public class EnumInspectorEditorViewModel : InspectorEditorBase<Enum>, ILabelledInspector
    {
        private readonly List<EnumValueViewModel> _items;
        public IEnumerable<EnumValueViewModel> Items => _items;

        public EnumInspectorEditorViewModel(Type enumType)
        {
            _items = Enum.GetValues(enumType)
                .Cast<object>()
                .Select(x => new EnumValueViewModel
                {
                    Value = x,
                    Text = Enum.GetName(enumType, x)
                })
                .ToList();
        }
    }
}