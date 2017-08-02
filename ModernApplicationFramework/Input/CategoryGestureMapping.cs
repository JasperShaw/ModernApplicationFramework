using System;
using System.Globalization;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Input
{
    public class CategoryGestureMapping : IEquatable<CategoryGestureMapping>
    {
        private static readonly MultiKeyGestureConverter Converter = new MultiKeyGestureConverter();
        
        public CategoryGestureMapping(CommandGestureCategory category, MultiKeyGesture keyGesture)
        {
            Category = category;
            KeyGesture = keyGesture;
        }

        public CommandGestureCategory Category { get; }

        public MultiKeyGesture KeyGesture { get; }

        public string Text => $"{Converter.ConvertTo(null, CultureInfo.CurrentCulture, KeyGesture, typeof(string))} ({Category.Name})";

        public bool Equals(CategoryGestureMapping other)
        {
            return other != null && Equals(Category, other.Category) && Equals(KeyGesture, other.KeyGesture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            var a = obj as CategoryGestureMapping;
            return a != null && Equals(a);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Category != null ? Category.GetHashCode() : 0) * 397) ^ (KeyGesture != null ? KeyGesture.GetHashCode() : 0);
            }
        }
    }
}