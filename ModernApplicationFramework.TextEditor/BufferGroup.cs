using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ModernApplicationFramework.TextEditor
{
    internal class BufferGroup
    {
        internal Queue<Tuple<BaseBuffer.ITextEventRaiser, BaseBuffer>> EventQueue =
            new Queue<Tuple<BaseBuffer.ITextEventRaiser, BaseBuffer>>();


        internal int Depth;
        internal bool EventingInProgress;
        private BaseBuffer _masterBuffer;
        private Dictionary<BaseBuffer, GraphEntry> _graph;
        private Dictionary<ITextBuffer, ISubordinateTextEdit> _buffer2EditMap;
        private HashSet<BaseProjectionBuffer> _pendingIndependentBuffers;
        private EditOptions _masterOptions;
        private object _masterEditTag;

        public BufferGroup(ITextBuffer member)
        {
            Members.Add(new BufferWeakReference(member));
        }

        internal HashSet<BufferWeakReference> Members { get; } = new HashSet<BufferWeakReference>();

        public bool MembersContains(ITextBuffer buffer)
        {
            return Members.Contains(new BufferWeakReference(buffer));
        }

        public void AddMember(ITextBuffer member)
        {
            Members.RemoveWhere(m => m.Buffer == null);
            Members.Add(new BufferWeakReference(member));
        }

        public void RemoveMember(ITextBuffer member)
        {
            Members.Remove(new BufferWeakReference(member));
        }

        public void Swallow(BufferGroup victim)
        {
            if (victim == this)
                return;
            foreach (var member in victim.Members)
            {
                ITextBuffer buffer = member.Buffer;
                if (buffer != null && Members.Add(member))
                    ((BaseBuffer) buffer).Group = this;
            }

            if (victim.EventQueue.Count <= 0)
                return;
            var tupleQueue =
                new Queue<Tuple<BaseBuffer.ITextEventRaiser, BaseBuffer>>(
                    victim.EventQueue);
            while (EventQueue.Count > 0)
                tupleQueue.Enqueue(EventQueue.Dequeue());
            EventQueue = tupleQueue;
            victim.EventQueue.Clear();
        }

        public ITextBuffer MasterBuffer => _masterBuffer;

        public bool MasterEditInProgress => _masterBuffer != null;

        public static bool Tracing { get; set; }

        public static bool DetailedTracing { get; set; }

        public Dictionary<ITextBuffer, ISubordinateTextEdit> BufferToEditMap
        {
            get
            {
                if (_buffer2EditMap == null)
                    throw new InvalidOperationException();
                return _buffer2EditMap;
            }
        }

        public ITextEdit GetEdit(BaseBuffer buffer)
        {
            return GetEdit(buffer, _masterOptions);
        }

        public ITextEdit GetEdit(BaseBuffer buffer, EditOptions options)
        {
            if (!_buffer2EditMap.TryGetValue(buffer, out var subordinateEdit))
            {
                subordinateEdit = buffer.CreateSubordinateEdit(options, new int?(), _masterEditTag);
                _buffer2EditMap.Add(buffer, subordinateEdit);
            }

            return (ITextEdit) subordinateEdit;
        }

        public void PerformMasterEdit(ITextBuffer buffer, ISubordinateTextEdit xedit, EditOptions options,
            object editTag)
        {
            if (_masterBuffer != null)
                throw new InvalidOperationException("Master edit already in progress");
            _masterBuffer = (BaseBuffer) buffer;
            _masterOptions = options;
            _masterEditTag = editTag;
            _buffer2EditMap = new Dictionary<ITextBuffer, ISubordinateTextEdit> {{buffer, xedit}};
            _pendingIndependentBuffers = new HashSet<BaseProjectionBuffer>();
            BuildGraph();
            var appliedSubordinateEdits = new Stack<ISubordinateTextEdit>();
            while (_buffer2EditMap.Count > 0)
            {
                var subordinateTextEdit = PickEdit();
                PopulateSourceEdits(subordinateTextEdit.TextBuffer);
                subordinateTextEdit.PreApply();
                appliedSubordinateEdits.Push(subordinateTextEdit);
            }

            void CancelAction()
            {
                foreach (var subordinateTextEdit in appliedSubordinateEdits) subordinateTextEdit.CancelApplication();
                _graph = null;
                _buffer2EditMap = null;
                _masterBuffer = null;
                _pendingIndependentBuffers = null;
            }

            foreach (var subordinateTextEdit in appliedSubordinateEdits)
            {
                if (!subordinateTextEdit.CheckForCancellation(CancelAction))
                    return;
            }

            while (appliedSubordinateEdits.Count > 0)
                appliedSubordinateEdits.Pop().FinalApply();
            while (_pendingIndependentBuffers.Count > 0)
            {
                var projectionBuffer = PickIndependentBuffer();
                EventQueue.Enqueue(new Tuple<BaseBuffer.ITextEventRaiser, BaseBuffer>(
                    projectionBuffer.PropagateSourceChanges(options, editTag), projectionBuffer));
            }

            _graph = null;
            _buffer2EditMap = null;
            _masterBuffer = null;
            _pendingIndependentBuffers = null;
        }

        public void ScheduleIndependentEdit(BaseProjectionBuffer projectionBuffer)
        {
            _pendingIndependentBuffers.Add(projectionBuffer);
        }

        public void CancelIndependentEdit(BaseProjectionBuffer projectionBuffer)
        {
            _pendingIndependentBuffers.Remove(projectionBuffer);
        }

        private void PopulateSourceEdits(ITextBuffer buffer)
        {
            var projectionBufferBase = buffer as IProjectionBufferBase;
            if (projectionBufferBase == null)
                return;
            foreach (var sourceBuffer in projectionBufferBase.SourceBuffers)
            {
                if (sourceBuffer is IProjectionBufferBase)
                {
                    var baseBuffer = (BaseBuffer) sourceBuffer;
                    if (!_buffer2EditMap.ContainsKey(baseBuffer))
                        _buffer2EditMap.Add(sourceBuffer,
                            baseBuffer.CreateSubordinateEdit(_masterOptions, new int?(), _masterEditTag));
                }
            }
        }

        private ISubordinateTextEdit PickEdit()
        {
            foreach (var buffer2Edit in _buffer2EditMap)
            {
                var key = (BaseBuffer) buffer2Edit.Key;
                if (!_graph.TryGetValue(key, out var graphEntry))
                {
                    _buffer2EditMap.Remove(key);
                    return buffer2Edit.Value;
                }

                if (InvulnerableToFutureEdits(graphEntry))
                {
                    graphEntry.EditComplete = true;
                    _buffer2EditMap.Remove(key);
                    return buffer2Edit.Value;
                }
            }

            throw new InvalidOperationException("Internal error in BufferGroup.PickEdit");
        }

        private bool InvulnerableToFutureEdits(GraphEntry graphEntry)
        {
            foreach (BaseBuffer target in graphEntry.Targets)
            {
                var graphEntry1 = _graph[target];
                if (!graphEntry1.EditComplete)
                {
                    if (!InvulnerableToFutureEdits(graphEntry1) ||
                        _buffer2EditMap.ContainsKey(target))
                        return false;
                    graphEntry1.EditComplete = true;
                }
            }

            return true;
        }

        private BaseProjectionBuffer PickIndependentBuffer()
        {
            foreach (var independentBuffer in _pendingIndependentBuffers)
            {
                var flag = true;
                foreach (BaseBuffer sourceBuffer in independentBuffer.SourceBuffers)
                {
                    if (!IsStableDuringIndependentPhase(sourceBuffer))
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    _graph[independentBuffer].Dependent = true;
                    _pendingIndependentBuffers.Remove(independentBuffer);
                    return independentBuffer;
                }
            }

            throw new InvalidOperationException("Couldn't pick an independent buffer");
        }

        private bool IsStableDuringIndependentPhase(BaseBuffer sourceBuffer)
        {
            if (sourceBuffer is BaseProjectionBuffer projectionBuffer && _pendingIndependentBuffers.Contains(projectionBuffer))
                return false;
            if (_graph.TryGetValue(sourceBuffer, out var graphEntry) && !graphEntry.Dependent)
                return !InTargetClosureOfBuffer(sourceBuffer, _masterBuffer);
            return true;
        }

        private bool InTargetClosureOfBuffer(BaseBuffer candidateBuffer, BaseBuffer governingBuffer)
        {
            foreach (var target in _graph[governingBuffer].Targets)
            {
                if (target == candidateBuffer || InTargetClosureOfBuffer(candidateBuffer, (BaseBuffer) target))
                    return true;
            }

            return false;
        }

        private void BuildGraph()
        {
            Members.RemoveWhere(member => member.Buffer == null);
            _graph = new Dictionary<BaseBuffer, GraphEntry>(Members.Count);
            foreach (var member in Members)
            {
                var buffer = member.Buffer;
                if (buffer != null)
                    _graph.Add(buffer,
                        new GraphEntry(new HashSet<IProjectionBufferBase>(), false, false));
            }

            foreach (var member in Members)
            {
                if (member.Buffer is IProjectionBufferBase buffer)
                {
                    foreach (BaseBuffer sourceBuffer in buffer.SourceBuffers)
                        _graph[sourceBuffer].Targets.Add(buffer);
                }
            }

            MarkMasterClosure(_masterBuffer);
        }

        private void MarkMasterClosure(BaseBuffer buffer)
        {
            var graphEntry = _graph[buffer];
            if (graphEntry.Dependent)
                return;
            graphEntry.Dependent = true;
            if (!(buffer is IProjectionBufferBase projectionBufferBase))
                return;
            foreach (BaseBuffer sourceBuffer in projectionBufferBase.SourceBuffers)
                MarkMasterClosure(sourceBuffer);
        }

        public string DumpGraph()
        {
            var stringBuilder = new StringBuilder("BufferGroup Graph");
            if (_graph == null)
            {
                stringBuilder.AppendLine(" <null>");
            }
            else
            {
                stringBuilder.AppendLine("");
                foreach (var keyValuePair in _graph)
                {
                    stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0,8} {1}: ",
                        TextUtilities.GetTag(keyValuePair.Key),
                        keyValuePair.Value.EditComplete ? "T" : "F"));
                    foreach (var target in keyValuePair.Value.Targets)
                        stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0,8},",
                            TextUtilities.GetTag(target)));
                    stringBuilder.Append("\r\n");
                }
            }

            return stringBuilder.ToString();
        }

        public void BeginEdit()
        {
            if (Depth < 0)
                throw new InvalidOperationException();
            ++Depth;
        }

        public void FinishEdit()
        {
            if (Depth <= 0)
                throw new InvalidOperationException();
            if (--Depth != 0)
                return;
            RaiseEvents();
        }

        public void CancelEdit()
        {
            FinishEdit();
        }

        public void EnqueueEvents(BaseBuffer.ITextEventRaiser raiser, BaseBuffer baseBuffer)
        {
            if (Depth <= 0)
                throw new InvalidOperationException();
            EventQueue.Enqueue(new Tuple<BaseBuffer.ITextEventRaiser, BaseBuffer>(raiser, baseBuffer));
        }

        public void EnqueueEvents(IEnumerable<BaseBuffer.ITextEventRaiser> raisers, BaseBuffer baseBuffer)
        {
            if (Depth <= 0)
                throw new InvalidOperationException();
            foreach (var raiser in raisers)
                EventQueue.Enqueue(new Tuple<BaseBuffer.ITextEventRaiser, BaseBuffer>(raiser, baseBuffer));
        }

        private void RaiseEvents()
        {
            if (EventingInProgress)
                return;
            var baseBufferList = new List<BaseBuffer>();
            try
            {
                EventingInProgress = true;
                while (EventQueue.Count > 0)
                {
                    var tuple = EventQueue.Dequeue();
                    tuple.Item1.RaiseEvent(tuple.Item2, false);
                    if (tuple.Item1.HasPostEvent)
                        baseBufferList.Add(tuple.Item2);
                }
            }
            finally
            {
                EventingInProgress = false;
            }

            foreach (var baseBuffer in baseBufferList)
                baseBuffer.RaisePostChangedEvent();
        }

        private class GraphEntry
        {
            public HashSet<IProjectionBufferBase> Targets { get; }

            public bool EditComplete { get; set; }

            public bool Dependent { get; set; }

            public GraphEntry(HashSet<IProjectionBufferBase> targets, bool editComplete, bool dependent)
            {
                Targets = targets;
                EditComplete = editComplete;
                Dependent = dependent;
            }
        }

        internal class BufferWeakReference
        {
            private readonly WeakReference<BaseBuffer> _buffer;
            private readonly int _hashCode;

            public BufferWeakReference(ITextBuffer buffer)
            {
                _buffer = new WeakReference<BaseBuffer>((BaseBuffer) buffer);
                _hashCode = buffer.GetHashCode();
            }

            public BaseBuffer Buffer
            {
                get
                {
                    if (_buffer.TryGetTarget(out var target))
                        return target;
                    return null;
                }
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                    return true;
                if (!(obj is BufferWeakReference bufferWeakReference))
                    return false;
                var buffer = Buffer;
                if (buffer == bufferWeakReference.Buffer)
                    return buffer != null;
                return false;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }
    }
}