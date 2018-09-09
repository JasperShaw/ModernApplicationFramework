using System;
using System.Threading.Tasks;
using ModernApplicationFramework.Threading;

namespace ModernApplicationFramework.Text.Utilities
{
    public class JoinableTaskHelper
    {
        public readonly JoinableTaskContext Context;
        public readonly JoinableTaskCollection Collection;
        public readonly JoinableTaskFactory Factory;

        public JoinableTaskHelper(JoinableTaskContext context)
        {
            var joinableTaskContext = context;
            Context = joinableTaskContext ?? throw new ArgumentNullException(nameof(context));
            Collection = context.CreateCollection();
            Factory = context.CreateFactory(Collection);
        }

        public JoinableTask RunOnUiThread(Action action, bool forceTaskSwitch = true)
        {
            using (Context.SuppressRelevance())
                return Factory.RunAsync(async () =>
                {
                    if (forceTaskSwitch && Context.IsOnMainThread)
                        await Task.Yield();
                    await Factory.SwitchToMainThreadAsync();
                    action();
                });
        }

        public JoinableTask<T> RunOnUiThread<T>(Func<T> function, bool forceTaskSwitch = true)
        {
            using (Context.SuppressRelevance())
                return Factory.RunAsync(async () =>
                {
                    if (forceTaskSwitch && Context.IsOnMainThread)
                        await Task.Yield();
                    await Factory.SwitchToMainThreadAsync();
                    return function();
                });
        }

        public Task DisposeAsync()
        {
            return Collection.JoinTillEmptyAsync();
        }

        public void Dispose()
        {
            Context.Factory.Run(async () => await DisposeAsync().ConfigureAwait(false));
        }
    }
}
