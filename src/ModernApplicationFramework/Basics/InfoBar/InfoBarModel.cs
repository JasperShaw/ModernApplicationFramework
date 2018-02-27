using System.Collections.Generic;
using System.Linq;
using ModernApplicationFramework.Controls.InfoBar;
using ModernApplicationFramework.Interfaces.Controls.InfoBar;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics.InfoBar
{
    public class InfoBarModel
    {
        public bool IsCloseButtonVisible { get; }

        public IInfoBarActionItemCollection ActionItems { get; }

        public IInfoBarTextSpanCollection TextSpans { get;}

        public ImageInfo ImageInfo { get; }

        public bool UseImageInfo => ImageInfo.Id != null && ImageInfo.Path != null && ImageInfo.FromXamlResource != null;


        public InfoBarModel(string text, ImageInfo image = default(ImageInfo), bool isCloseButtonVisible = true)
            : this(new IInfoBarTextSpan[]
            {
                new InfoBarTextSpan(text)
            }, Enumerable.Empty<IInfoBarActionItem>(), image, (isCloseButtonVisible ? 1 : 0) != 0)
        {
        }

        public InfoBarModel(IEnumerable<IInfoBarTextSpan> textSpans, ImageInfo image = default(ImageInfo), bool isCloseButtonVisible = true)
            : this(textSpans, Enumerable.Empty<IInfoBarActionItem>(), image, isCloseButtonVisible)
        {
            
        }

        public InfoBarModel(string text, IEnumerable<IInfoBarActionItem> actionItems, ImageInfo image = default(ImageInfo), bool isCloseButtonVisible = true)
            : this(new IInfoBarTextSpan[]
            {
                new InfoBarTextSpan(text)
            }, actionItems, image, (isCloseButtonVisible ? 1 : 0) != 0)
        {
        }

        public InfoBarModel(IEnumerable<IInfoBarTextSpan> textSpans, IEnumerable<IInfoBarActionItem> actionItems, ImageInfo image = default(ImageInfo), bool isCloseButtonVisible = true)
        {
            Validate.IsNotNull(textSpans, nameof(textSpans));
            Validate.IsNotNull(actionItems, nameof(actionItems));

            IsCloseButtonVisible = isCloseButtonVisible;
            TextSpans = new TextSpanCollection(textSpans);
            ActionItems = new ActionItemCollection(actionItems);
            ImageInfo = image;
        }

        private class ActionItemCollection : IInfoBarActionItemCollection
        {
            private readonly IReadOnlyList<IInfoBarActionItem> _actionItems;

            public ActionItemCollection(IEnumerable<IInfoBarActionItem> actionItems)
            {
                _actionItems = actionItems as IReadOnlyList<IInfoBarActionItem> ?? actionItems.ToArray();
            }

            public int Count => _actionItems.Count;

            public IInfoBarActionItem GetItem(int index)
            {
                return _actionItems[index];
            }
        }

        private class TextSpanCollection : IInfoBarTextSpanCollection
        {
            private readonly IReadOnlyList<IInfoBarTextSpan> _textSpans;

            public TextSpanCollection(IEnumerable<IInfoBarTextSpan> textSpans)
            {
                _textSpans = textSpans as IReadOnlyList<IInfoBarTextSpan> ?? textSpans.ToArray();
            }

            public int Count => _textSpans.Count;

            public IInfoBarTextSpan GetSpan(int index)
            {
                return _textSpans[index];
            }
        }
    }
}