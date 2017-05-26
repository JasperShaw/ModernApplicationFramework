using System.Collections.Generic;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsDialog.ViewModels
{
    public class SettingsPageContainerViewModel
    {
        public List<SettingsPageContainerViewModel> Children { get; }

        public List<ISettingsPage> Pages { get; }

        public string Text { get; set; }

        public SettingsCategory Category { get; set; }

        public SettingsPageContainerViewModel()
        {
            Children = new List<SettingsPageContainerViewModel>();
            Pages = new List<ISettingsPage>();
        }
    }
}