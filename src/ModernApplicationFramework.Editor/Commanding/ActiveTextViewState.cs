using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Editor.Commanding
{
    [Export(typeof(ITextViewCreationListener))]
    [Export(typeof(IActiveTextViewState))]
    [ContentType("any")]
    [TextViewRole("INTERACTIVE")]
    internal class ActiveTextViewState : ITextViewCreationListener, IActiveTextViewState
    {
        private readonly ConditionalWeakTable<ITextView, EditorAndMenuFocusTracker> _mapping;

        private readonly object _lockObj = new object();
        private static IActiveTextViewState _instance;

        public ITextView ActiveTextView { get; set; }

        internal static IActiveTextViewState Instance => _instance ?? (_instance = IoC.Get<IActiveTextViewState>());

        public ICommandTarget ActiveCommandTarget
        {
            get
            {
                if (ActiveTextView == null)
                    return null;
                ActiveTextView.Properties.TryGetProperty<ICommandTarget>(typeof(ICommandTarget), out var commandTarget);
                return commandTarget;
            }
        }

        public ActiveTextViewState()
        {
            _mapping = new ConditionalWeakTable<ITextView, EditorAndMenuFocusTracker>();
        }

        public void TextViewCreated(ITextView textView)
        {
            textView.Closed += TextView_Closed;
            var focusTracker = new EditorAndMenuFocusTracker(textView);
            _mapping.Add(textView, focusTracker);
            focusTracker.GotFocus += FocusTrackerGotFocus;
            focusTracker.LostFocus += FocusTrackerOnLostFocus;
        }

        private void FocusTrackerOnLostFocus(object sender, EventArgs e)
        {
            lock (_lockObj)
                ActiveTextView = null;
        }

        private void FocusTrackerGotFocus(object sender, EventArgs e)
        {
            lock (_lockObj)
                ActiveTextView = sender as ITextView;
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            if (!(sender is ITextView textView))
                return;
            if (_mapping.TryGetValue(textView, out var tracker))
            {
                tracker.GotFocus -= FocusTrackerGotFocus;
                tracker.LostFocus -= FocusTrackerOnLostFocus;
                _mapping.Remove(textView);
            }
            textView.Closed -= TextView_Closed;
        }
    }
}