using System.Collections.Generic;
using ISettingsPage = ModernApplicationFramework.Settings.Interfaces.ISettingsPage;

namespace ModernApplicationFramework.Settings.SettingsDialog.ViewModels
{
    public class SettingsPageContainerViewModel
    {
        public List<SettingsPageContainerViewModel> Children { get; }

        public List<ISettingsPage> Pages { get; }

        public string Text { get; set; }

        public SettingsPageCategory Category { get; set; }

        public SettingsPageContainerViewModel()
        {
            Children = new List<SettingsPageContainerViewModel>();
            Pages = new List<ISettingsPage>();
        }
    }
}