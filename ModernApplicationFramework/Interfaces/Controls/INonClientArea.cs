using System.Windows;

namespace ModernApplicationFramework.Interfaces.Controls
{
    internal interface INonClientArea
    {
        int HitTest(Point point);
    }
}