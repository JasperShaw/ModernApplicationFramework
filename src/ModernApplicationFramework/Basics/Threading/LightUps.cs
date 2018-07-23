using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ModernApplicationFramework.Basics.Threading
{
    internal static class LightUps<T>
    {
        internal static readonly Func<TaskCompletionSource<T>, CancellationToken, bool> TrySetCanceled;
        internal static readonly bool IsAsyncLocalSupported;
        private static readonly Type BclAsyncLocalType;
        private static readonly PropertyInfo BclAsyncLocalValueProperty;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "These fields have dependency relationships.")]
        static LightUps()
        {
            MethodInfo methodInfo = typeof(TaskCompletionSource<T>).GetTypeInfo().GetDeclaredMethods(nameof(TrySetCanceled)).FirstOrDefault<MethodInfo>(m =>
            {
                if (m.GetParameters().Length == 1)
                    return m.GetParameters()[0].ParameterType == typeof(CancellationToken);
                return false;
            });
            if (methodInfo != null)
                TrySetCanceled = (Func<TaskCompletionSource<T>, CancellationToken, bool>)methodInfo.CreateDelegate(typeof(Func<TaskCompletionSource<T>, CancellationToken, bool>));
            if (!(Type.GetType("System.Threading.AsyncLocal`1") != null))
                return;
            BclAsyncLocalType = (Type.GetType("System.Threading.AsyncLocal`1")?.MakeGenericType(typeof(T)));
            BclAsyncLocalValueProperty = BclAsyncLocalType.GetTypeInfo().GetDeclaredProperty("Value");
            IsAsyncLocalSupported = true;
        }

        internal static object CreateAsyncLocal()
        {
            return AsyncLocalHelper.Instance.CreateAsyncLocal();
        }

        internal static void SetAsyncLocalValue(object instance, T value)
        {
            AsyncLocalHelper.Instance.Setter(instance, value);
        }

        internal static T GetAsyncLocalValue(object instance)
        {
            return AsyncLocalHelper.Instance.Getter(instance);
        }

        private abstract class AsyncLocalHelper
        {
            internal static readonly AsyncLocalHelper Instance = CreateNew();

            internal abstract Func<object, T> Getter { get; }

            internal abstract Action<object, T> Setter { get; }

            internal abstract object CreateAsyncLocal();

            private static AsyncLocalHelper CreateNew()
            {
                return (AsyncLocalHelper)Activator.CreateInstance(typeof(LightUps<>.AsyncLocalHelper<>).MakeGenericType(typeof(T), BclAsyncLocalType));
            }
        }

        private class AsyncLocalHelper<TAsyncLocal> : AsyncLocalHelper where TAsyncLocal : new()
        {
            public AsyncLocalHelper()
            {
                Func<TAsyncLocal, T> getter = (Func<TAsyncLocal, T>)BclAsyncLocalValueProperty.GetMethod.CreateDelegate(typeof(Func<TAsyncLocal, T>));
                Getter = o => getter((TAsyncLocal)o);
                Action<TAsyncLocal, T> setter = (Action<TAsyncLocal, T>)BclAsyncLocalValueProperty.SetMethod.CreateDelegate(typeof(Action<TAsyncLocal, T>));
                Setter = (o, v) => setter((TAsyncLocal)o, v);
            }

            internal override Func<object, T> Getter { get; }

            internal override Action<object, T> Setter { get; }

            internal override object CreateAsyncLocal()
            {
                return Activator.CreateInstance<TAsyncLocal>();
            }
        }
    }
}
