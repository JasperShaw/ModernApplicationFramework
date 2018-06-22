using System;
using ModernApplicationFramework.Controls.SearchControl;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    internal class WindowSearchMruItem : SearchMruItem
    {
        private IWindowSearchEventsHandler EventsHandler { get; }

        public WindowSearchMruItem(string text, IWindowSearchEventsHandler eventsHandler)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException(nameof(text));
            Text = text;
            EventsHandler = eventsHandler ?? throw new ArgumentNullException(nameof(eventsHandler));
        }

        protected override void OnDelete()
        {
            EventsHandler.OnDeleteMruItem(this);
        }

        protected override void OnSelect()
        {
            EventsHandler.OnSelectMruItem(this);
        }
    }
}