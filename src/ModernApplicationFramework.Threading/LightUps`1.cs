using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Threading
{
    internal static class LightUps<T>
    {
        internal static readonly Func<TaskCompletionSource<T>, CancellationToken, bool> TrySetCanceled;
        internal static readonly bool IsAsyncLocalSupported;
        private static readonly Type BclAsyncLocalType;
        private static readonly PropertyInfo BclAsyncLocalValueProperty;

        static LightUps()
        {
            var methodInfo = typeof(TaskCompletionSource<T>).GetTypeInfo()
                .GetDeclaredMethods(nameof(TaskCompletionSource<int>.TrySetCanceled))
                .FirstOrDefault(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(CancellationToken));
            if (methodInfo != null)
            {
                TrySetCanceled = (Func<TaskCompletionSource<T>, CancellationToken, bool>)methodInfo.CreateDelegate(typeof(Func<TaskCompletionSource<T>, CancellationToken, bool>));
            }

            if (LightUps.BclAsyncLocalType == null)
                return;
            BclAsyncLocalType = LightUps.BclAsyncLocalType.MakeGenericType(typeof(T));
            BclAsyncLocalValueProperty = BclAsyncLocalType.GetTypeInfo().GetDeclaredProperty("Value");
            IsAsyncLocalSupported = true;
        }

        internal static object CreateAsyncLocal()
        {
            if (!IsAsyncLocalSupported)
                throw new Exception();
            return AsyncLocalHelper.Instance.CreateAsyncLocal();
        }

        internal static void SetAsyncLocalValue(object instance, T value) => AsyncLocalHelper.Instance.Setter(instance, value);

        internal static T GetAsyncLocalValue(object instance) => AsyncLocalHelper.Instance.Getter(instance);

        private abstract class AsyncLocalHelper
        {
            internal static readonly AsyncLocalHelper Instance = CreateNew();

            internal abstract Func<object, T> Getter { get; }

            internal abstract Action<object, T> Setter { get; }

            internal abstract object CreateAsyncLocal();

            private static AsyncLocalHelper CreateNew()
            {
                var genericHelperType = typeof(AsyncLocalHelper<>);
                var instanceHelperType = genericHelperType.MakeGenericType(typeof(T), BclAsyncLocalType);
                return (AsyncLocalHelper) Activator.CreateInstance(instanceHelperType);
            }
        }

        private class AsyncLocalHelper<TAsyncLocal> : AsyncLocalHelper where TAsyncLocal : new()
        {
            internal override Func<object, T> Getter { get; }

            internal override Action<object, T> Setter { get; }

            public AsyncLocalHelper()
            {
                var getter = (Func<TAsyncLocal, T>)BclAsyncLocalValueProperty.GetMethod.CreateDelegate(typeof(Func<TAsyncLocal, T>));
                Getter = o => getter((TAsyncLocal)o);

                var setter = (Action<TAsyncLocal, T>)BclAsyncLocalValueProperty.SetMethod.CreateDelegate(typeof(Action<TAsyncLocal, T>));
                Setter = (o, v) => setter((TAsyncLocal)o, v);
            }

            internal override object CreateAsyncLocal() => new TAsyncLocal();
        }
    }
}
