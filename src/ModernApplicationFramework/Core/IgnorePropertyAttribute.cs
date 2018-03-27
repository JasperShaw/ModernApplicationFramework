using System;

namespace ModernApplicationFramework.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {
        public static readonly IgnorePropertyAttribute Default = new IgnorePropertyAttribute();

        public IgnorePropertyAttribute(bool value)
        {
            IgnorePropertyValue = value;
        }

        public IgnorePropertyAttribute() : this(false)
        {
            
        }

        public virtual bool IgnoreProperty => IgnorePropertyValue;

        protected bool IgnorePropertyValue { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            return obj is IgnorePropertyAttribute other && other.IgnoreProperty == IgnoreProperty;
        }

        public override int GetHashCode()
        {
            return IgnoreProperty.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }
    }
}
