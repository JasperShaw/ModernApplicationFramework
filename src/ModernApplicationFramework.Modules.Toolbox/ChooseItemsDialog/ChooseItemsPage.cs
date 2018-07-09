using Caliburn.Micro;

namespace ModernApplicationFramework.Modules.Toolbox.ChooseItemsDialog
{
    public abstract class ChooseItemsPage : Screen, IToolboxItemPage
    {
        private ChooseItemsPageView _pageView;

        protected ChooseItemsPage()
        {
            ViewModelBinder.Bind(this, new ChooseItemsPageView(), null);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            //Release for GC
            _pageView = null;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            //Prevents View from getting destroyed by GC
            if (view is ChooseItemsPageView pageView)
                _pageView = pageView;
        }
    }
}
