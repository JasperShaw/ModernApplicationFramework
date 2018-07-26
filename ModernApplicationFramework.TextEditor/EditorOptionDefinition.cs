using System;

namespace ModernApplicationFramework.TextEditor
{
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