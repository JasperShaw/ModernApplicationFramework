namespace ModernApplicationFramework.Core.Utilities
{
    public static class BitRotator
    {
        public static sbyte RotateLeft(sbyte value, int count)
        {
            return (sbyte)RotateLeft((byte)value, count);
        }

        public static byte RotateLeft(byte value, int count)
        {
            if (count < 0)
                return RotateRight(value, -count);
            count %= 8;
            return (byte)(value << count | value >> 8 - count);
        }

        public static sbyte RotateRight(sbyte value, int count)
        {
            return (sbyte)RotateRight((byte)value, count);
        }

        public static byte RotateRight(byte value, int count)
        {
            if (count < 0)
                return RotateLeft(value, -count);
            count %= 8;
            return (byte)(value >> count | value << 8 - count);
        }

        public static short RotateLeft(short value, int count)
        {
            return (short)RotateLeft((ushort)value, count);
        }

        public static ushort RotateLeft(ushort value, int count)
        {
            if (count < 0)
                return RotateRight(value, -count);
            count %= 16;
            return (ushort)(value << count | value >> 16 - count);
        }

        public static short RotateRight(short value, int count)
        {
            return (short)RotateRight((ushort)value, count);
        }

        public static ushort RotateRight(ushort value, int count)
        {
            if (count < 0)
                return RotateLeft(value, -count);
            count %= 16;
            return (ushort)(value >> count | value << 16 - count);
        }

        public static int RotateLeft(int value, int count)
        {
            return (int)RotateLeft((uint)value, count);
        }

        public static uint RotateLeft(uint value, int count)
        {
            return value << count | value >> 32 - count;
        }

        public static int RotateRight(int value, int count)
        {
            return (int)RotateRight((uint)value, count);
        }

        public static uint RotateRight(uint value, int count)
        {
            return value >> count | value << 32 - count;
        }

        public static long RotateLeft(long value, int count)
        {
            return (long)RotateLeft((ulong)value, count);
        }

        public static ulong RotateLeft(ulong value, int count)
        {
            return value << count | value >> 64 - count;
        }

        public static long RotateRight(long value, int count)
        {
            return (long)RotateRight((ulong)value, count);
        }

        public static ulong RotateRight(ulong value, int count)
        {
            return value >> count | value << 64 - count;
        }
    }
}
