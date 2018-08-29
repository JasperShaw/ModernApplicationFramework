using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Editor.KeyGestureScope
{
    internal sealed class GestureScopeManager : ICanHaveInputBindings
    {
        private readonly GestureScopeCreationListener _factory;
        private readonly KeyGestureScopeState _state;
        private readonly ITextView _textView;

        internal IList<GestureScope> ActiveScopes;

        private readonly List<GestureScope> _gestureScopes = new List<GestureScope>();

        public GestureScopeManager(ITextView textView, GestureScopeCreationListener factory)
        {
            if (textView.Properties.ContainsProperty(typeof(ICanHaveInputBindings)))
                return;
            _factory = factory;
            _state = factory.State;
            _textView = textView;

            ActiveScopes = new List<GestureScope>();
            var bufferContentTypes = CollectedContentTypes();
            var scopes = UpdateScopes(bufferContentTypes);

            BindableElement = _textView.VisualElement;
            _gestureScopes.AddRange(scopes);

            _factory.GestureService.AddModel(this);
            textView.Closed += OnTextViewClosed;
            textView.BufferGraph.GraphBufferContentTypeChanged += OnGraphChanged;
            textView.BufferGraph.GraphBuffersChanged += OnGraphChanged;
            textView.Properties.AddProperty(typeof(ICanHaveInputBindings), this);
        }

        private void OnTextViewClosed(object sender, EventArgs e)
        {
            _textView.Closed -= OnTextViewClosed;
            _textView.BufferGraph.GraphBufferContentTypeChanged -= OnGraphChanged;
            _textView.BufferGraph.GraphBuffersChanged -= OnGraphChanged;
            _factory.GestureService.RemoveModel(this);
        }

        private void OnGraphChanged(object sender, EventArgs e)
        {
            RefreshHandlers();
        }

        private void RefreshHandlers()
        {
            var bufferContentTypes = CollectedContentTypes();
            var scopes = UpdateScopes(bufferContentTypes);
            if (scopes != null)
            {
                _gestureScopes.Clear();
                _gestureScopes.AddRange(scopes);
                _factory.GestureService.RemoveModel(this);
                _factory.GestureService.AddModel(this);
            }             
        }

        private IEnumerable<GestureScope> UpdateScopes(IEnumerable<IContentType> bufferContentTypes)
        {
            var flag = false;

            foreach (var scopeProvider in _state.OrderedGestureScopeProviders)
            {
                var scopes = GuardedOperations.SingletonInstance.InstantiateExtension(scopeProvider, scopeProvider,
                    p => p.GetAssociatedScopes());

                if (_textView.Roles.ContainsAll(scopeProvider.Metadata.TextViewRoles) &&
                    ExtensionSelector.ContentTypeMatch(bufferContentTypes, scopeProvider.Metadata.ContentTypes))
                {
                    foreach (var scope in scopes)
                        ActiveScopes.Add(scope);
                    flag = true;
                }
                else
                {
                    foreach (var scope in scopes)
                        ActiveScopes.Remove(scope);
                    flag = true;
                }
            }
            ActiveScopes = ActiveScopes.Distinct().ToList();
            return !flag ? null : ActiveScopes.Where(scope => scope != null).ToList();
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

        public IEnumerable<GestureScope> GestureScopes => _gestureScopes;
        public UIElement BindableElement { get; }
    }
}