using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ModernApplicationFramework.TextEditor
{
    internal class SpaceReservationStack
    {
        internal readonly List<SpaceReservationManager> Managers = new List<SpaceReservationManager>();
        internal Dictionary<string, int> OrderedManagerDefinitions;
        internal readonly TextView View;
        private bool _hasAggregateFocus;

        private void OnManagerLostFocus(object sender, EventArgs e)
        {
            if (!_hasAggregateFocus)
                return;
            foreach (var manager in Managers)
            {
                if (manager.HasAggregateFocus)
                    return;
            }
            _hasAggregateFocus = false;
            View.QueueAggregateFocusCheck();
        }

        private void OnManagerGotFocus(object sender, EventArgs e)
        {
            _hasAggregateFocus = true;
            View.QueueAggregateFocusCheck();
        }

        public SpaceReservationStack(Dictionary<string, int> orderedManagerDefinitions, TextView view)
        {
            OrderedManagerDefinitions = orderedManagerDefinitions;
            View = view;
        }

        public ISpaceReservationManager GetOrCreateManager(string name)
        {
            foreach (var manager in Managers)
            {
                if (manager.Name == name)
                    return manager;
            }

            if (!OrderedManagerDefinitions.TryGetValue(name, out var rank))
                return null;
            var reservationManager = new SpaceReservationManager(name, rank, View);
            var index = 0;
            while (index < Managers.Count && Managers[index].Rank <= rank)
                ++index;
            Managers.Insert(index, reservationManager);
            reservationManager.LostAggregateFocus += OnManagerLostFocus;
            reservationManager.GotAggregateFocus += OnManagerGotFocus;
            return reservationManager;
        }

        public void Refresh()
        {
            var reservedGeometry = new GeometryGroup();
            foreach (var reservationManager in new List<SpaceReservationManager>(Managers))
                reservationManager.PositionAndDisplay(reservedGeometry);
        }

        public bool IsMouseOver
        {
            get
            {
                foreach (var manager in Managers)
                {
                    if (manager.IsMouseOver)
                        return true;
                }
                return false;
            }
        }

        public bool HasAggregateFocus
        {
            get
            {
                foreach (var manager in Managers)
                {
                    if (manager.HasAggregateFocus)
                        return true;
                }
                return false;
            }
        }
    }
}