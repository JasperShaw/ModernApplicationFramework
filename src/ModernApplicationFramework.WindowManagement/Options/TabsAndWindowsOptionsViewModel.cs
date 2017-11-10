using System.ComponentModel;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.WindowManagement.LayoutManagement;

namespace ModernApplicationFramework.WindowManagement.Options
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TabsAndWindowsOptionsViewModel : SettingsPage
    {
        private readonly StorableTabsAndWindowsOptions _storableTabsAndWindowsOptions;
        private IWindowLayoutSettings _layoutSettings;
        public override uint SortOrder => 3;
        public override string Name => "Tabs";
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;

        public DockPreference DocumentDockPreference
        {
            get => DockingManagerPreferences.DocumentDockPreference;
            set => DockingManagerPreferences.DocumentDockPreference = value;
        }

        public bool MaintainPinStatus
        {
            get => DockingManagerPreferences.MaintainPinStatus;
            set => DockingManagerPreferences.MaintainPinStatus = value;
        }

        public bool ShowPinnedTabsInSeparateRow
        {
            get => DockingManagerPreferences.IsPinnedTabPanelSeparate;
            set => DockingManagerPreferences.IsPinnedTabPanelSeparate = value;
        }

        public bool ShowPinButtonInUnpinnedTabs
        {
            get => DockingManagerPreferences.ShowPinButtonInUnpinnedTabs;
            set => DockingManagerPreferences.ShowPinButtonInUnpinnedTabs = value;
        }

        public bool ShowAutoHiddenWindowsOnHover
        {
            get => DockingManagerPreferences.ShowAutoHiddenWindowsOnHover;
            set => DockingManagerPreferences.ShowAutoHiddenWindowsOnHover = value;
        }

        public bool ShowApplyLayoutConfirmation
        {
            get
            {
                var layoutSettings = LayoutSettings;
                if (layoutSettings != null)
                    return !layoutSettings.SkipApplyLayoutConfirmation;
                return true;
            }
            set
            {
                var flag = !value;
                var layoutSettings = LayoutSettings;
                if (layoutSettings == null || layoutSettings.SkipApplyLayoutConfirmation == flag)
                    return;
                layoutSettings.SkipApplyLayoutConfirmation = flag;
            }
        }

        private DockingManagerPreferences DockingManagerPreferences => DockingManagerPreferences.Instance;

        private IWindowLayoutSettings LayoutSettings
        {
            get
            {
                if (_layoutSettings != null)
                    return _layoutSettings;
                _layoutSettings = LayoutManagementService.Instance.LayoutSettings;
                if (_layoutSettings is INotifyPropertyChanged layoutSettings)
                    layoutSettings.PropertyChanged += OnLayoutSettingsChanged;
                return _layoutSettings;
            }
        }

        [ImportingConstructor]
        public TabsAndWindowsOptionsViewModel(StorableTabsAndWindowsOptions storableTabsAndWindowsOptions)
        {
            _storableTabsAndWindowsOptions = storableTabsAndWindowsOptions;
        }

        protected override bool SetData()
        {
            _storableTabsAndWindowsOptions.StoreSettings();
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {
        }

        private void OnLayoutSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SkipApplyLayoutConfirmation")
                return;
            ShowApplyLayoutConfirmation = !LayoutSettings.SkipApplyLayoutConfirmation;
        }
    }
}
