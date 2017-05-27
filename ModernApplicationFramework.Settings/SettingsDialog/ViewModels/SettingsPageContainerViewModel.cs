using System.Collections.Generic;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsDialog.ViewModels
{
    public class SettingsPageContainerViewModel
    {
        public List<SettingsPageContainerViewModel> Children { get; }

        public List<ISettingsPage> Pages { get; }

        public string Text { get; set; }

        public ISettingsCategory Category { get; set; }

        public SettingsPageContainerViewModel()
        {
            Children = new List<SettingsPageContainerViewModel>();
            Pages = new List<ISettingsPage>();
        }
    }
}