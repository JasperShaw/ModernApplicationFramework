using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Docking.Controls
{
    public static class LayoutSynchronizer
    {
        private static int _layoutSynchronizationRefCount;
        private static readonly HashSet<PresentationSource> ElementsToUpdate = new HashSet<PresentationSource>();
        private static bool _isUpdatingLayout;

        public static IDisposable BeginLayoutSynchronization()
        {
            return new LayoutSynchronizationScope();
        }

        public static bool IsSynchronizing => _layoutSynchronizationRefCount > 0;

        public static void Update(Visual element)
        {
            if (!IsSynchronizing || _isUpdatingLayout)
                return;
            var presentationSource = PresentationSource.FromVisual(element);
            if (presentationSource == null)
                return;
            ElementsToUpdate.Add(presentationSource);
        }

        private static void Synchronize()
        {
            if (_isUpdatingLayout)
                return;
            _isUpdatingLayout = true;
            try
            {
                foreach (var presentationSource in ElementsToUpdate)
                    (presentationSource.RootVisual as UIElement)?.UpdateLayout();
                ElementsToUpdate.Clear();
            }
            finally
            {
                _isUpdatingLayout = false;
            }
        }

        private class LayoutSynchronizationScope : DisposableObject
        {
            public LayoutSynchronizationScope()
            {
                ++_layoutSynchronizationRefCount;
            }

            protected override void DisposeManagedResources()
            {
                --_layoutSynchronizationRefCount;
                if (_layoutSynchronizationRefCount != 0)
                    return;
                Synchronize();
            }
        }
    }
}