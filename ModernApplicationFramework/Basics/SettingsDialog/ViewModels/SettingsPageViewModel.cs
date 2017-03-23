using System.Collections.Generic;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsDialog.ViewModels
{
    public class SettingsPageViewModel
    {
        public SettingsPageViewModel()
        {
            Children = new List<SettingsPageViewModel>();
            Pages = new List<ISettingsPage>();
        }

        public List<SettingsPageViewModel> Children { get; }

        public string Name { get; set; }
        public List<ISettingsPage> Pages { get; }
    }
}