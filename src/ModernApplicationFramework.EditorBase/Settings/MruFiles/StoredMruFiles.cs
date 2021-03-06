﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Settings;

namespace ModernApplicationFramework.EditorBase.Settings.MruFiles
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(StoredMruFiles))]
    public class StoredMruFiles : SettingsDataModel
    {
        [Export]
        public static ISettingsCategory MruFilesSettingsCategory =
            new SettingsCategory(Guids.MruFilesSettingsCategoryId,
                SettingsCategoryType.Normal, "Environment_MRUFiles", SettingsCategories.EnvironmentCategory);

        public override ISettingsCategory Category => MruFilesSettingsCategory;
        public override string Name => "MRUFiles";

        [ImportingConstructor]
        public StoredMruFiles(ISettingsManager settingsManager) : base(settingsManager)
        {
        }

        public IReadOnlyCollection<string> GetSotredItems()
        {
            GetAllDataModel(out ICollection<StorableMruFile> models);
            return models.Select(x => x.PersistenceData).ToList();
        }

        public void StoreItems(IReadOnlyCollection<MruItem> items)
        {
            RemoveAllModels();

            foreach (var mruItem in items)
                InsertSettingsModel(new StorableMruFile(mruItem.PersistenceData.ToString()), true);
        }
    }
}
