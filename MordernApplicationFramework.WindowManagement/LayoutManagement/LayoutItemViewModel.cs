using ModernApplicationFramework.Utilities;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal sealed class LayoutItemViewModel : ObservableObject
    {
        internal string Key { get; }

        internal WindowLayout Layout { get; }

        internal int Position
        {
            get => Layout.Position;
            set => Layout.Position = value;
        }

        public string Name
        {
            get => Layout.Name;
            set
            {
                if (Layout.Name == value)
                    return;
                Layout.Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        internal LayoutItemViewModel(string key, WindowLayout layout)
        {
            Validate.IsNotNullAndNotEmpty(key, "key");
            Key = key;
            Layout = layout;
        }
    }
}
