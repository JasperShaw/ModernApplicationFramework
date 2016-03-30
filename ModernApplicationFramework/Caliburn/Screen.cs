using System;
using ModernApplicationFramework.Caliburn.EventArgs;
using ModernApplicationFramework.Caliburn.Interfaces;
using ModernApplicationFramework.Caliburn.Logger;

namespace ModernApplicationFramework.Caliburn
{
    /// <summary>
    /// A base implementation of <see cref = "IScreen" />.
    /// </summary>
    public class Screen : ViewAware, IScreen, IChild
    {
        static readonly ILog Log = LogManager.GetLog(typeof (Screen));
        string _displayName;

        bool _isActive;
        bool _isInitialized;
        object _parent;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public Screen()
        {
            _displayName = GetType().FullName;
        }

        void IActivate.Activate()
        {
            if (IsActive)
            {
                return;
            }

            var initialized = false;

            if (!IsInitialized)
            {
                IsInitialized = initialized = true;
                OnInitialize();
            }

            IsActive = true;
            Log.Info("Activating {0}.", this);
            OnActivate();

            var handler = Activated;
            handler?.Invoke(this, new ActivationEventArgs
            {
                WasInitialized = initialized
            });
        }

        /// <summary>
        /// Raised after activation occurs.
        /// </summary>
        public virtual event EventHandler<ActivationEventArgs> Activated = delegate { };

        /// <summary>
        /// Indicates whether or not this instance is currently active.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsActive
        {
            get { return _isActive; }
            private set
            {
                _isActive = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or Sets the Parent <see cref = "IConductor" />
        /// </summary>
        public virtual object Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Tries to close this instance by asking its Parent to initiate shutdown or by asking its corresponding view to close.
        /// Also provides an opportunity to pass a dialog result to it's corresponding view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        public virtual void TryClose(bool? dialogResult = null)
        {
            PlatformProvider.PlatformProvider.Current.GetViewCloseAction(this, Views.Values, dialogResult).OnUiThread();
        }

        /// <summary>
        /// Raised before deactivation.
        /// </summary>
        public virtual event EventHandler<DeactivationEventArgs> AttemptingDeactivation = delegate { };

        void IDeactivate.Deactivate(bool close)
        {
            if (IsActive || (IsInitialized && close))
            {
                var attemptingDeactivationHandler = AttemptingDeactivation;
                attemptingDeactivationHandler?.Invoke(this, new DeactivationEventArgs
                {
                    WasClosed = close
                });

                IsActive = false;
                Log.Info("Deactivating {0}.", this);
                OnDeactivate(close);

                var deactivatedHandler = Deactivated;
                deactivatedHandler?.Invoke(this, new DeactivationEventArgs
                {
                    WasClosed = close
                });

                if (close)
                {
                    Views.Clear();
                    Log.Info("Closed {0}.", this);
                }
            }
        }

        /// <summary>
        /// Raised after deactivation.
        /// </summary>
        public virtual event EventHandler<DeactivationEventArgs> Deactivated = delegate { };

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name = "callback">The implementor calls this action with the result of the close check.</param>
        public virtual void CanClose(Action<bool> callback)
        {
            callback(true);
        }

        /// <summary>
        /// Gets or Sets the Display Name
        /// </summary>
        public virtual string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Indicates whether or not this instance is currently initialized.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsInitialized
        {
            get { return _isInitialized; }
            private set
            {
                _isInitialized = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected virtual void OnActivate()
        {
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name = "close">Inidicates whether this instance will be closed.</param>
        protected virtual void OnDeactivate(bool close)
        {
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }
    }
}