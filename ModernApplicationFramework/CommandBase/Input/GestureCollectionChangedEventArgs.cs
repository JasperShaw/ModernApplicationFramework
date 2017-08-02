using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.CommandBase.Input
{
    public class GestureCollectionChangedEventArgs : EventArgs
    {
        public GestureCollectionChangedType Type { get; }

        public IReadOnlyCollection<CategoryGestureMapping> CategoryKeyGesture { get; }

        public GestureCollectionChangedEventArgs(GestureCollectionChangedType type, IEnumerable<CategoryGestureMapping> list)
        {
            Type = type;
            CategoryKeyGesture = new List<CategoryGestureMapping>(list);
        }

        public GestureCollectionChangedEventArgs(GestureCollectionChangedType type, CategoryGestureMapping categoryKeyGesture)
        {
            Type = type;
            CategoryKeyGesture = new List<CategoryGestureMapping> { categoryKeyGesture };
        }
    }
}