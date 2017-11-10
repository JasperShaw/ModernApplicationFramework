using System.ComponentModel.Composition;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.WindowManagement.Options
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(StorableTabsAndWindowsOptions))]
    public class StorableTabsAndWindowsOptions : SettingsDataModel
    {
        private readonly DockingManagerPreferences _localPreferences;

        public override ISettingsCategory Category { get; }
        public override string Name => "Environment_TabsAndWindows";

        [ImportingConstructor]
        public StorableTabsAndWindowsOptions(ISettingsManager settingsManager, DockingManagerPreferences localPreferences)
        {
            _localPreferences = localPreferences;
            SettingsManager = settingsManager;
            Category = Settings.TabsAndWindowsCategory;
        }

        public override void LoadOrCreate()
        {
            SetClassPropertyFromPropertyValue(_localPreferences, "DocumentDockPreference", nameof(_localPreferences.DocumentDockPreference), (int)DockPreference.DockAtBeginning);
            SetClassPropertyFromPropertyValue(_localPreferences, "MaintainPinStatus", nameof(_localPreferences.MaintainPinStatus), false);
            SetClassPropertyFromPropertyValue(_localPreferences, "ShowPinnedTabsInSeparateRow", nameof(_localPreferences.IsPinnedTabPanelSeparate), false);
            SetClassPropertyFromPropertyValue(_localPreferences, "ShowPinButtonInUnpinnedTabs", nameof(_localPreferences.ShowPinButtonInUnpinnedTabs), true);
            SetClassPropertyFromPropertyValue(_localPreferences, "ShowAutoHiddenWindowsOnHover", nameof(_localPreferences.ShowAutoHiddenWindowsOnHover), false);
        }

        public override void StoreSettings()
        {
            SetPropertyValue("DocumentDockPreference", (int) _localPreferences.DocumentDockPreference);
            SetPropertyValue("MaintainPinStatus", _localPreferences.MaintainPinStatus);
            SetPropertyValue("ShowPinnedTabsInSeparateRow", _localPreferences.IsPinnedTabPanelSeparate);
            SetPropertyValue("ShowPinButtonInUnpinnedTabs", _localPreferences.ShowPinButtonInUnpinnedTabs);
            SetPropertyValue("ShowAutoHiddenWindowsOnHover", _localPreferences.ShowAutoHiddenWindowsOnHover);
        }
    }
}
