using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;

namespace MordernApplicationFramework.WindowManagement.Options
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TabsAndWindowsOptionsViewModel : SettingsPage
    {
        public override uint SortOrder => 3;
        public override string Name => "Tabs";
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;

        [ImportingConstructor]
        public TabsAndWindowsOptionsViewModel()
        {
            
        }

        protected override bool SetData()
        {
            return true;
        }

        public override bool CanApply()
        {
            return true;
        }

        public override void Activate()
        {
        }
    }
}
