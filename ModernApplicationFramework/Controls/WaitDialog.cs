using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace ModernApplicationFramework.Controls
{
    public class WaitDialog : ModernChromeWindow
    {
        public static readonly DependencyProperty MessageTextProperty =
            DependencyProperty.Register("MessageText", typeof(string), typeof(WaitDialog),
                new FrameworkPropertyMetadata("Preparing..."));

        private readonly Dispatcher _dispatcher;

        static WaitDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitDialog),
                new FrameworkPropertyMetadata(typeof(WaitDialog)));
        }

        public string MessageText
        {
            get { return (string) GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        public bool ActionWasAborted { get; private set; }

        public bool? ShowDialog(Action action)
        {
            bool? result = true;
            // start a new thread to start the submitted action
            var t = new Thread(() =>
            {
                // start the submitted action
                try
                {
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    DoClose();
                }
            });
            _thread = t;
            t.Start();

            if (t.ThreadState != ThreadState.Stopped)
            {
                result = ShowDialog();
            }
            return result;
        }

        private Thread _thread;

        private void DoClose()
        {
            _dispatcher.BeginInvoke(new ThreadStart(Close));
        }

        private new bool? ShowDialog()
        {
            Topmost = true;
            return base.ShowDialog();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_thread.IsAlive)
                ActionWasAborted = true;
            _thread?.Abort();
            DoClose();
            base.OnClosed(e);
        }

        public WaitDialog()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            Height = 130;
            Width = 450;
        }
    }
}