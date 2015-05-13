using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public interface INonClientArea
    {
        int HitTest(Point point);
    }
}
