using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics
{
    internal sealed class EnvironmentRenderCapabilities : DisposableObject, INotifyPropertyChanged
    {
        private static EnvironmentRenderCapabilities _current;

        private int _visualEffectsAllowed;
        private bool _areGradientsAllowed;
        private bool _areAnimationsAllowed;
        private uint _shellPropertyChangesCookie;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler RenderCapabilitiesChanged;

        public static EnvironmentRenderCapabilities Current => _current ?? (_current = new EnvironmentRenderCapabilities());

        public int VisualEffectsAllowed
        {
            get => _visualEffectsAllowed;
            set
            {
                if (_visualEffectsAllowed == value)
                    return;
                _visualEffectsAllowed = value;
                AreGradientsAllowed = value >= 2;
                AreAnimationsAllowed = value >= 1;
                OnPropertyChanged();
                RenderCapabilitiesChanged.RaiseEvent(this, EventArgs.Empty);
            }
        }

        public bool AreGradientsAllowed
        {
            get => _areGradientsAllowed;
            private set
            {
                if (_areGradientsAllowed == value)
                    return;
                _areGradientsAllowed = value;
                OnPropertyChanged();
            }
        }

        public bool AreAnimationsAllowed
        {
            get => _areAnimationsAllowed;
            private set
            {
                if (_areAnimationsAllowed == value)
                    return;
                _areAnimationsAllowed = value;
                OnPropertyChanged();
            }
        }

        protected override void DisposeManagedResources()
        {
            if ((int) _shellPropertyChangesCookie != 0)
                _shellPropertyChangesCookie = 0U;
            base.DisposeManagedResources();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
