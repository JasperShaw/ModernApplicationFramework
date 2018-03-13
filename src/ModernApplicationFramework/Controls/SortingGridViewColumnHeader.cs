using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernApplicationFramework.Controls
{
    public class SortingGridViewColumnHeader : GridViewColumnHeader
    {
        private Thumb _headerGripper;
        private static Cursor _splitCursorCache;

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(SortingGridViewColumnHeader), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty ListSortDirectionProperty = DependencyProperty.Register(
            "ListSortDirection", typeof(ListSortDirection), typeof(SortingGridViewColumnHeader), new PropertyMetadata(default(ListSortDirection)));

        private static Cursor SplitCursor => _splitCursorCache ?? (_splitCursorCache = GetCursor());

        public ListSortDirection ListSortDirection
        {
            get => (ListSortDirection)GetValue(ListSortDirectionProperty);
            set => SetValue(ListSortDirectionProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        static SortingGridViewColumnHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SortingGridViewColumnHeader),
                new FrameworkPropertyMetadata(typeof(SortingGridViewColumnHeader)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Somehow .net does not update the curser so we need to re-do it.
            switch (Role)
            {
                case GridViewColumnHeaderRole.Normal:
                    HookupGripperEvents();
                    break;
            }
        }

        private void HookupGripperEvents()
        {
            _headerGripper = GetTemplateChild("PART_HeaderGripper") as Thumb;
            if (_headerGripper == null)
                return;
            _headerGripper.Cursor = SplitCursor;
        }

        private static Cursor GetCursor()
        {
            var cursorStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("ModernApplicationFramework.Resources.split.cur");
            return new Cursor(cursorStream ?? throw new InvalidOperationException());
        }
    }
}