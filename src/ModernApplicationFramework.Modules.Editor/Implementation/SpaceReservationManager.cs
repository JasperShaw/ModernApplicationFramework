using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Adornments;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class SpaceReservationManager : ISpaceReservationManager
    {
        public readonly string Name;
        public readonly int Rank;
        internal IList<ISpaceReservationAgent> _agents = new List<ISpaceReservationAgent>();
        private readonly TextView _view;
        private bool _hasAggregateFocus;

        public event EventHandler<SpaceReservationAgentChangedEventArgs> AgentChanged;

        public event EventHandler GotAggregateFocus;

        public event EventHandler LostAggregateFocus;

        public ReadOnlyCollection<ISpaceReservationAgent> Agents =>
            new ReadOnlyCollection<ISpaceReservationAgent>(_agents);

        public bool HasAggregateFocus
        {
            get
            {
                foreach (var agent in _agents)
                    if (agent.HasFocus)
                        return true;
                return false;
            }
        }

        public bool IsMouseOver
        {
            get
            {
                foreach (var agent in _agents)
                    if (agent.IsMouseOver)
                        return true;
                return false;
            }
        }

        public SpaceReservationManager(string name, int rank, TextView view)
        {
            Name = name;
            Rank = rank;
            _view = view;
            _view.Closed += OnViewClosed;
        }

        public void AddAgent(ISpaceReservationAgent agent)
        {
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));
            _agents.Add(agent);
            ChangeAgents(null, agent);
            CheckFocusChange();
            _view.QueueSpaceReservationStackRefresh();
        }

        public ISpaceReservationAgent CreatePopupAgent(ITrackingSpan visualSpan, PopupStyles styles, UIElement content)
        {
            return new PopupAgent(_view, this, visualSpan, styles, content);
        }

        public bool RemoveAgent(ISpaceReservationAgent agent)
        {
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));
            if (!_agents.Remove(agent))
                return false;
            ChangeAgents(agent, null);
            CheckFocusChange();
            _view.QueueSpaceReservationStackRefresh();
            return true;
        }

        public void UpdatePopupAgent(ISpaceReservationAgent agent, ITrackingSpan visualSpan, PopupStyles styles)
        {
            if (agent == null)
                throw new ArgumentNullException(nameof(agent));
            if (visualSpan == null)
                throw new ArgumentNullException(nameof(visualSpan));
            var popupAgent = agent as PopupAgent;
            if (popupAgent == null)
                throw new ArgumentException("The agent is not a PopupAgent", nameof(agent));
            popupAgent.SetVisualSpan(visualSpan);
            popupAgent.Style = styles;
            CheckFocusChange();
            _view.QueueSpaceReservationStackRefresh();
        }

        internal void ChangeAgents(ISpaceReservationAgent oldAgent, ISpaceReservationAgent newAgent)
        {
            if (oldAgent != null)
            {
                oldAgent.LostFocus -= OnAgentLostFocus;
                oldAgent.GotFocus -= OnAgentGotFocus;
                oldAgent.Hide();
            }

            // ISSUE: reference to a compiler-generated field
            var agentChanged = AgentChanged;
            agentChanged?.Invoke(this, new SpaceReservationAgentChangedEventArgs(oldAgent, newAgent));
            if (newAgent != null)
            {
                newAgent.LostFocus += OnAgentLostFocus;
                newAgent.GotFocus += OnAgentGotFocus;
            }

            _view.QueueSpaceReservationStackRefresh();
        }

        internal void PositionAndDisplay(GeometryGroup reservedGeometry)
        {
            _view.GuardedOperations.CallExtensionPoint(this, () =>
            {
                if (_agents.Count == 0)
                    return;
                if (_view.VisualElement.IsVisible)
                    for (var index = _agents.Count - 1; index >= 0; --index)
                    {
                        var agent = _agents[index];
                        var geometry = agent.PositionAndDisplay(reservedGeometry);
                        if (geometry == null)
                        {
                            _agents.RemoveAt(index);
                            ChangeAgents(agent, null);
                        }
                        else if (!geometry.IsEmpty())
                        {
                            reservedGeometry.Children.Add(geometry);
                        }
                    }
                else
                    for (var index = _agents.Count - 1; index >= 0; --index)
                    {
                        var agent = _agents[index];
                        _agents.RemoveAt(index);
                        ChangeAgents(agent, null);
                    }

                CheckFocusChange();
            });
        }

        private void CheckFocusChange()
        {
            var hasAggregateFocus = HasAggregateFocus;
            if (_hasAggregateFocus == hasAggregateFocus)
                return;
            _hasAggregateFocus = hasAggregateFocus;
            var eventHandler = _hasAggregateFocus ? GotAggregateFocus : LostAggregateFocus;
            eventHandler?.Invoke(this, new EventArgs());
        }

        private void OnAgentGotFocus(object sender, EventArgs e)
        {
            if (_hasAggregateFocus)
                return;
            _hasAggregateFocus = true;
            var gotAggregateFocus = GotAggregateFocus;
            gotAggregateFocus?.Invoke(sender, e);
        }

        private void OnAgentLostFocus(object sender, EventArgs e)
        {
            if (!_hasAggregateFocus)
                return;
            if (_agents.Any(agent => agent.HasFocus)) return;
            _hasAggregateFocus = false;
            var lostAggregateFocus = LostAggregateFocus;
            lostAggregateFocus?.Invoke(sender, e);
        }

        private void OnViewClosed(object sender, EventArgs e)
        {
            var reservationAgentList = new List<ISpaceReservationAgent>();
            reservationAgentList.AddRange(_agents);
            foreach (var agent in reservationAgentList)
                RemoveAgent(agent);
            _view.Closed -= OnViewClosed;
        }
    }
}