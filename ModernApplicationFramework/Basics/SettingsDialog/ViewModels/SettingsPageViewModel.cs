using System.Collections.Generic;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsDialog.ViewModels
{
    public class SettingsPageViewModel
    {
        public List<SettingsPageViewModel> Children { get; }

        public List<ISettingsPage> Pages { get; }

        public string Name { get; set; }

        public SettingsCategory Category { get; set; }

        public SettingsPageViewModel()
        {
            Children = new List<SettingsPageViewModel>();
            Pages = new List<ISettingsPage>();
        }
    }
}