using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ModernApplicationFramework.Basics.InfoBar.Internal;

namespace ModernApplicationFramework.Controls.InfoBar.Internal
{
    internal class InfoBarTextBlock : TextBlock
    {
        public static readonly DependencyProperty TextSourceProperty = DependencyProperty.Register(nameof(TextSource), typeof(IEnumerable<InfoBarTextViewModel>), typeof(InfoBarTextBlock), new FrameworkPropertyMetadata(null, OnInlinesInvalidated));
        private bool _updatingInlines;

        static InfoBarTextBlock()
        {
            TextProperty.OverrideMetadata(typeof(InfoBarTextBlock), new FrameworkPropertyMetadata(OnTextChanged));
        }

        public IEnumerable<InfoBarTextViewModel> TextSource
        {
            get => (IEnumerable<InfoBarTextViewModel>)GetValue(TextSourceProperty);
            set => SetValue(TextSourceProperty, value);
        }

        private static void OnInlinesInvalidated(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is InfoBarTextBlock infoBarTextBlock))
                return;
            infoBarTextBlock.UpdateInlines();
        }

        private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is InfoBarTextBlock infoBarTextBlock && !infoBarTextBlock._updatingInlines)
                throw new NotSupportedException("Error assign text");
        }

        private void UpdateInlines()
        {
            try
            {
                _updatingInlines = true;
                IEnumerable<InfoBarTextViewModel> textSource = TextSource;
                InlineCollection inlines = Inlines;
                inlines.Clear();
                if (textSource == null)
                    return;
                foreach (InfoBarTextViewModel barTextViewModel in textSource)
                {
                    var run = new Run {Text = barTextViewModel.Text};
                    if (barTextViewModel.Bold)
                        run.FontWeight = FontWeights.Bold;
                    if (barTextViewModel.Italic)
                        run.FontStyle = FontStyles.Italic;
                    if (barTextViewModel.Underline)
                        run.TextDecorations = System.Windows.TextDecorations.Underline;
                    if (barTextViewModel is InfoBarActionViewModel barActionViewModel)
                    {
                        var hyperlink = new Hyperlink(run)
                        {
                            DataContext = barActionViewModel,
                            Command = barActionViewModel.ClickActionItemCommand
                        };
                        inlines.Add(hyperlink);
                    }
                    else
                        inlines.Add(run);
                }
            }
            finally
            {
                _updatingInlines = false;
            }
        }
    }
}
