using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernApplicationFramework.TextEditor
{
    public interface ISpaceReservationManager
    {
        ISpaceReservationAgent CreatePopupAgent(ITrackingSpan visualSpan, PopupStyles style, UIElement content);

        void UpdatePopupAgent(ISpaceReservationAgent agent, ITrackingSpan visualSpan, PopupStyles styles);

        void AddAgent(ISpaceReservationAgent agent);

        bool RemoveAgent(ISpaceReservationAgent agent);

        ReadOnlyCollection<ISpaceReservationAgent> Agents { get; }

        event EventHandler<SpaceReservationAgentChangedEventArgs> AgentChanged;

        bool IsMouseOver { get; }

        bool HasAggregateFocus { get; }

        event EventHandler LostAggregateFocus;

        event EventHandler GotAggregateFocus;
    }
}