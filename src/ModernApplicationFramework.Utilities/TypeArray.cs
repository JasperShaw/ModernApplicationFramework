using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Utilities
{
    /// <summary>
    /// Special array type that holds only types comptabile to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T"/>
    [Serializable]
    public class TypeArray<T>
    {
        /// <summary>
        /// Value indicating whether the type <see cref="Object"/> shall be inclued as compatible type.
        /// </summary>
        public bool IncludeObjectType { get; }

        /// <summary>
        /// Members of this list.
        /// </summary>
        public IList<Type> Memebers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArray{T}"/> class.
        /// </summary>
        /// <param name="input">Collection of types from which this instance shall be initialized from</param>
        /// <param name="includeObjectType">if set to <see langword="true"/> the instance will contain the <see cref="Object"/> type as a compatible type.</param>
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

        private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
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