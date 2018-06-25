using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ModernApplicationFramework.Extended.Annotations;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Extended.UIContext
{
    public class UiContext : INotifyPropertyChanged

    {
        private static readonly PropertyChangedEventArgs IsActivePropertyChangedEventArgs = new PropertyChangedEventArgs(nameof(IsActive));
        private bool _isActive;
        private bool _isKnown;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<UiContextChangedEventArgs> UiContextChanged;

        internal Guid Guid { get; private set; }
        internal uint Cookie { get; private set; }


        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isKnown)
                    throw new NotSupportedException();
                if (_isActive == value)
                    return;
                _isActive = value;
                UiContextImpl.Instance.SetContext(this);
            }
        }

        internal UiContext(Guid contextGuid, uint cookie, bool isActive, bool isKnown)
        {
            Guid = contextGuid;
            Cookie = cookie;
            _isActive = isActive;
            _isKnown = isKnown;
        }

        public static UiContext FromUIContextGuid(Guid contextGuid)
        {
            return UiContextImpl.Instance.Register(contextGuid, false);
        }

        public void WhenActivated(Action action)
        {
            if (IsActive)
            {
                action();
            }
            else
            {
                var whenActivatedHandler = new WhenActivatedHandler(this, action);
            }
        }

        internal void OnActivated(bool activated)
        {
            IsActive = activated;
            var list = UiContextChanged?.GetInvocationList();
            if (list != null)
            {
                var e = UiContextChangedEventArgs.From(activated);
                foreach (var entry in list)
                {
                    if (!(entry is EventHandler<UiContextChangedEventArgs> handler))
                        continue;
                    try
                    {
                        handler(this, e);
                    }
                    catch
                    {          
                    }
                }
            }
            PropertyChanged.RaiseEvent(this, IsActivePropertyChangedEventArgs);
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class WhenActivatedHandler
        {
            private readonly Action _action;
            private readonly ExecutionContextTrackerHelper.CapturedContext _capturedContext;

            public WhenActivatedHandler(UiContext context, Action action)
            {
                _action = action;
                _capturedContext = ExecutionContextTrackerHelper.CaptureCurrentContext();
                context.UiContextChanged += OnContextChanged;
            }

            private void OnContextChanged(object sender, UiContextChangedEventArgs e)
            {
                if (!e.Activated)
                    return;
                ((UiContext)sender).UiContextChanged -= OnContextChanged;
                _capturedContext.ExecuteUnderContext(_action);
                _capturedContext.Dispose();
            }
        }
    }
}
