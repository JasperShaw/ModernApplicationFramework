using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data;

namespace ModernApplicationFramework.Editor.Implementation
{
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