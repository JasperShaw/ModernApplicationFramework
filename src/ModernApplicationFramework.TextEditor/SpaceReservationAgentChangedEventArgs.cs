using System;

namespace ModernApplicationFramework.TextEditor
{
    public class SpaceReservationAgentChangedEventArgs : EventArgs
    {
        public SpaceReservationAgentChangedEventArgs(ISpaceReservationAgent oldAgent, ISpaceReservationAgent newAgent)
        {
            OldAgent = oldAgent;
            NewAgent = newAgent;
        }

        public ISpaceReservationAgent OldAgent { get; }

        public ISpaceReservationAgent NewAgent { get; }
    }
}