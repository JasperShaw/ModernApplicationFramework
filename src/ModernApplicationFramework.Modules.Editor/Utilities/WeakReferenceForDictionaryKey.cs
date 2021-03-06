﻿using System;
using System.Runtime.Serialization;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    [Serializable]
    public class WeakReferenceForDictionaryKey : WeakReference
    {
        private readonly int _hashCode;

        public WeakReferenceForDictionaryKey(object target)
            : base(target)
        {
            if (target == null)
                return;
            _hashCode = target.GetHashCode() ^ 52428;
        }

        protected WeakReferenceForDictionaryKey(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public override bool Equals(object obj)
        {
            var forDictionaryKey = obj as WeakReferenceForDictionaryKey;
            bool flag;
            if (forDictionaryKey == null)
            {
                flag = false;
            }
            else if (this == forDictionaryKey)
            {
                flag = true;
            }
            else
            {
                var objA = null as object;
                var objB = (object) null;
                try
                {
                    objA = Target;
                    objB = forDictionaryKey.Target;
                }
                catch (InvalidOperationException)
                {
                }

                flag = objA != null && objB != null && Equals(objA, objB);
            }

            return flag;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}