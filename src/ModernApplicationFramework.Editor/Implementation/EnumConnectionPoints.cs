using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace ModernApplicationFramework.Editor.Implementation
{
    internal class EnumConnectionPoints : IEnumConnectionPoints
    {
        private readonly Dictionary<Type, IConnectionPoint> _connectionPoints;
        private Dictionary<Type, IConnectionPoint>.Enumerator _connectionPointEnumerator;

        internal EnumConnectionPoints(Dictionary<Type, IConnectionPoint> eventTypeToConnectionPointMap)
        {
            _connectionPoints = eventTypeToConnectionPointMap;
            _connectionPointEnumerator = eventTypeToConnectionPointMap.GetEnumerator();
        }

        public int Next(int cConnectionPoints, IConnectionPoint[] ppCp, IntPtr pcFetched)
        {
            int num1 = -2147024809;
            if (cConnectionPoints > 0U && ppCp != null)
            {
                uint num2 = 0;
                if (cConnectionPoints > (long)ppCp.Length)
                {
                    cConnectionPoints = ppCp.Length;
                }
                else
                {
                    num1 = 0;
                    for (; num2 < cConnectionPoints; ++num2)
                    {
                        if (!_connectionPointEnumerator.MoveNext())
                        {
                            num1 = 1;
                            break;
                        }
                        ppCp[(int)num2] = _connectionPointEnumerator.Current.Value;
                    }
                }
                for (; num2 < cConnectionPoints; ++num2)
                    ppCp[(int)num2] = null;
            }
            return num1;
        }

        public int Skip(int cConnectionPoints)
        {
            if (cConnectionPoints == 0U)
                return -2147024809;
            for (uint index = 0; index < cConnectionPoints; ++index)
            {
                if (!_connectionPointEnumerator.MoveNext())
                    return 1;
            }
            return 0;
        }

        public void Reset()
        {
            _connectionPointEnumerator = _connectionPoints.GetEnumerator();
        }

        public void Clone(out IEnumConnectionPoints ppEnum)
        {
            ppEnum = new EnumConnectionPoints(_connectionPoints);
        }
    }
}