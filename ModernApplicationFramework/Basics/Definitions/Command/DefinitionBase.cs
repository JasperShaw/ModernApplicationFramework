using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics.Definitions.Command
{
    public abstract class DefinitionBase : INotifyPropertyChanged
    {
        private string _shortcutText;
        public abstract string Name { get; }
        public abstract string Text { get; }
        public abstract string ToolTip { get; }
        public abstract Uri IconSource { get; }
        public abstract string IconId { get; }
        public abstract bool IsList { get; }

        public virtual string ShortcutText
        {
            get => _shortcutText;
            set
            {
                if (value == _shortcutText)
                    return;
                _shortcutText = value;
                OnPropertyChanged();
            }
        }

        public abstract CommandControlTypes ControlType { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}