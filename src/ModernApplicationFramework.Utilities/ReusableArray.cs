namespace ModernApplicationFramework.Utilities
{
    public class ReusableArray<T> : ReusableResourceStore<T[], int>
    {
        private readonly bool _requiresExactSize;
        private readonly int _maximumCacheArrayLength;

        public ReusableArray(bool requiresExactSize, int maximumCacheArrayLength = 2147483647)
        {
            Validate.IsWithinRange(maximumCacheArrayLength, 1, int.MaxValue, "maximumCacheArrayLength");
            _requiresExactSize = requiresExactSize;
            _maximumCacheArrayLength = maximumCacheArrayLength;
        }

        protected override T[] Allocate(int constructorParameter)
        {
            return new T[constructorParameter];
        }

        protected override bool CanReuse(T[] value, int parameter)
        {
            if (_requiresExactSize)
                return value.Length == parameter;
            return value.Length >= parameter;
        }

        protected override bool Cleanup(T[] value)
        {
            return value.Length <= _maximumCacheArrayLength;
        }
    }
}
