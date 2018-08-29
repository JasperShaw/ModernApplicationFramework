using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Editor.Interop;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Attributes;
using ModernApplicationFramework.Utilities.Core;
using ModernApplicationFramework.Utilities.Interfaces;
using Action = System.Action;

namespace ModernApplicationFramework.Editor.Commanding
{
    [Export(typeof(ITextViewCreationListener))]
    [Export(typeof(IActiveTextViewState))]
    [ContentType("any")]
    [TextViewRole("INTERACTIVE")]
    internal class ActiveTextViewState : ITextViewCreationListener, IActiveTextViewState
    {
        private readonly ConditionalWeakTable<ITextView, EditorAndMenuFocusTracker> _mapping;

        private readonly object _lockObj = new object();
        private static IActiveTextViewState _instance;

        public ITextView ActiveTextView { get; set; }

        internal static IActiveTextViewState Instance => _instance ?? (_instance = IoC.Get<IActiveTextViewState>());

        public ICommandTarget ActiveCommandTarget
        {
            get
            {
                if (ActiveTextView == null)
                    return null;
                ActiveTextView.Properties.TryGetProperty<ICommandTarget>(typeof(ICommandTarget), out var commandTarget);
                return commandTarget;
            }
        }

        public ActiveTextViewState()
        {
            _mapping = new ConditionalWeakTable<ITextView, EditorAndMenuFocusTracker>();
        }

        public void TextViewCreated(ITextView textView)
        {
            textView.Closed += TextView_Closed;
            var focusTracker = new EditorAndMenuFocusTracker(textView);
            _mapping.Add(textView, focusTracker);
            focusTracker.GotFocus += FocusTrackerGotFocus;
            focusTracker.LostFocus += FocusTrackerOnLostFocus;
        }

        private void FocusTrackerOnLostFocus(object sender, EventArgs e)
        {
            lock (_lockObj)
                ActiveTextView = null;
        }

        private void FocusTrackerGotFocus(object sender, EventArgs e)
        {
            lock (_lockObj)
                ActiveTextView = sender as ITextView;
        }

        private void TextView_Closed(object sender, EventArgs e)
        {
            if (!(sender is ITextView textView))
                return;
            if (_mapping.TryGetValue(textView, out var tracker))
            {
                tracker.GotFocus -= FocusTrackerGotFocus;
                tracker.LostFocus -= FocusTrackerOnLostFocus;
                _mapping.Remove(textView);
            }
            textView.Closed -= TextView_Closed;
        }
    }


    [Export(typeof(IEdtiorGestureScopeProvider))]
    [Name("DefaultKeyGestureScope")]
    [ContentType("any")]
    [TextViewRole("INTERACTIVE")]
    internal class KeyBindingScopeFactory : IEdtiorGestureScopeProvider
    {
        public IEnumerable<GestureScope> GetAssociatedScopes()
        {
            return new[]
            {
                TextEditorGestureScope.TextEditorScope,
                GestureScopes.GlobalGestureScope
            };
        }
    }

    [Export(typeof(IEdtiorGestureScopeProvider))]
    [ContentType("Output")]
    [TextViewRole("INTERACTIVE")]
    [Name("OutputKeyGestureScope")]
    [Order(Before = "DefaultKeyGestureScope")]
    internal class OutputKeyBindingScopeFactory : IEdtiorGestureScopeProvider
    {
        public IEnumerable<GestureScope> GetAssociatedScopes()
        {
            return new[] { TextEditorGestureScope.OutputGestureScope };
        }
    }

    public interface IEdtiorGestureScopeProvider
    {
        IEnumerable<GestureScope> GetAssociatedScopes();
    }

    [Export(typeof(ITextViewCreationListener))]
    [ContentType("Any")]
    [TextViewRole("INTERACTIVE")]
    internal sealed class GestureScopeCreationListener : ITextViewCreationListener
    {
        [Import] internal KeyGestureScopeState State;

        [Import] internal IKeyGestureService GestureService;

        public void TextViewCreated(ITextView textView)
        {
            new GestureScopeManager(textView, this);
        }
    }

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

    //[Export(typeof(KeyBindingScopeManager))]
    //internal class KeyBindingScopeManager : ICanHaveInputBindings
    //{
    //    private readonly ITextView _textView;
    //    private readonly IKeyGestureService _gestureService;
    //    private readonly List<GestureScope> _gestureScopes = new List<GestureScope>();

    //    public KeyBindingScopeManager(ITextView textView, IKeyGestureService gestureService, IEnumerable<GestureScope> gestureScopes)
    //    {
    //        _textView = textView;
    //        _gestureService = gestureService;
    //        _gestureScopes.AddRange(gestureScopes);
    //        _gestureService.AddModel(this);
    //    }


    //    public KeyBindingScopeManager(ITextView textView, IKeyGestureService gestureService, GestureScope gestureScope) :
    //        this(textView, gestureService, new List<GestureScope> { gestureScope })
    //    {
    //    }

    //    public IEnumerable<GestureScope> GestureScopes => _gestureScopes;

    //    public UIElement BindableElement => _textView.VisualElement;
    //}

    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    internal sealed class KeyGestureScopeState : IPartImportsSatisfiedNotification
    {
        [ImportMany] public List<Lazy<GestureScope>> Scopes;

        [ImportMany]
        public List<Lazy<IEdtiorGestureScopeProvider, IOrderableContentTypeAndTextViewRoleMetadata>> GestureScopeProviders
        {
            get;
            set;
        }

        public IList<Lazy<IEdtiorGestureScopeProvider, IOrderableContentTypeAndTextViewRoleMetadata>> OrderedGestureScopeProviders { get; private set; }

        public void OnImportsSatisfied()
        {
            OrderedGestureScopeProviders = Orderer.Order(GestureScopeProviders);
        }
    }

    public interface IContentTypeAndTextViewRoleMetadata
    {
        IEnumerable<string> TextViewRoles { get; }

        IEnumerable<string> ContentTypes { get; }
    }

    public interface IOrderableContentTypeAndTextViewRoleMetadata : IContentTypeAndTextViewRoleMetadata, IOrderable
    {
    }

    internal static class ExtensionSelector
    {
        public static List<Lazy<TProvider, TMetadataView>> SelectMatchingExtensions<TProvider, TMetadataView>(IEnumerable<Lazy<TProvider, TMetadataView>> providerHandles, IContentType dataContentType) where TMetadataView : IContentTypeMetadata
        {
            return providerHandles.Where(providerHandle => ContentTypeMatch(dataContentType, providerHandle.Metadata.ContentTypes)).ToList();
        }

        public static bool ContentTypeMatch(IContentType dataContentType, IEnumerable<string> extensionContentTypes)
        {
            return extensionContentTypes.Any(dataContentType.IsOfType);
        }

        public static bool ContentTypeMatch(IEnumerable<IContentType> dataContentTypes, IEnumerable<string> extensionContentTypes)
        {
            return dataContentTypes.Any(dataContentType => ContentTypeMatch(dataContentType, extensionContentTypes));
        }
    }

    internal sealed class GuardedOperations
    {
        private List<IExtensionErrorHandler> _errorHandlers;
        private static GuardedOperations _instance;

        [ImportMany]
        public List<Lazy<IExtensionErrorHandler>> _errorHandlerExports { get; set; }

        public List<IExtensionErrorHandler> ErrorHandlers
        {
            get
            {
                if (_errorHandlers == null)
                {
                    _errorHandlers = new List<IExtensionErrorHandler>();
                    if (_errorHandlerExports != null)
                    {
                        foreach (var errorHandlerExport in _errorHandlerExports)
                        {
                            try
                            {
                                IExtensionErrorHandler extensionErrorHandler = errorHandlerExport.Value;
                                if (extensionErrorHandler != null)
                                    _errorHandlers.Add(extensionErrorHandler);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                return _errorHandlers;
            }
            set => _errorHandlers = value;
        }

        public static GuardedOperations SingletonInstance => _instance ?? (_instance = new GuardedOperations());

        public GuardedOperations()
        {
        }

        public GuardedOperations(IExtensionErrorHandler extensionErrorHandler)
        {
            _errorHandlers = new List<IExtensionErrorHandler> { extensionErrorHandler };
        }



        public TExtension InstantiateExtension<TExtension>(object errorSource, Lazy<TExtension> provider)
        {
            try
            {
                return provider.Value;
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
                return default;
            }
        }

        public TExtension InstantiateExtension<TExtension, TMetadata>(object errorSource, Lazy<TExtension, TMetadata> provider)
        {
            try
            {
                return provider.Value;
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
                return default;
            }
        }

        public TExtensionInstance InstantiateExtension<TExtension, TMetadata, TExtensionInstance>(object errorSource, Lazy<TExtension, TMetadata> provider, Func<TExtension, TExtensionInstance> getter)
        {
            try
            {
                return getter(provider.Value);
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
                return default;
            }
        }

        public void CallExtensionPoint(object errorSource, Action call)
        {
            try
            {
                call();
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
            }
        }

        public void RaiseEvent(object sender, EventHandler eventHandlers)
        {
            if (eventHandlers == null)
                return;
            foreach (EventHandler invocation in eventHandlers.GetInvocationList())
            {
                try
                {
                    invocation(sender, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    HandleException(sender, ex);
                }
            }
        }

        public void RaiseEvent<TArgs>(object sender, EventHandler<TArgs> eventHandlers, TArgs args) where TArgs : EventArgs
        {
            if (eventHandlers == null)
                return;
            foreach (EventHandler<TArgs> invocation in eventHandlers.GetInvocationList())
            {
                try
                {
                    invocation(sender, args);
                }
                catch (Exception ex)
                {
                    HandleException(sender, ex);
                }
            }
        }

        public void HandleException(object errorSource, Exception e)
        {
            foreach (var errorHandler in ErrorHandlers)
            {
                try
                {
                    errorHandler.HandleError(errorSource, e);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}