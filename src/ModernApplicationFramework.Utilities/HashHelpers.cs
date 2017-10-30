namespace ModernApplicationFramework.Utilities
{
    public static class HashHelpers
    {
        public static int GetStableHashCode(string s, bool ignoreCase = false)
        {
            if (s == null)
                return 0;
            int num1 = 5381;
            int num2 = num1;
            int length = s.Length;
            int index1;
            for (int index2 = 0; index2 < length; index2 = index1 + 1)
            {
                char ch1 = ignoreCase ? char.ToLowerInvariant(s[index2]) : s[index2];
                num1 = (num1 << 5) + num1 ^ ch1;
                if ((index1 = index2 + 1) != length)
                {
                    char ch2 = ignoreCase ? char.ToLowerInvariant(s[index1]) : s[index1];
                    num2 = (num2 << 5) + num2 ^ ch2;
                }
                else
                    break;
            }
            return num1 + num2 * 1566083941;
        }

        public static int CombineHashes(int hash1, int hash2)
        {
            return BitRotator.RotateLeft(hash1, 5) ^ hash2 * 1566083941;
        }
    }
}
