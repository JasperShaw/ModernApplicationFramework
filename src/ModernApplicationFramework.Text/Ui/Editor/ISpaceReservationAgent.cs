using System;
using System.Windows.Media;

namespace ModernApplicationFramework.Text.Ui.Editor
{
    public interface ISpaceReservationAgent
    {
        Geometry PositionAndDisplay(Geometry reservedSpace);

        void Hide();

        bool IsMouseOver { get; }

        bool HasFocus { get; }

        event EventHandler LostFocus;

        event EventHandler GotFocus;
    }
}