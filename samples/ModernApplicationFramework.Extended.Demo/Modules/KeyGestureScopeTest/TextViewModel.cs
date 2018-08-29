using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Text.Ui.Editor;

namespace ModernApplicationFramework.Extended.Demo.Modules.KeyGestureScopeTest
{
    [DisplayName("TextViewModel")]
    [Export(typeof(TextViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class TextViewModel : LayoutItem
    {
        private object _content;

        public object Content
        {
            get => _content;
            set
            {
                if (Equals(value, _content)) return;
                _content = value;
                NotifyOfPropertyChange();
            }
        }

        public TextViewModel()
        {
            DisplayName = "Text View";
            _content = IoC.Get<PrimitiveTextView>().Content;
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            (_content as ITextViewHost)?.Close();
        }
    }
}
