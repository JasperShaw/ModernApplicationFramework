using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Layout
{
    public abstract class KeyBindingLayoutItem : LayoutItem, ICanHaveInputBindings
    {
        public virtual IEnumerable<GestureScope> GestureScopes => new[]
            {ModernApplicationFramework.Input.Command.GestureScopes.GlobalGestureScope};
        public UIElement BindableElement { get; protected set; }

        protected override void OnViewLoaded(object view)
        {
            if (view is Control element)
            {
                BindableElement = element;
                IoC.Get<IKeyGestureService>().AddModel(this);
            }
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            if (!close)
                return;
            IoC.Get<IKeyGestureService>().RemoveModel(this);
            BindableElement = null;
        }
    }
}
