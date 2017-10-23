using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
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

            var payload = Encoding.UTF8.GetString(Decompress(Convert.FromBase64String(GetLayoutAt(index).Value.Payload), 4096));

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
            return Convert.ToBase64String(Compress(Encoding.UTF8.GetBytes(data)));
        }

        public static byte[] Compress(byte[] data)
        {
            Validate.IsNotNull((object)data, nameof(data));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream((Stream)memoryStream, CompressionMode.Compress, true))
                    gzipStream.Write(data, 0, data.Length);
                return memoryStream.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data, int bufferSize = 4096)
        {
            Validate.IsNotNull(data, nameof(data));
            Validate.IsWithinRange(bufferSize, 1, int.MaxValue, nameof(bufferSize));
            using (MemoryStream memoryStream1 = new MemoryStream(data))
            {
                using (GZipStream gzipStream = new GZipStream((Stream)memoryStream1, CompressionMode.Decompress))
                {
                    using (MemoryStream memoryStream2 = new MemoryStream())
                    {
                        byte[] buffer = new byte[bufferSize];
                        int count;
                        do
                        {
                            count = gzipStream.Read(buffer, 0, bufferSize);
                            if (count > 0)
                                memoryStream2.Write(buffer, 0, count);
                        }
                        while (count > 0);
                        return memoryStream2.ToArray();
                    }
                }
            }
        }
    }
}