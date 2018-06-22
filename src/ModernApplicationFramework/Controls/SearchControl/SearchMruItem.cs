using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ModernApplicationFramework.Controls.SearchControl
{
    public class SearchMruItem : INotifyPropertyChanged
    {
        private string _text;

        public string Text
        {
            get => _text;
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged();
            }
        }

        internal static void Select(SearchMruItem item)
        {
            item.OnSelect();
        }

        internal static void Delete(SearchMruItem item)
        {
            item.OnDelete();
        }

        protected virtual void OnDelete()
        {
            
        }

        protected virtual void OnSelect()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}