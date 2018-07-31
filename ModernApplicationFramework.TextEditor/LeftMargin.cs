using System.Windows.Controls;
using System.Windows.Media;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal class LeftMargin : ContainerMargin
    {
        private WpfMouseProcessor _dragDropProcessor;

        private LeftMargin(ITextViewHost textViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
            : base("Left", Orientation.Vertical, textViewHost, guardedOperations, marginState)
        {
            if (!textViewHost.TextView.Properties.TryGetProperty(typeof(IDragDropMouseProcessor), out IDragDropMouseProcessor property))
                return;
            _dragDropProcessor = new WpfMouseProcessor(this, new FrugalList<IMouseProcessor>
            {
                new DragDropHelper(this, textViewHost.TextView, property)
            }, guardedOperations);
            Background = Brushes.Transparent;
        }

        public static ITextViewMargin Create(ITextViewHost textViewHost, GuardedOperations guardedOperations, TextViewMarginState marginState)
        {
            LeftMargin leftMargin = new LeftMargin(textViewHost, guardedOperations, marginState);
            leftMargin.Initialize();
            return leftMargin;
        }

        protected override void Close()
        {
            if (_dragDropProcessor != null)
            {
                _dragDropProcessor.Dispose();
                _dragDropProcessor = null;
            }
            base.Close();
        }
    }
}