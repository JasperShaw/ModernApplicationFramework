using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ModernApplicationFramework.Core.Events;
using ModernApplicationFramework.Core.Input;

namespace ModernApplicationFramework.Controls
{
    //http://www.codeproject.com/Articles/234703/WPF-TextBox-with-PreviewTextChanged-event-for-filt
    public class TextBox : System.Windows.Controls.TextBox
    {
        static TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        public bool PreviewUndoEnabled { get; set; }

        #region Event

        public static readonly RoutedEvent PreviewTextChangedEvent =
            EventManager.RegisterRoutedEvent("PreviewTextChanged", RoutingStrategy.Tunnel,
                typeof(PreviewTextChangedEventHandler), typeof(TextBox));

        public event PreviewTextChangedEventHandler PrevieTextChanged
        {
            add { AddHandler(PreviewTextChangedEvent, value); }
            remove { RemoveHandler(PreviewTextChangedEvent, value); }
        }

        #endregion

        #region Protected Methods

        protected override void OnInitialized(EventArgs e)
        {
            AddHandler(CommandManager.PreviewExecutedEvent, new ExecutedRoutedEventHandler(PreviewExecutedEvent), true);
            base.OnInitialized(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                TextInput(e, " ");
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            TextInput(e, e.Text);
            base.OnPreviewTextInput(e);
        }

        protected virtual void OnPreviewTextChanged(PreviewTextChangedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        #region Private Methods

        private void PreviewExecutedEvent(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Delete)
            {
                var start = SelectionStart;
                var lenght = SelectionLength;

                if (lenght > 0)
                    TextDelete(e, start, lenght);
                return;
            }
            if (e.Command == ApplicationCommands.Paste)
            {
                var dataObject = Clipboard.GetDataObject();
                if (dataObject != null)
                    TextInput(e, (string) dataObject.GetData(typeof(string)));
                return;
            }
            if (PreviewUndoEnabled)
            {
                if (e.Command == ApplicationCommands.Undo)
                {
                    if (!Undo())
                        return;
                    var text = Text;
                    Redo();

                    TextChange(e, TextChangedType.Undo, text);
                    return;
                }
                if (e.Command == ApplicationCommands.Redo)
                {
                    if (!Redo())
                        return;
                    var text = Text;
                    Undo();
                    TextChange(e, TextChangedType.Redo, text);
                    return;
                }
            }
            if (e.Command == EditingCommands.Backspace)
            {
                var start = SelectionStart;
                var lenght = SelectionLength;

                if (lenght > 0 || start > 0)
                {
                    if (lenght > 0)
                        TextDelete(e, start, lenght);
                    else
                        TextDelete(e, start - 1, 1);
                }
                return;
            }
            if (e.Command == EditingCommands.DeleteNextWord)
            {
                var text = Text;
                var length = text.Length;

                var start = CaretIndex;
                var end = start;

                while (end < length && !char.IsWhiteSpace(text[end]))
                    end++;

                while (end < length && char.IsWhiteSpace(text[end]))
                    end++;

                if (end > start)
                    TextDelete(e, start, end - start);
                return;
            }

            if (e.Command == EditingCommands.DeletePreviousWord)
            {
                var text = Text;

                var end = CaretIndex;
                var start = end;

                while (start > 0 && char.IsWhiteSpace(text[start - 1]))
                    start--;

                while (start > 0 && !char.IsWhiteSpace(text[start - 1]))
                    start--;

                if (end > start)
                    TextDelete(e, start, end - start);
            }
        }

        private new void TextInput(RoutedEventArgs e, string text)
        {
            var start = SelectionStart;
            var length = SelectionLength;

            if (length > 0)
                TextReplace(e, start, length, text);
            else
                TextInsert(e, start, text);
        }

        private void TextDelete(RoutedEventArgs e, int startIndex, int count)
        {
            TextChange(e, TextChangedType.Delete, Text.Remove(startIndex, count));
        }

        private void TextInsert(RoutedEventArgs e, int startIndex, string text)
        {
            TextChange(e, TextChangedType.Insert, Text.Insert(startIndex, text));
        }

        private void TextReplace(RoutedEventArgs e, int startIndex, int count, string text)
        {
            TextChange(e, TextChangedType.Replace, text.Remove(startIndex, count).Insert(startIndex, text));
        }

        private void TextChange(RoutedEventArgs e, TextChangedType type, string text)
        {
            if (Text == text)
                return;
            if (TextChange(type, text))
                e.Handled = true;
        }

        private bool TextChange(TextChangedType type, string text)
        {
            var e = new PreviewTextChangedEventArgs(PreviewTextChangedEvent, type, text);
            OnPreviewTextChanged(e);
            return e.Handled;
        }

        #endregion
    }
}