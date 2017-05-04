using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsDialog.ViewModels
{
    [Export(typeof(SettingsWindowViewModel))]
    public sealed class SettingsWindowViewModel : Screen
    {
        private SettingsPageViewModel _selectedPage;
        private IEnumerable<ISettingsPage> _settingPages;

        public ICommand CancelCommand => new Command(Cancel);

        public ICommand OkCommand => new Command(ApplyChanges);

        public List<SettingsPageViewModel> Pages { get; private set; }

        public SettingsPageViewModel SelectedPage
        {
            get => _selectedPage;
            set
            {
                _selectedPage = value;
                NotifyOfPropertyChange();
            }
        }

        public SettingsWindowViewModel()
        {
            DisplayName = "Options";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var pages = new List<SettingsPageViewModel>();

            _settingPages = IoC.GetAll<ISettingsPage>().OrderBy(x => x.SortOrder);

            var groups = _settingPages.Select(p => p.Category.Root).Distinct().OrderBy(x => x.SortOrder);
            foreach (var settingsCategory in groups)
                FillRecursive(settingsCategory, pages);
            Pages = pages;
            SelectedPage = GetFirstLeafPageRecursive(pages);
        }


        private static SettingsPageViewModel GetFirstLeafPageRecursive(List<SettingsPageViewModel> pages)
        {
            if (!pages.Any())
                return null;
            var firstPage = pages.First();
            return !firstPage.Children.Any() ? firstPage : GetFirstLeafPageRecursive(firstPage.Children);
        }

        private static IList<SettingsPageViewModel> GetParentCollection(ISettingsPage settingPage,
            IList<SettingsPageViewModel> pages)
        {
            if (settingPage.Category == null)
                return pages;

            foreach (var category in settingPage.Category.Path)
            {
                var page = pages.FirstOrDefault(s => s.Category == category);
                if (page == null)
                {
                    page = new SettingsPageViewModel { Name = category.Name, Category = category };
                    page.Pages.Add(settingPage);
                    pages.Add(page);
                }
                else if (page.Pages.First().Name == settingPage.Name)
                {
                    page.Pages.Add(settingPage);
                }
                pages = page.Children;
            }
            return pages;
        }

        private void FillRecursive(SettingsCategory category, IList<SettingsPageViewModel> pagesList)
        {
            var subGroups = category.Children.OrderBy(x => x.SortOrder);

            foreach (var settingsCategory in subGroups)
                FillRecursive(settingsCategory, pagesList);

            var toppages = _settingPages.Where(x => x.Category == category);
            foreach (var settingPage in toppages)
            {
                var parentCollection = GetParentCollection(settingPage, pagesList);

                var page = parentCollection.FirstOrDefault(m => m.Name == settingPage.Name);

                if (page == null)
                {
                    page = new SettingsPageViewModel
                    {
                        Name = settingPage.Name,
                        Category = settingPage.Category
                    };
                    parentCollection.Add(page);
                }
                page.Pages.Add(settingPage);
            }
        }

        private void ApplyChanges()
        {
            if (_settingPages.Any(settingPage => !settingPage.CanApply()))
                return;
            foreach (var settingPage in _settingPages)
                settingPage.Apply();

            TryClose(true);
        }

        private void Cancel()
        {
            TryClose(false);
        }
    }
}