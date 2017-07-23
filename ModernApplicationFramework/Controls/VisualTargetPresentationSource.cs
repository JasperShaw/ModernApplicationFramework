using System;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;

namespace ModernApplicationFramework.Controls
{
    internal class VisualTargetPresentationSource : PresentationSource, IDisposable
    {
        private VisualTarget _visualTarget;

        public VisualTargetPresentationSource(HostVisual hostVisual)
        {
            if (hostVisual == null)
                throw new ArgumentNullException(nameof(hostVisual));
            _visualTarget = new VisualTarget(hostVisual);
        }

        public override Visual RootVisual
        {
            get { return _visualTarget.RootVisual; }
            [SecurityCritical]
            [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
            [UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
            set
            {
                var rootVisual = _visualTarget.RootVisual;
                if (Equals(rootVisual, value))
                    return;
                _visualTarget.RootVisual = value;
                RootChanged(rootVisual, value);
            }
        }

        public override bool IsDisposed => _visualTarget == null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~VisualTargetPresentationSource()
        {
            Dispose(false);
        }

        protected override CompositionTarget GetCompositionTargetCore()
        {
            return _visualTarget;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            using (_visualTarget)
            {
                _visualTarget = null;
            }
        }
    }
}