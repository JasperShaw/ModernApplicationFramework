using System.Windows.Media;

namespace ModernApplicationFramework.Basics.Services
{
    public interface IApplicationIconService
    {
        Geometry VectorIcon { get; }

        Brush ActiveColor { get; }
        Brush InactiveColor { get; }
    }
}