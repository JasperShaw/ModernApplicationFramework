using System.ComponentModel;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls.Utilities
{
    public static class ContextMenuGlyphItemUtilities
    {
        public static void SetCheckMark(ContextMenuGlyphItem item)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geomitry = (Geometry) converter.ConvertFrom("F1 M 5,11 L 3,7 L 5,7 L 6,9 L 9,3 L 11,3 L 7,11 L 5,11 Z");

            item.IconGeometry = geomitry;
        }
    }
}