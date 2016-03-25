/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class FullWeakDictionary<TK, TV> where TK : class
    {
        private readonly List<WeakReference> _keys = new List<WeakReference>();
        private readonly List<WeakReference> _values = new List<WeakReference>();

        public TV this[TK key]
        {
            get
            {
                TV valueToReturn;
                if (!GetValue(key, out valueToReturn))
                    throw new ArgumentException();
                return valueToReturn;
            }
            set { SetValue(key, value); }
        }

        public bool ContainsKey(TK key)
        {
            CollectGarbage();
            return -1 != _keys.FindIndex(k => k.GetValueOrDefault<TK>() == key);
        }

        public bool GetValue(TK key, out TV value)
        {
            CollectGarbage();
            int vIndex = _keys.FindIndex(k => k.GetValueOrDefault<TK>() == key);
            value = default(TV);
            if (vIndex == -1)
                return false;
            value = _values[vIndex].GetValueOrDefault<TV>();
            return true;
        }

        public void SetValue(TK key, TV value)
        {
            CollectGarbage();
            int vIndex = _keys.FindIndex(k => k.GetValueOrDefault<TK>() == key);
            if (vIndex > -1)
                _values[vIndex] = new WeakReference(value);
            else
            {
                _values.Add(new WeakReference(value));
                _keys.Add(new WeakReference(key));
            }
        }

        private void CollectGarbage()
        {
            int vIndex = 0;

            do
            {
                vIndex = _keys.FindIndex(vIndex, k => !k.IsAlive);
                if (vIndex >= 0)
                {
                    _keys.RemoveAt(vIndex);
                    _values.RemoveAt(vIndex);
                }
            } while (vIndex >= 0);

            vIndex = 0;
            do
            {
                vIndex = _values.FindIndex(vIndex, v => !v.IsAlive);
                if (vIndex >= 0)
                {
                    _values.RemoveAt(vIndex);
                    _keys.RemoveAt(vIndex);
                }
            } while (vIndex >= 0);
        }
    }
}