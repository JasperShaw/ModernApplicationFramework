using System.Diagnostics.Tracing;

namespace ModernApplicationFramework.Threading
{
    internal sealed class ThreadingEventSource : EventSource
    {
        private const int ReaderWriterLockIssuedLockCountsEvent = 1;
        private const int WaitReaderWriterLockStartEvent = 2;
        private const int WaitReaderWriterLockStopEvent = 3;
        private const int CompleteOnCurrentThreadStartEvent = 11;
        private const int CompleteOnCurrentThreadStopEvent = 12;
        private const int WaitSynchronouslyStartEvent = 13;
        private const int WaitSynchronouslyStopEvent = 14;
        private const int PostExecutionStartEvent = 15;
        private const int PostExecutionStopEvent = 16;

        internal static readonly ThreadingEventSource Instance = new ThreadingEventSource();

        [Event(ReaderWriterLockIssuedLockCountsEvent, Task = Tasks.LockRequest, Opcode = Opcodes.ReaderWriterLockIssued)]
        public void ReaderWriterLockIssued(int lockId, AsyncReaderWriterLock.LockKind kind, int issuedUpgradeableReadCount, int issuedReadCount)
        {
            WriteEvent(ReaderWriterLockIssuedLockCountsEvent, lockId, (int)kind, issuedUpgradeableReadCount, issuedReadCount);
        }

        [Event(WaitReaderWriterLockStartEvent, Task = Tasks.LockRequestContention, Opcode = EventOpcode.Start)]
        public void WaitReaderWriterLockStart(int lockId, AsyncReaderWriterLock.LockKind kind, int issuedWriteCount, int issuedUpgradeableReadCount, int issuedReadCount)
        {
            this.WriteEvent(WaitReaderWriterLockStartEvent, lockId, kind, issuedWriteCount, issuedUpgradeableReadCount, issuedReadCount);
        }

        [Event(WaitReaderWriterLockStopEvent, Task = Tasks.LockRequestContention, Opcode = EventOpcode.Stop)]
        public void WaitReaderWriterLockStop(int lockId, AsyncReaderWriterLock.LockKind kind)
        {
            WriteEvent(WaitReaderWriterLockStopEvent, lockId, (int)kind);
        }

        [Event(CompleteOnCurrentThreadStartEvent)]
        public void CompleteOnCurrentThreadStart(int taskId, bool isOnMainThread)
        {
            WriteEvent(CompleteOnCurrentThreadStartEvent, taskId, isOnMainThread);
        }

        [Event(CompleteOnCurrentThreadStopEvent)]
        public void CompleteOnCurrentThreadStop(int taskId)
        {
            WriteEvent(CompleteOnCurrentThreadStopEvent, taskId);
        }

        [Event(WaitSynchronouslyStartEvent, Level = EventLevel.Verbose)]
        public void WaitSynchronouslyStart()
        {
            WriteEvent(WaitSynchronouslyStartEvent);
        }

        [Event(WaitSynchronouslyStopEvent, Level = EventLevel.Verbose)]
        public void WaitSynchronouslyStop()
        {
            WriteEvent(WaitSynchronouslyStopEvent);
        }

        [Event(PostExecutionStartEvent, Level = EventLevel.Verbose)]
        public void PostExecutionStart(int requestId, bool mainThreadAffinitized)
        {
            WriteEvent(PostExecutionStartEvent, requestId, mainThreadAffinitized);
        }

        [Event(PostExecutionStopEvent, Level = EventLevel.Verbose)]
        public void PostExecutionStop(int requestId)
        {
            WriteEvent(PostExecutionStopEvent, requestId);
        }

        public static class Tasks
        {
            public const EventTask LockRequest = (EventTask)1;
            public const EventTask LockRequestContention = (EventTask)2;
        }

        public static class Opcodes
        {
            public const EventOpcode ReaderWriterLockWaiting = (EventOpcode)100;
            public const EventOpcode ReaderWriterLockIssued = (EventOpcode)101;
            public const EventOpcode ReaderWriterLockIssuedAfterContention = (EventOpcode)102;
        }
    }
}
