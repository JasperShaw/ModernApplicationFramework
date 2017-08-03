using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Input
{
    public class GestureCollectionChangedEventArgs : EventArgs
    {
        public GestureCollectionChangedType Type { get; }

        public IReadOnlyCollection<GestureScopeMapping> CategoryKeyGesture { get; }

        public GestureCollectionChangedEventArgs(GestureCollectionChangedType type, IEnumerable<GestureScopeMapping> list)
        {
            Type = type;
            CategoryKeyGesture = new List<GestureScopeMapping>(list);
        }

        public GestureCollectionChangedEventArgs(GestureCollectionChangedType type, GestureScopeMapping keyGestureScope)
        {
            Type = type;
            CategoryKeyGesture = new List<GestureScopeMapping> { keyGestureScope };
        }
    }
}