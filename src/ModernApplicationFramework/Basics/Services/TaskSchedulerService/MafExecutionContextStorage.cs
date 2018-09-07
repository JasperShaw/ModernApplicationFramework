using System;
using System.Collections.Generic;

namespace ModernApplicationFramework.Basics.Services.TaskSchedulerService
{
    internal class MafExecutionContextStorage
    {
        private static readonly MafExecutionContextStorage EmptyContext = new MafExecutionContextStorage();
        private readonly List<Tuple<Guid, Guid>> _elements;

        public MafExecutionContextStorage PreviousContext { get; }

        public bool IsEmpty => _elements == null;

        public bool IsNoFlowContext { get; set; }

        private MafExecutionContextStorage()
        {
            PreviousContext = null;
        }

        private MafExecutionContextStorage(MafExecutionContextStorage previousContext, List<Tuple<Guid, Guid>> elements)
        {
            PreviousContext = previousContext;
            _elements = elements;
        }

        public MafExecutionContextStorage(MafExecutionContextStorage previousContext, MafExecutionContextStorage newContext)
        {
            _elements = newContext._elements;
            PreviousContext = previousContext;
        }

        public Guid GetElement(Guid elementType)
        {
            if (_elements == null)
                return Guid.Empty;
            foreach (var element in _elements)
            {
                if (element.Item1 == elementType)
                    return element.Item2;
            }
            return Guid.Empty;
        }

        public MafExecutionContextStorage UpdateElement(Guid elementType, Guid value, out Guid previousValue)
        {
            GetElement(elementType);
            previousValue = Guid.Empty;
            if (value == Guid.Empty && (_elements == null || _elements.Count == 1))
                return EmptyContext;
            var elements = new List<Tuple<Guid, Guid>>();
            if (value != Guid.Empty)
                elements.Add(Tuple.Create(elementType, value));
            if (_elements != null)
            {
                foreach (var element in _elements)
                {
                    if (element.Item1 != elementType)
                        elements.Add(element);
                    else
                        previousValue = element.Item2;
                }
            }
            return new MafExecutionContextStorage(PreviousContext, elements);
        }

        public static MafExecutionContextStorage GetEmptyContext()
        {
            return EmptyContext;
        }
    }
}