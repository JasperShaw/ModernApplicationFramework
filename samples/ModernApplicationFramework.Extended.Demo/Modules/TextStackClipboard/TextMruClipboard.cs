using System.Linq;
using System.Windows;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;
using ModernApplicationFramework.Extended.Clipboard;

namespace ModernApplicationFramework.Extended.Demo.Modules.TextStackClipboard
{
    public sealed class TextMruClipboard : MruClipboard<TextClipboadData>
    {
        public TextMruClipboard(int maxCount) : base(maxCount)
        {
        }

        protected override TextClipboadData CreateItem(object persistenceData)
        {
            return TextClipboadData.Create(persistenceData);
        }

        protected override void OnClipboardPushed()
        {
            var t = System.Windows.Clipboard.GetDataObject();
            if (t == null)
                return;
            var f = t.GetFormats();
            if (f.Any(x => x == DataFormats.Text))
                AddItem(System.Windows.Clipboard.GetData(DataFormats.Text));
        }
    }

    public class TextClipboadData : MruItem
    {

        private TextClipboadData(object data)
        {
            if (!(data is string text))
                return;
            Text = text;
            PersistenceData = text;
        }

        public static TextClipboadData Create(object data)
        {
            return new TextClipboadData(data);
        }

        public override object PersistenceData { get; }

        public override bool Matches(object data)
        {
            if (!(data is string text))
                return false;
            return Text.Equals(text);
        }
    }
}
