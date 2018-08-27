using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using ModernApplicationFramework.Basics.Definitions.CommandBar.Elements;
using ModernApplicationFramework.Basics.Search;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Extended.Utilities.PaneUtilities;
using ModernApplicationFramework.Interfaces.Search;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolSearch
{

    [Export(typeof(ToolSearchExampleViewModel))]
    [Guid("9CB5AAF2-57E4-41C1-9617-F182BC17DA47")]
    public sealed class ToolSearchExampleViewModel : Tool
    {
        private WindowSearchOptionEnumerator _searchOptionsEnum;
        private WindowSearchBooleanOption _testSearchOption;
        private bool _testSeachOptionValue;
        private WindowSearchCommandOption _testSearchOptionCommand;


        public override PaneLocation PreferredLocation => PaneLocation.Right;

        public override bool SearchEnabled => true;

        public override bool HasToolbar => true;

        public override CommandBarToolbar Toolbar => TestToolbar.TestToolBar as CommandBarToolbar;

        public override IEnumWindowSearchOptions SearchOptionsEnum
        {
            get
            {
                if (_searchOptionsEnum == null)
                {
                    _searchOptionsEnum = new WindowSearchOptionEnumerator(new IWindowSearchOption[]
                    {
                        TestSearchOption,
                        TestSearchOptionCommand,
                    });
                }

                return _searchOptionsEnum;
            }
        }

        private WindowSearchBooleanOption TestSearchOption
        {
            get
            {
                var testOption = _testSearchOption;
                if (testOption != null)
                    return testOption;

                bool Func() => TestSeachOptionValue;
                return _testSearchOption =
                    new WindowSearchBooleanOption("Test Option", "Test Tooltip", Func, b => TestSeachOptionValue = b);
            }
        }

        private WindowSearchCommandOption TestSearchOptionCommand
        {
            get
            {
                var testOption = _testSearchOptionCommand;
                if (testOption != null)
                    return testOption;

                return _testSearchOptionCommand = new WindowSearchCommandOption("Test Command", "Test Tooltip", CommandAction);
            }
        }

        public bool TestSeachOptionValue
        {
            get => _testSeachOptionValue;
            set
            {
                _testSeachOptionValue = value;
                SearchHost?.SearchEvents?.SearchOptionValueChanged(TestSearchOption);
            }
        }

        public ToolSearchExampleViewModel()
        {
            DisplayName = "Search Example";
        }

        public override void ProvideSearchSettings(SearchSettingsDataSource dataSource)
        {
            base.ProvideSearchSettings(dataSource);
        }

        public override void ClearSearch()
        {
            base.ClearSearch();
        }

        public override ISearchTask CreateSearch(uint cookie, ISearchQuery searchQuery, ISearchCallback searchCallback)
        {
            return base.CreateSearch(cookie, searchQuery, searchCallback);
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            SearchControlPlacement = SearchPlacement.Stretch;
        }

        private static void CommandAction()
        {
            MessageBox.Show("Test Message");
        }
    }
}
