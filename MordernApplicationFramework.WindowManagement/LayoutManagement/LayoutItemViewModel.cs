using ModernApplicationFramework.Utilities;

namespace MordernApplicationFramework.WindowManagement.LayoutManagement
{
    internal sealed class LayoutItemViewModel : ObservableObject
    {
        internal string Key { get; }

        internal WindowLayoutInfo Info { get; }

        internal int Position
        {
            get => Info.Position;
            set => Info.Position = value;
        }

        public string Name
        {
            get => Info.Name;
            set
            {
                if (Info.Name == value)
                    return;
                Info.Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        internal LayoutItemViewModel(string key, WindowLayoutInfo info)
        {
            Validate.IsNotNullAndNotEmpty(key, "key");
            Key = key;
            Info = info;
        }
    }
}
