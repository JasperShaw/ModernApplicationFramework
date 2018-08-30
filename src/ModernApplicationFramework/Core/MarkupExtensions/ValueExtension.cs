using System;
using System.Windows.Markup;

namespace ModernApplicationFramework.Core.MarkupExtensions
{
    public abstract class ValueExtension<T> : MarkupExtension
    {
        public T Value { get; set; }

        protected ValueExtension()
        {
            
        }

        protected ValueExtension(T value)
        {
            Value = value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
