using System.Collections.Generic;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    public class SettingsPageViewModel
    {
        public SettingsPageViewModel()
        {
            Children = new List<SettingsPageViewModel>();
            Pages = new List<ISettingsPage>();
        }

        public List<SettingsPageViewModel> Children { get; private set; }

        public string Name { get; set; }
        public List<ISettingsPage> Pages { get; private set; }
    }
}