using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Basics
{
    public class ObservableTask<TResult> : ObservableObject
    {
        private readonly Task<TResult> _task;
        private TResult _result;

        private ObservableTask(TResult result)
        {
            _task = Task.FromResult(result);
            Result = result;
        }

        public ObservableTask(Task<TResult> task)
        {
            var observableTask1 = this;
            Validate.IsNotNull(task, nameof(task));
            _task = (Task<TResult>) Task.Run(async () =>
            {
                var observableTask2 = observableTask1;
                observableTask2.Result = await task;
            });
        }

        public ObservableTask(DispatcherOperation operation)
        {
            var observableTask = this;
            Validate.IsNotNull(operation, nameof(operation));
            _task = (Task<TResult>) Task.Run(async () =>
            {
                await operation;
                observableTask.Result = (TResult)operation.Result;
            });
        }

        public TResult Result
        {
            get => _result;
            set => SetProperty(ref _result, value, nameof(Result));
        }

        public static ObservableTask<TResult> FromResult(TResult result)
        {
            return new ObservableTask<TResult>(result);
        }

        public TaskAwaiter<TResult> GetAwaiter()
        {
            return _task.GetAwaiter();
        }
    }
}
