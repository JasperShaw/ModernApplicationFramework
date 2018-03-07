using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsManager;

namespace ModernApplicationFramework.EditorBase.Settings.EditorAssociation
{
    public static class EditorSettingsCategory
    {
        [Export]
        public static ISettingsCategory EditorCategory =
            new SettingsCategory(Guids.EditorCategoryGuid,
                SettingsCategoryType.Normal, "Editor_Group", null);

    }
}
