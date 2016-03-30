using System;
using System.Collections.Generic;
using ModernApplicationFramework.Caliburn.Collections;
using ModernApplicationFramework.Caliburn.EventArgs;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.Caliburn.Interfaces;

namespace ModernApplicationFramework.Caliburn
{
    /// <summary>
    /// A base implementation of <see cref = "IViewAware" /> which is capable of caching views by context.
    /// </summary>
    public class ViewAware : PropertyChangedBase, IViewAware
    {
        /// <summary>
        /// The default view context.
        /// </summary>
        public static readonly object DefaultContext = new object();

        /// <summary>
        /// Creates an instance of <see cref="ViewAware"/>.
        /// </summary>
        public ViewAware()
        {
            Views = new WeakValueDictionary<object, object>();
        }

        void IViewAware.AttachView(object view, object context)
        {
            Views[context ?? DefaultContext] = view;

            var nonGeneratedView = PlatformProvider.PlatformProvider.Current.GetFirstNonGeneratedView(view);
            PlatformProvider.PlatformProvider.Current.ExecuteOnFirstLoad(nonGeneratedView, OnViewLoaded);
            OnViewAttached(nonGeneratedView, context);
            ViewAttached(this, new ViewAttachedEventArgs {View = nonGeneratedView, Context = context});

            var activatable = this as IActivate;
            if (activatable == null || activatable.IsActive)
            {
                PlatformProvider.PlatformProvider.Current.ExecuteOnLayoutUpdated(nonGeneratedView, OnViewReady);
            }
            else
            {
                AttachViewReadyOnActivated(activatable, nonGeneratedView);
            }
        }

        /// <summary>
        /// Gets a view previously attached to this instance.
        /// </summary>
        /// <param name = "context">The context denoting which view to retrieve.</param>
        /// <returns>The view.</returns>
        public virtual object GetView(object context = null)
        {
            object view;
            Views.TryGetValue(context ?? DefaultContext, out view);
            return view;
        }

        /// <summary>
        /// Raised when a view is attached.
        /// </summary>
        public event EventHandler<ViewAttachedEventArgs> ViewAttached = delegate { };

        /// <summary>
        /// The view chache for this instance.
        /// </summary>
        protected IDictionary<object, object> Views { get; }

        /// <summary>
        /// Called when a view is attached.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="context">The context in which the view appears.</param>
        protected virtual void OnViewAttached(object view, object context)
        {
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name = "view"></param>
        protected virtual void OnViewLoaded(object view)
        {
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name = "view"></param>
        protected virtual void OnViewReady(object view)
        {
        }

        static void AttachViewReadyOnActivated(IActivate activatable, object nonGeneratedView)
        {
            var viewReference = new WeakReference(nonGeneratedView);
            EventHandler<ActivationEventArgs> handler = null;
            handler = (s, e) =>
            {
                ((IActivate) s).Activated -= handler;
                var view = viewReference.Target;
                if (view != null)
                {
                    PlatformProvider.PlatformProvider.Current.ExecuteOnLayoutUpdated(view, ((ViewAware) s).OnViewReady);
                }
            };
            activatable.Activated += handler;
        }
    }
}