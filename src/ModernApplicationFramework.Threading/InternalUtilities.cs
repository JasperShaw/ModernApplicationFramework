using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Threading
{
    internal static class InternalUtilities
    {
        private const string AsyncReturnStackPrefix = " -> ";

        internal static IEnumerable<string> GetAsyncReturnStackFrames(this Delegate continuationDelegate)
        {
            var stateMachine = FindAsyncStateMachine(continuationDelegate);
            if (stateMachine == null)
            {
                yield return GetDelegateLabel(continuationDelegate);
                yield break;
            }

            do
            {
                var state = GetStateMachineFieldValueOnSuffix(stateMachine, "__state");
                yield return string.Format(
                    CultureInfo.CurrentCulture,
                    "{2}{0} (state: {1}, address: 0x{3:X8})",
                    stateMachine.GetType().FullName,
                    state,
                    AsyncReturnStackPrefix,
                    (int)GetAddress(stateMachine));

                var continuationDelegates = FindContinuationDelegates(stateMachine).ToArray();
                if (continuationDelegates.Length == 0)
                {
                    break;
                }
                stateMachine = continuationDelegates.Select(FindAsyncStateMachine)
                    .FirstOrDefault((s) => s != null);
                if (stateMachine == null)
                {
                    yield return GetDelegateLabel(continuationDelegates.First());
                }
            } while (stateMachine != null);
        }

        private static string GetDelegateLabel(Delegate invokeDelegate)
        {
            Validate.IsNotNull(invokeDelegate, nameof(invokeDelegate));

            var method = invokeDelegate.GetMethodInfo();
            if (invokeDelegate.Target != null)
            {
                var instanceType = string.Empty;
                if (method.DeclaringType != null && method.DeclaringType != invokeDelegate.Target.GetType())
                {
                    instanceType = " (" + invokeDelegate.Target.GetType().FullName + ")";
                }

                return string.Format(
                    CultureInfo.CurrentCulture,
                    "{3}{0}.{1}{2} (target address: 0x{4:X8})",
                    method.DeclaringType.FullName,
                    method.Name,
                    instanceType,
                    AsyncReturnStackPrefix,
                    (int)GetAddress(invokeDelegate.Target));
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "{2}{0}.{1}",
                method.DeclaringType.FullName,
                method.Name,
                AsyncReturnStackPrefix);
        }

        private static IEnumerable<Delegate> FindContinuationDelegates(IAsyncStateMachine stateMachine)
        {
            Validate.IsNotNull(stateMachine, nameof(stateMachine));

            var builder = GetStateMachineFieldValueOnSuffix(stateMachine, "__builder");
            if (builder == null)
            {
                yield break;
            }

            var task = GetFieldValue(builder, "m_task");
            if (task == null)
            {
                builder = GetFieldValue(builder, "m_builder");
                if (builder != null)
                {
                    task = GetFieldValue(builder, "m_task");
                }
            }

            if (task == null)
            {
                yield break;
            }
            var continuationField = typeof(Task).GetTypeInfo().GetDeclaredField("m_continuationObject");
            if (continuationField == null)
            {
                yield break;
            }

            var continuationObject = continuationField.GetValue(task);
            if (continuationObject == null)
            {
                yield break;
            }

            if (continuationObject is IEnumerable items)
            {
                foreach (var item in items)
                {
                    var action = item as Delegate ?? GetFieldValue(item, "m_action") as Delegate;
                    if (action != null)
                    {
                        yield return action;
                    }
                }
            }
            else
            {
                var action = continuationObject as Delegate ?? GetFieldValue(continuationObject, "m_action") as Delegate;
                if (action != null)
                {
                    yield return action;
                }
            }
        }

        private static IAsyncStateMachine FindAsyncStateMachine(Delegate invokeDelegate)
        {
            Validate.IsNotNull(invokeDelegate, nameof(invokeDelegate));

            if (invokeDelegate.Target != null)
            {
                if (GetFieldValue(invokeDelegate.Target, "m_continuation") is Action continuation)
                {
                    invokeDelegate = continuation;
                }

                var stateMachine = GetFieldValue(invokeDelegate.Target, "m_stateMachine") as IAsyncStateMachine;
                return stateMachine;
            }

            return null;
        }

        private static object GetFieldValue(object obj, string fieldName)
        {
            Validate.IsNotNull(obj, nameof(obj));
            Validate.IsNotNullAndNotEmpty(fieldName, nameof(fieldName));

            var field = obj.GetType().GetTypeInfo().GetDeclaredField(fieldName);
            if (field != null)
            {
                return field.GetValue(obj);
            }

            return null;
        }

        private static IntPtr GetAddress(object value)
        {
            unsafe
            {
                var tr = __makeref(value);
                return **(IntPtr**)&tr;
            }
        }

        private static object GetStateMachineFieldValueOnSuffix(IAsyncStateMachine stateMachine, string suffix)
        {
            Validate.IsNotNull(stateMachine, nameof(stateMachine));
            Validate.IsNotNullAndNotEmpty(suffix, nameof(suffix));

            var fields = stateMachine.GetType().GetTypeInfo().DeclaredFields;
            var field = fields.FirstOrDefault((f) => f.Name.EndsWith(suffix, StringComparison.Ordinal));
            if (field != null)
            {
                return field.GetValue(stateMachine);
            }

            return null;
        }

        internal static bool RemoveMidQueue<T>(this Queue<T> queue, T valueToRemove)
            where T : class
        {
            Validate.IsNotNull(queue, nameof(queue));
            Validate.IsNotNull(valueToRemove, nameof(valueToRemove));

            var originalCount = queue.Count;
            var dequeueCounter = 0;
            var found = false;
            while (dequeueCounter < originalCount)
            {
                dequeueCounter++;
                var dequeued = queue.Dequeue();
                if (!found && dequeued == valueToRemove)
                {
                    found = true;
                }
                else
                {
                    queue.Enqueue(dequeued);
                }
            }

            return found;
        }
    }
}
