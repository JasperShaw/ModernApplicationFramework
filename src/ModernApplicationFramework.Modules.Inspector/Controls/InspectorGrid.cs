using System;
using System.Windows;

namespace ModernApplicationFramework.Modules.Inspector.Controls
{
    public class InspectorGrid
    {
        private static GridLength _propertyNameColumnWidth = new GridLength(1, GridUnitType.Star);


        private static GridLength _propertyValueColumnWidth = new GridLength(1.5, GridUnitType.Star);
        public static event EventHandler PropertyNameColumnWidthChanged;
        public static event EventHandler PropertyValueColumnWidthChanged;

        public static GridLength PropertyNameColumnWidth
        {
            get => _propertyNameColumnWidth;
            set
            {
                _propertyNameColumnWidth = value;
                var handler = PropertyNameColumnWidthChanged;
                handler?.Invoke(null, EventArgs.Empty);
            }
        }

        public static GridLength PropertyValueColumnWidth
        {
            get => _propertyValueColumnWidth;
            set
            {
                _propertyValueColumnWidth = value;
                var handler = PropertyValueColumnWidthChanged;
                handler?.Invoke(null, EventArgs.Empty);
            }
        }
    }
}