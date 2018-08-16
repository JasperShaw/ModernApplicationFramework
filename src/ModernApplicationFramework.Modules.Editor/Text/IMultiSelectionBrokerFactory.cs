using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Text;

namespace ModernApplicationFramework.Modules.Editor.Text
{
    public interface IMultiSelectionBrokerFactory
    {
        IMultiSelectionBroker CreateBroker(ITextView textView);
    }
}
