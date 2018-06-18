using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.Docking.Controls;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Controls.Toolbar
{
    internal class LayoutItemToolbarHost : BaseToolbarHost
    {
        private readonly LayoutItem _layoutItem;

        internal LayoutItemToolbarHost(LayoutItem layoutItem)
        {       
            Validate.IsNotNull(layoutItem, nameof(layoutItem));
            _layoutItem = layoutItem;
        }

        protected override IEnumerable<FrameworkElement> FocusableControls
        {
            get
            {
                foreach (var focusableControl in base.FocusableControls)
                    yield return focusableControl;

                if (_layoutItem.TryGetInfoBarHostIfCreated(out var control))
                    yield return control;
            }
        }
    }

    internal abstract class BaseToolbarHost : DisposableObject, IAnchorableToolbarHost, IAnchorableToolbarHostProvate
    {

        private IEnumerable<System.Windows.Controls.ToolBarTray> Trays
        {
            get
            {
                if (TopTray != null)
                    yield return TopTray;
                if (LeftTray != null)
                    yield return LeftTray;
                if (RightTray != null)
                    yield return RightTray;
                if (BottomTray != null)
                    yield return BottomTray;
            }
        }

        protected virtual IEnumerable<FrameworkElement> FocusableControls => Trays;

        protected AnchorableToolBarTray TopTray { get; private set; }

        protected AnchorableToolBarTray BottomTray { get; private set; }

        protected AnchorableToolBarTray LeftTray { get; private set; }

        protected AnchorableToolBarTray RightTray { get; private set; }

        protected BaseToolbarHost()
        {

        }

        ~BaseToolbarHost()
        {
            Dispose(false);
        }

        protected virtual void OnToolbarTrayCreated(AnchorableToolBarTray newTray, Dock location)
        {
        }

        //protected virtual void OnToolbarVisibilityChanged(IVsUIDataSource toolbarDataSource)
        //{
        //}

        private void SetToolbarTrayVisibilities(Visibility visibility)
        {
            foreach (var tray in Trays)
                tray.Visibility = visibility;
        }
    }

    internal interface IAnchorableToolbarHostProvate
    {
    }


    public interface IAnchorableToolbarHost
    {
    }


}
