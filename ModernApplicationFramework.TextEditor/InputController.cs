using System;
using System.Collections.Generic;
using ModernApplicationFramework.TextEditor.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    internal sealed class InputController
    {
        private readonly InputControllerState _state;
        private readonly ITextView _textView;
        private readonly MasterMouseProcessor _masterMouseProcessor;
        internal IMouseProcessor[] ActiveMouseProcessors;
        private readonly KeyProcessorDispatcher _keyProcessorDispatcher;
        internal KeyProcessor[] ActiveKeyProcessors;
        private readonly InputControllerViewCreationListener _factory;

        private List<IMouseProcessor> UpdateMouseHandlers(IEnumerable<IContentType> bufferContentTypes)
        {
            var flag = false;
            for (var index = 0; index < _state.OrderedMouseProcessorProviders.Count; ++index)
            {
                var processorProvider = _state.OrderedMouseProcessorProviders[index];
                if (_textView.Roles.ContainsAny(processorProvider.Metadata.TextViewRoles) && ExtensionSelector.ContentTypeMatch(bufferContentTypes, processorProvider.Metadata.ContentTypes))
                {
                    if (ActiveMouseProcessors[index] == null)
                    {
                        ActiveMouseProcessors[index] = _factory.GuardedOperations.InstantiateExtension(processorProvider, processorProvider, p => p.GetAssociatedProcessor(_textView));
                        flag = true;
                    }
                }
                else if (ActiveMouseProcessors[index] != null)
                {
                    ActiveMouseProcessors[index] = null;
                    flag = true;
                }
            }
            if (!flag)
                return null;
            var mouseProcessorList = new List<IMouseProcessor>();
            foreach (var processor in ActiveMouseProcessors)
            {
                if (processor != null)
                    mouseProcessorList.Add(processor);
            }
            return mouseProcessorList;
        }

        private List<KeyProcessor> UpdateKeyProcessors(IEnumerable<IContentType> bufferContentTypes)
        {
            var flag = false;
            for (var index = 0; index < _state.OrderedKeyProcessorProviders.Count; ++index)
            {
                var processorProvider = _state.OrderedKeyProcessorProviders[index];
                if (_textView.Roles.ContainsAny(processorProvider.Metadata.TextViewRoles) && ExtensionSelector.ContentTypeMatch(bufferContentTypes, processorProvider.Metadata.ContentTypes))
                {
                    if (ActiveKeyProcessors[index] != null)
                        continue;
                    ActiveKeyProcessors[index] = _factory.GuardedOperations.InstantiateExtension(processorProvider, processorProvider, p => p.GetAssociatedProcessor(_textView));
                    flag = true;
                }
                else if (ActiveKeyProcessors[index] != null)
                {
                    ActiveKeyProcessors[index] = null;
                    flag = true;
                }
            }
            if (!flag)
                return null;
            var keyProcessorList = new List<KeyProcessor>();
            foreach (var processor in ActiveKeyProcessors)
            {
                if (processor != null)
                    keyProcessorList.Add(processor);
            }
            return keyProcessorList;
        }

        private void RefreshHandlers()
        {
            var bufferContentTypes = CollectedContentTypes();
            var mouseProcessorList = UpdateMouseHandlers(bufferContentTypes);
            if (mouseProcessorList != null)
                _masterMouseProcessor.MouseProcessors = mouseProcessorList;
            var keyProcessorList = UpdateKeyProcessors(bufferContentTypes);
            if (keyProcessorList == null)
                return;
            _keyProcessorDispatcher.KeyProcessors = keyProcessorList;
        }

        private IEnumerable<IContentType> CollectedContentTypes()
        {
            var bufferContentTypes = new HashSet<IContentType>();
            _textView.BufferGraph.GetTextBuffers(buffer =>
            {
                bufferContentTypes.Add(buffer.ContentType);
                return false;
            });
            return bufferContentTypes;
        }

        internal InputController(ITextView theTextView, InputControllerViewCreationListener factory)
        {
            if (theTextView.Properties.ContainsProperty(typeof(InputController)))
                return;
            _factory = factory;
            _state = _factory.State;
            _textView = theTextView;
            ActiveMouseProcessors = new IMouseProcessor[_state.OrderedMouseProcessorProviders.Count];
            ActiveKeyProcessors = new KeyProcessor[_state.OrderedKeyProcessorProviders.Count];
            var bufferContentTypes = CollectedContentTypes();
            var mouseProcessorList = UpdateMouseHandlers(bufferContentTypes);
            var keyProcessorList = UpdateKeyProcessors(bufferContentTypes);
            _masterMouseProcessor = new MasterMouseProcessor(_textView, _state.EditorOperationsFactoryService.GetEditorOperations(_textView), mouseProcessorList ?? new List<IMouseProcessor>(0), _factory);
            _keyProcessorDispatcher = new KeyProcessorDispatcher(_textView, keyProcessorList ?? new List<KeyProcessor>(0), _factory.GuardedOperations);
            theTextView.Closed += OnTextViewClosed;
            theTextView.BufferGraph.GraphBufferContentTypeChanged += OnGraphChanged;
            theTextView.BufferGraph.GraphBuffersChanged += OnGraphChanged;
            theTextView.Properties.AddProperty(typeof(InputController), this);
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _masterMouseProcessor.Dispose();
            _textView.Closed -= OnTextViewClosed;
            _textView.BufferGraph.GraphBufferContentTypeChanged -= OnGraphChanged;
            _textView.BufferGraph.GraphBuffersChanged -= OnGraphChanged;
        }

        private void OnGraphChanged(object sender, EventArgs e)
        {
            RefreshHandlers();
        }
    }
}