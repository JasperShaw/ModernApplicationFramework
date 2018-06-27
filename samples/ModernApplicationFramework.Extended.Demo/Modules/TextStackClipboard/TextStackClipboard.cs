using System.Linq;
using System.Windows;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;
using ModernApplicationFramework.Extended.Clipboard;

namespace ModernApplicationFramework.Extended.Demo.Modules.TextStackClipboard
{
    public sealed class TextStackClipboard : StackClipboard<TextClipboadData>
    {
        public TextStackClipboard(int maxCount) : base(maxCount)
        {
        }

        protected override TextClipboadData CreateItem(string persistenceData)
        {
            return TextClipboadData.Create(persistenceData);
        }

        protected override void OnClipboardPushed()
        {
            var t = System.Windows.Clipboard.GetDataObject();
            if (t == null)
                return;
            var f = t.GetFormats();
            if (f.Any(x => x == DataFormats.Text || x == DataFormats.UnicodeText))
                AddItem("");
        }
    }

    public class TextClipboadData : MruItem
    {

        private TextClipboadData(string data)
        {
        }

        public static TextClipboadData Create(string data)
        {
            return new TextClipboadData(data);
        }

        public override string PersistenceData { get; }

        public override bool Matches(string stringValue)
        {
            return false;
        }
    }
}
