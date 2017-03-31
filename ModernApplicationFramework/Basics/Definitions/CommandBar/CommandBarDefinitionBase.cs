using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;
using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public abstract class CommandBarDefinitionBase : INotifyPropertyChanged
    {
        public abstract uint SortOrder { get; set; }
        public abstract string Text { get; set; }

        public abstract bool IsCustom { get; }
        public abstract bool IsChecked { get; set; }

        public abstract DefinitionBase CommandDefinition { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
