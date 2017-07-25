using System.ComponentModel.Composition;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Extended.Settings.Keyboard
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class KeyboardSettingsViewModel : AbstractSettingsPage, IStretchSettingsPanelPanel
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IDialogProvider _dialogProvider;
        public override uint SortOrder => 15;
        public override string Name => "Keyboard";
        public override ISettingsCategory Category => SettingsCategories.EnvironmentCategory;


        [ImportingConstructor]
        public KeyboardSettingsViewModel(ISettingsManager settingsManager,
            IDialogProvider dialogProvider)
        {
            _settingsManager = settingsManager;
            _dialogProvider = dialogProvider;
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
