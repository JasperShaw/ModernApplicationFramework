using System;

namespace ModernApplicationFramework.TextEditor
{
    public abstract class EditorOptionDefinition<T> : EditorOptionDefinition
    {
        public sealed override Type ValueType => typeof(T);

        public sealed override string Name => Key.Name;

        public sealed override object DefaultValue => Default;

        public sealed override bool IsValid(ref object proposedValue)
        {
            if (!(proposedValue is T))
                return false;
            T proposedValue1 = (T)proposedValue;
            int num = IsValid(ref proposedValue1) ? 1 : 0;
            proposedValue = proposedValue1;
            return num != 0;
        }

        public abstract EditorOptionKey<T> Key { get; }

        public virtual T Default => default(T);

        public virtual bool IsValid(ref T proposedValue)
        {
            return true;
        }
    }

    public abstract class EditorOptionDefinition
    {
        public abstract object DefaultValue { get; }

        public abstract Type ValueType { get; }

        public abstract string Name { get; }

        public virtual bool IsApplicableToScope(IPropertyOwner scope)
        {
            return true;
        }

        public virtual bool IsValid(ref object proposedValue)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is EditorOptionDefinition optionDefinition)
                return optionDefinition.Name == Name;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}