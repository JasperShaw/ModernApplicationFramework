using System.ComponentModel;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Layout
{
    public class LayoutItem : LayoutItemBase, ILayoutItem
    {
        private bool _isStateDirty;
        private bool _markAsReadOnly;

        /// <summary>
        ///  Indicates whether this layout item's state is is dirty. If <see langword="true"/> the UI will show a dirty indicator in the item's tab.
        /// </summary>
        [Browsable(false)]
        public bool IsStateDirty
        {
            get => _isStateDirty;
            protected set
            {
                if (value == _isStateDirty) return;
                _isStateDirty = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Indicates whether this layout item shall be presented as readonly. If <see langword="true"/> the UI will show a lock symbol in the item's tab.
        /// </summary>
        public bool MarkAsReadOnly
        {
            get => _markAsReadOnly;
            protected set
            {
                if (value == _markAsReadOnly)
                    return;
                _markAsReadOnly = value;
                NotifyOfPropertyChange();
            }
        }

        public override void TryClose(bool? dialogResult = null)
        {
            DockingModel.CloseCommand.Execute(null);
        }
    }
}