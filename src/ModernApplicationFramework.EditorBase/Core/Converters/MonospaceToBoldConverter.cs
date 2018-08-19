using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.EditorBase.Core.Converters
{
    internal class MonospaceToBoldConverter : ValueConverter<bool, FontWeight>
    {
        protected override FontWeight Convert(bool isMonospace, object parameter, CultureInfo culture)
        {
            return isMonospace ? FontWeights.Bold : FontWeights.Normal;
        }
    }
}
