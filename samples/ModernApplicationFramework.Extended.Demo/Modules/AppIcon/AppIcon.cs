using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Demo.Modules.AppIcon
{
    [Export(typeof(IApplicationIconService))]
    public class AppIcon : IApplicationIconService
    {
        private bool _isVectorIconInitialized;
        private Geometry _vectorIcon;

        public Geometry VectorIcon
        {
            get
            {
                if (_isVectorIconInitialized)
                    return _vectorIcon;
                _isVectorIconInitialized = true;

                var uri = new Uri("/ModernApplicationFramework.Extended.Demo;component/Resources/Icon.xaml",
                    UriKind.RelativeOrAbsolute);
 
                var myResourceDictionary = new ResourceDictionary { Source = uri };

                object pvar = myResourceDictionary["IconGeometry"];

                try
                {

                    if (pvar is Geometry icon)
                    {
                        _vectorIcon = icon;
                        _vectorIcon.Freeze();
                        return _vectorIcon;
                    }

                    _vectorIcon = Geometry.Parse(pvar.ToString());
                    _vectorIcon.Freeze();
                }
                catch (Exception ex)
                {
                    _vectorIcon = null;
                }
                return _vectorIcon;
            }
        }

        public Brush ActiveColor => Brushes.Red;
        public Brush InactiveColor => Brushes.Blue;
    }
}
