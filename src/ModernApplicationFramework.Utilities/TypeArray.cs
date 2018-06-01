using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities
{
    public class TypeArray<T>
    {
        public bool IncludeObjectType { get; }

        public IList<Type> Memebers { get; }

        public TypeArray(IEnumerable<Type> input, bool includeObjectType = false)
        {
            IncludeObjectType = includeObjectType;
            Memebers = new List<Type>();
            foreach (var type in input)
            {
                if (IsSubclassOfRawGeneric(typeof(T), type))
                    Memebers.Add(type);
            }
        }

        bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            if (toCheck == generic)
                return true;
            if (toCheck == typeof(object) && IncludeObjectType)
                return true;

            if (generic.IsAssignableFrom(toCheck))
                return true;
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}