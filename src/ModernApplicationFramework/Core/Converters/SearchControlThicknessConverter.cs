using System.Globalization;
using System.Windows;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters
{
    public class SearchControlThicknessConverter : ValueConverter<string, Thickness>
    {
        protected override Thickness Convert(string borderThickness, object parameter, CultureInfo culture)
        {
            Thickness thickness = (Thickness)new ThicknessConverter().ConvertFromString(borderThickness);
            switch ((BorderType)parameter)
            {
                case BorderType.Left:
                    thickness.Right = thickness.Top = thickness.Bottom = 0.0;
                    break;
                case BorderType.Top:
                    thickness.Left = thickness.Right = thickness.Bottom = 0.0;
                    break;
                case BorderType.Right:
                    thickness.Left = thickness.Top = thickness.Bottom = 0.0;
                    break;
                case BorderType.Bottom:
                    thickness.Left = thickness.Right = thickness.Top = 0.0;
                    break;
                case BorderType.LeftRight:
                    thickness.Top = thickness.Bottom = 0.0;
                    break;
                case BorderType.TopBottom:
                    thickness.Left = thickness.Right = 0.0;
                    break;
                case BorderType.LeftTopBottom:
                    thickness.Right = 0.0;
                    break;
                case BorderType.TopRightBottom:
                    thickness.Left = 0.0;
                    break;
                case BorderType.LeftTopRight:
                    thickness.Bottom = 0.0;
                    break;
                case BorderType.LeftRightBottom:
                    thickness.Top = 0.0;
                    break;
            }
            return thickness;
        }
    }

    internal enum BorderType
    {
        Left,
        Top,
        Right,
        Bottom,
        LeftRight,
        TopBottom,
        LeftTopBottom,
        TopRightBottom,
        LeftTopRight,
        LeftRightBottom,
        LeftTopRightBottom,
    }
}
