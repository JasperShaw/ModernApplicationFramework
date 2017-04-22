using System;
using System.Globalization;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters
{
    public sealed class SplitButtonItemHighlightConverter : ToBooleanValueConverter<int>
    {
        private readonly SplitButton _itemOwningButton;

        public SplitButtonItemHighlightConverter(SplitButton itemOwningButton)
        {
            _itemOwningButton = itemOwningButton ?? throw new ArgumentNullException(nameof(itemOwningButton));
        }

        protected override bool Convert(int value, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));
            var item = parameter as SplitButtonItem;
            if (item == null)
                throw new ArgumentException();
            return value >= _itemOwningButton.ItemContainerGenerator.IndexFromContainer(item);
        }
    }
}
