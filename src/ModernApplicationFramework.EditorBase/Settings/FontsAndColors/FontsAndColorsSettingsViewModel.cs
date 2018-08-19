using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Editor.TextManager;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingsDialog;
using Color = System.Windows.Media.Color;

namespace ModernApplicationFramework.EditorBase.Settings.FontsAndColors
{
    [Export(typeof(ISettingsPage))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class FontsAndColorsSettingsViewModel : SettingsPage, IStretchSettingsPanelPanel
    {
        private readonly FontsAndColorsSettingsFactory _factory;
        private IEnumerable<FontColorCategoryItem> _categories;
        private FontColorCategoryItem _selectedCategory;
        private FontNameItem _selectedFont;
        public override uint SortOrder => 11;
        public override string Name => "Fonts and Colors";
        public override SettingsPageCategory Category => SettingsPageCategories.EnvironmentCategory;


        public IObservableCollection<FontColorEntry> Items { get; }

        public IEnumerable<FontColorCategoryItem> Categories { get; }

        public IEnumerable<FontNameItem> InstalledFonts { get; }


        public FontNameItem SelectedFont
        {
            get => _selectedFont;
            set
            {
                if (_selectedFont.Equals(value))
                    return;
                _selectedFont = value;
                OnPropertyChanged();
            }
        }

        public FontColorCategoryItem SelectedCategoryItem
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory.Equals(value))
                    return;
                _selectedCategory = value;
                OnPropertyChanged();
                ChangeCategory(value);
            }
        }

        private FontColorCategory FontColorCategory { get; set; }

        [ImportingConstructor]
        public FontsAndColorsSettingsViewModel(FontsAndColorsSettingsFactory itemFactory)
        {
            Items = new BindableCollection<FontColorEntry>();
            _factory = itemFactory;
            Categories = itemFactory.GetCategories();
            InstalledFonts = _factory.GetFonNames();
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

        private void ChangeCategory(FontColorCategoryItem item)
        {
            FontColorCategory = new FontColorCategory(item.CategoryId, item.Name, item.IsGroup);
            Items.Clear();

            var guid = item.CategoryId;

            Items.AddRange(_factory.ItemEntriesFromCategory(item.CategoryId));

            try
            {
                _factory.Storage.OpenCategory(guid, StorageFlags.Loaddefaults);
                var fontInfo = new[]
                {
                    new FontInfo()
                };
                _factory.Storage.GetFont(null, fontInfo);
                SelectedFont = InstalledFonts.FirstOrDefault(x => x.Name == fontInfo[0].Typeface);

            }
            finally
            {
                _factory.Storage.CloseCategory();
            }     

        }

    }


    internal struct FontColorCategoryItem : IEquatable<FontColorCategoryItem>
    {
        public Guid CategoryId { get; }

        public string Name { get; }

        public bool IsGroup { get; }

        public FontColorCategoryItem(Guid categoryId, string name, bool isGroup)
        {
            CategoryId = categoryId;
            Name = name;
            IsGroup = isGroup;
        }


        public bool Equals(FontColorCategoryItem other)
        {
            return CategoryId == other.CategoryId;
        }
    }



    internal sealed class FontColorCategory
    {
        public Guid CategoryGuid { get; }

        public string Name { get; }

        public bool IsGroup { get; }

        public FontInformation Font { get; set; }

        public List<FontColorEntry> Colors { get; }

        public FontColorCategory(Guid categoryGuid, string name, bool isGroup)
        {
            CategoryGuid = categoryGuid;
            Name = name;
            IsGroup = isGroup;
        }
    }

    public struct FontInformation
    {
        public string Typeface { get; set; }

        public short PointSize { get; set; }
    }

    public sealed class FontColorEntry : IEquatable<FontColorEntry>
    {
        public string Name { get; }

        public FontFlags FontFlags { get; set; }

        public Color BackgroundColor { get; set; }

        public Color Foreground { get; set; }

        public FontColorEntry(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is FontColorEntry entry))
                return false;
            return Equals(entry);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) FontFlags;
                hashCode = (hashCode * 397) ^ BackgroundColor.GetHashCode();
                hashCode = (hashCode * 397) ^ Foreground.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(FontColorEntry other)
        {
            if (other == null)
                return false;
            return Name == other.Name && FontFlags == other.FontFlags && BackgroundColor == other.BackgroundColor &&
                   Foreground == other.Foreground;
        }
    }


}
