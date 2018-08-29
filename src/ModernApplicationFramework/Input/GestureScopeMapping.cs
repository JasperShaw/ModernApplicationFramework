using System;
using System.Globalization;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Input
{
    public class GestureScopeMapping : IEquatable<GestureScopeMapping>
    {
        private static readonly MultiKeyGestureConverter Converter = new MultiKeyGestureConverter();
        
        public GestureScopeMapping(GestureScope scope, MultiKeyGesture keyGesture)
        {
            Scope = scope;
            KeyGesture = keyGesture;
        }

        public GestureScope Scope { get; }

        public MultiKeyGesture KeyGesture { get; }

        public string Text => $"{Converter.ConvertTo(null, CultureInfo.CurrentCulture, KeyGesture, typeof(string))} ({Scope.Text})";

        public bool Equals(GestureScopeMapping other)
        {
            return other != null && Scope.Equals(other.Scope) && KeyGesture.Equals(other.KeyGesture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is GestureScopeMapping a && Equals(a);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Scope != null ? Scope.GetHashCode() : 0) * 397) ^ (KeyGesture != null ? KeyGesture.GetHashCode() : 0);
            }
        }
    }
}