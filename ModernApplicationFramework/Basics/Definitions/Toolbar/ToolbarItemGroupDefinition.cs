using System.ComponentModel;
using System.Runtime.CompilerServices;
using ModernApplicationFramework.Annotations;

namespace ModernApplicationFramework.Basics.Definitions.Toolbar
{
    public class ToolbarItemGroupDefinition : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ToolbarDefinition ParentToolbar { get; set; }

        public uint SortOrder { get; set; }

        public ToolbarItemGroupDefinition(ToolbarDefinition toolbar, uint sortOrder)
        {
            ParentToolbar = toolbar;
            SortOrder = sortOrder;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}