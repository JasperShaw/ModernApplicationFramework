using System;

namespace ModernApplicationFramework.Utilities.Attributes
{
    public class OrderAttribute : MultipleBaseMetadataAttribute
    {
        private string _before;
        private string _after;

        public string Before
        {
            get => _before;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                    throw new ArgumentException();
                _before = value;
            }
        }

        public string After
        {
            get => _after;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                    throw new ArgumentException();
                _after = value;
            }
        }
    }
}
