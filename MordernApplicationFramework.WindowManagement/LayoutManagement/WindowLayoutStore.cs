using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        [ImportingConstructor]
        public WindowLayoutStore(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }


        private IList<KeyValuePair<string, WindowLayoutInfo>> CachedInfo
        {
            get
            {
                var cachedInfo = _cachedInfo;
                if (_cachedInfo == null)
                {
                    var tupleList = new List<Tuple<string, string>>();
                    lock (_cachedInfoLock)
                    {
                        List<KeyValuePair<string, WindowLayoutInfo>> source = new List<KeyValuePair<string, WindowLayoutInfo>>();
                        //foreach (string key in LayoutList.Keys)
                        //{
                            
                        //}
                    }
                }
                return cachedInfo;
            }
        }



        public KeyValuePair<string, WindowLayoutInfo> GetLayoutAt(int index)
        {
            throw new NotImplementedException();
        }

        public string GetLayoutDataAt(int index)
        {
            throw new NotImplementedException();
        }

        public int GetLayoutCount()
        {
            //TODO:
            return 0;
        }

        public string SaveLayout(string layoutName, string data)
        {
            Validate.IsNotNullAndNotWhiteSpace(layoutName, nameof(layoutName));
            layoutName = LayoutManagementUtilities.NormalizeName(layoutName);

            var key = LayoutManagementUtilities.GenerateKey();
            var info = new WindowLayoutInfo(layoutName, 0, key);
            InsertSettingsModel(info, true);

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
    }
}