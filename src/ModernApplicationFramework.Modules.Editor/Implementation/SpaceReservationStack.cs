using System;
using System.Collections.Generic;
using System.Windows.Media;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Modules.Editor.Implementation
{
    internal class SpaceReservationStack
    {
        internal readonly List<SpaceReservationManager> Managers = new List<SpaceReservationManager>();
        internal readonly TextView View;
        internal Dictionary<string, int> OrderedManagerDefinitions;
        private bool _hasAggregateFocus;

        public bool HasAggregateFocus
        {
            get
            {
                foreach (var manager in Managers)
                    if (manager.HasAggregateFocus)
                        return true;
                return false;
            }
        }

        public bool IsMouseOver
        {
            get
            {
                foreach (var manager in Managers)
                    if (manager.IsMouseOver)
                        return true;
                return false;
            }
        }

        public SpaceReservationStack(Dictionary<string, int> orderedManagerDefinitions, TextView view)
        {
            OrderedManagerDefinitions = orderedManagerDefinitions;
            View = view;
        }

        public ISpaceReservationManager GetOrCreateManager(string name)
        {
            foreach (var manager in Managers)
                if (manager.Name == name)
                    return manager;

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

        private void OnManagerGotFocus(object sender, EventArgs e)
        {
            _hasAggregateFocus = true;
            View.QueueAggregateFocusCheck();
        }

        private void OnManagerLostFocus(object sender, EventArgs e)
        {
            if (!_hasAggregateFocus)
                return;
            foreach (var manager in Managers)
                if (manager.HasAggregateFocus)
                    return;
            _hasAggregateFocus = false;
            View.QueueAggregateFocusCheck();
        }
    }
}