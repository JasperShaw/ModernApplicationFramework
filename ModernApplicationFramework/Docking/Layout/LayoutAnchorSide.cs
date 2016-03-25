/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Windows.Markup;

namespace ModernApplicationFramework.Docking.Layout
{
    [ContentProperty("Children")]
    [Serializable]
    public class LayoutAnchorSide : LayoutGroup<LayoutAnchorGroup>
    {
        private AnchorSide _side;

        public AnchorSide Side
        {
            get { return _side; }
            private set
            {
                if (_side == value)
                    return;
                RaisePropertyChanging("Side");
                _side = value;
                RaisePropertyChanged("Side");
            }
        }

        protected override bool GetVisibility()
        {
            return Children.Count > 0;
        }

        protected override void OnParentChanged(ILayoutContainer oldValue, ILayoutContainer newValue)
        {
            base.OnParentChanged(oldValue, newValue);

            UpdateSide();
        }

        private void UpdateSide()
        {
            if (Equals(Root.LeftSide, this))
                Side = AnchorSide.Left;
            else if (Equals(Root.TopSide, this))
                Side = AnchorSide.Top;
            else if (Equals(Root.RightSide, this))
                Side = AnchorSide.Right;
            else if (Equals(Root.BottomSide, this))
                Side = AnchorSide.Bottom;
        }
    }
}