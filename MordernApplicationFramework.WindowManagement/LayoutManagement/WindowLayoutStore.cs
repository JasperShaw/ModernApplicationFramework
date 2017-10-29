using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(IWindowLayoutStore))]
    internal class WindowLayoutStore : SettingsDataModel, IWindowLayoutStore
    {

        private List<KeyValuePair<string, WindowLayout>> _cachedInfo;
        private readonly object _cachedInfoLock = new object();


        public override ISettingsCategory Category => Settings.WindowLayoutsSettingsCategory;
        public override string Name => "Environment_WindowLayoutStore";

        private IList<KeyValuePair<string, WindowLayout>> CachedInfo
        {
            get
            {
                if (_cachedInfo != null)
                    return _cachedInfo;
                lock (_cachedInfoLock)
                {
                    var layouts = GetStoredLayouts();
                    var source = layouts.Select(layout => new KeyValuePair<string, WindowLayout>(layout.Key, layout)).ToList();
                    _cachedInfo = source.OrderBy(kvp => kvp.Value?.Position ?? int.MaxValue).ThenBy(kvp => kvp.Key).ToList();
                }
                return _cachedInfo;
            }
        }

        [ImportingConstructor]
        public WindowLayoutStore(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
            SettingsChanged += WindowLayoutStore_SettingsChanged;
        }

        private async void WindowLayoutStore_SettingsChanged(object sender, EventArgs e)
        {
            await System.Threading.Tasks.Task.Yield();
            lock (_cachedInfoLock)
                _cachedInfo = null;
        }

        public KeyValuePair<string, WindowLayout> GetLayoutAt(int index)
        {
            Validate.IsWithinRange(index, 0, CachedInfo.Count - 1, nameof(index));
            return CachedInfo[index];
        }

        public string GetLayoutDataAt(int index)
        {
            return GetLayoutAt(index).Value.DecompressedPayload;
        }

        public int GetLayoutCount()
        {
            return CachedInfo.Count;
        }

        public string SaveLayout(string layoutName, string data)
        {
            Validate.IsNotNullAndNotWhiteSpace(layoutName, nameof(layoutName));
            layoutName = LayoutManagementUtilities.NormalizeName(layoutName);
            var keyValuePair = CachedInfo.FirstOrDefault(kvp => kvp.Value != null &&
                string.Equals(kvp.Value.Name, layoutName, StringComparison.CurrentCultureIgnoreCase));

            var flag = !string.IsNullOrEmpty(keyValuePair.Key);
            var key = flag ? keyValuePair.Key : LayoutManagementUtilities.GenerateKey();

            var info = new WindowLayout(layoutName, flag ? keyValuePair.Value.Position : CachedInfo.Count, key, data, true);
            if (!flag)
                InsertSettingsModel(info, true);
            else
            {
                RemoveAllModels();
                var list = CachedInfo;
                list.RemoveAt(info.Position);
                list.Add(new KeyValuePair<string, WindowLayout>(info.Key, info));
                foreach (var valuePair in list.OrderBy(x => x.Value.Position))
                    InsertSettingsModel(valuePair.Value, true);
            }
            
            OnSettingsChanged();
            return key;
        }

        public void UpdateStore(IEnumerable<KeyValuePair<string, WindowLayout>> newLayouts)
        {
            RemoveAllModels();
            foreach (var valuePair in newLayouts)
                SaveLayout(valuePair.Value.Name, valuePair.Value.DecompressedPayload);
            lock (_cachedInfoLock)
                _cachedInfo = null;
        }

        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
        }

        private IEnumerable<WindowLayout> GetStoredLayouts()
        {
            GetAllDataModel(out ICollection<WindowLayout> models);
            return models;
        }
    }
}