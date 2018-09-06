using System;

namespace ModernApplicationFramework.Threading
{
    public class AsyncLocal<T> where T : class
    {
        private readonly AsyncLocalBase _asyncLocal;

        public T Value
        {
            get => _asyncLocal.Value;
            set => _asyncLocal.Value = value;
        }

        public AsyncLocal()
        {
            if (LightUps<T>.IsAsyncLocalSupported)
                _asyncLocal = new AsyncLocal46();
            else
                throw new NotSupportedException();
        }

        private abstract class AsyncLocalBase
        {
            public abstract T Value { get; set; }
        }

        private class AsyncLocal46 : AsyncLocalBase
        {
            private readonly object _asyncLocal;

            public override T Value
            {
                get => LightUps<T>.GetAsyncLocalValue(_asyncLocal);
                set => LightUps<T>.SetAsyncLocalValue(_asyncLocal, value);
            }

            public AsyncLocal46()
            {
                _asyncLocal = LightUps<T>.CreateAsyncLocal();
            }
        }
    }
}
