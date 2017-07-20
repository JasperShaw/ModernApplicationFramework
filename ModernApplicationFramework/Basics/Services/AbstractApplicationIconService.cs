using System.ComponentModel.Composition;
using System.Windows.Media;
using Caliburn.Micro;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(IApplicationIconService))]
    internal class InternalApplicationIconService : IApplicationIconService
    {
        private static IApplicationIconService _instance;

        public static IApplicationIconService Instance => _instance ??
                                                          (_instance = IoC.Get<IApplicationIconService>());

        public Geometry VectorIcon => null;
        public Brush ActiveColor => Brushes.Transparent;
        public Brush InactiveColor => Brushes.Transparent;
    }
}