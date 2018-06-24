using System;
using System.Globalization;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.Search;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    public class ToolboxSearchTask : SearchTask
    {
        private readonly IToolbox _toolbox;

        public event EventHandler<EventArgs> SearchComplete;

        internal ToolboxSearchTask(uint cookie, ISearchQuery query, ISearchCallback callback, IToolbox toolbox) : base(cookie, query, callback)
        {
            _toolbox = toolbox;
        }

        protected override void OnStartSearch()
        {

            var currentLayout = _toolbox.CurrentLayout;

            uint result = 0;
            currentLayout.ForEach(x =>
            {
                result += x.SetItemsVisibleWhere(i => i.IsVisible &&
                    CultureInfo.CurrentCulture.CompareInfo.IndexOf(i.Name, SearchQuery.SearchString, CompareOptions.IgnoreCase) >= 0);
            });

            SearchResults = result;
            OnSearchComplete();
            base.OnStartSearch();
        }

        protected override void OnStopSearch()
        {
            SearchResults = 0;
        }

        protected virtual void OnSearchComplete()
        {
            SearchComplete?.Invoke(this, EventArgs.Empty);
        }
    }
}
