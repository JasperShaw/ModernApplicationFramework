using System.Windows.Input;
using ModernApplicationFramework.Docking.Commands;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Controls
{
    public abstract class Document : LayoutItemBase, IDocument
    {
        private ICommand _closeCommand;

        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p => true)); }
        }
    }
}