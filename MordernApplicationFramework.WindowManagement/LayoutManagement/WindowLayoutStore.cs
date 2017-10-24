using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
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

        private List<KeyValuePair<string, WindowLayoutInfo>> _cachedInfo;
        private readonly object _cachedInfoLock = new object();


        public override ISettingsCategory Category => Settings.WindowLayoutsSettingsCategory;
        public override string Name => "Environment_WindowLayoutStore";

        private IList<KeyValuePair<string, WindowLayoutInfo>> CachedInfo
        {
            get
            {
                if (_cachedInfo != null)
                    return _cachedInfo;
                lock (_cachedInfoLock)
                {
                    var layouts = GetStoredLayouts();
                    var source = layouts.Select(layout => new KeyValuePair<string, WindowLayoutInfo>(layout.Key, layout)).ToList();
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

        public KeyValuePair<string, WindowLayoutInfo> GetLayoutAt(int index)
        {
            Validate.IsWithinRange(index, 0, CachedInfo.Count - 1, nameof(index));
            return CachedInfo[index];
        }

        public string GetLayoutDataAt(int index)
        {
            var payload = Encoding.UTF8.GetString(GZip.Decompress(Convert.FromBase64String(GetLayoutAt(index).Value.Payload)));
            return payload;
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

            var info = new WindowLayoutInfo(layoutName, flag ? keyValuePair.Value.Position : CachedInfo.Count, key, CompressAndEncode(data));
            InsertSettingsModel(info, true);
            OnSettingsChanged();
            return key;
        }

        public void UpdateLayouts(IEnumerable<KeyValuePair<string, WindowLayoutInfo>> keyInfoCollection)
        {
           // throw new System.NotImplementedException();
        }

        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
        }

        private IEnumerable<WindowLayoutInfo> GetStoredLayouts()
        {
            GetAllDataModel(out ICollection<WindowLayoutInfo> models);
            return models;
        }

        private string GetLayoutKeyAt(int index)
        {
            Validate.IsWithinRange(index, 0, CachedInfo.Count - 1, nameof(index));
            return CachedInfo[index].Key;
        }


        private static string CompressAndEncode(string data)
        {
            return Convert.ToBase64String(GZip.Compress(Encoding.UTF8.GetBytes(data)));
        }
    }
}