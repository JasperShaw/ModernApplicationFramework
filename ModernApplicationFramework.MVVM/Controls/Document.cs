using System.Windows.Input;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.Commands;

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