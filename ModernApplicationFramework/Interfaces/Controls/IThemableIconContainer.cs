using System.Windows;

namespace ModernApplicationFramework.Interfaces.Controls
{
    public interface IThemableIconContainer
    {
        object IconSource { get; }

        object Icon { get; set; }
    }
}