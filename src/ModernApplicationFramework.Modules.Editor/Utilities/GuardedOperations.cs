using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.TextEditor;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.Modules.Editor.Utilities
{
    [Export]
    [Export(typeof(IGuardedOperations))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class GuardedOperations : IGuardedOperations
    {
        [ImportMany]
        private List<Lazy<IExtensionErrorHandler>> _errorHandlerExports;

        private FrugalList<IExtensionErrorHandler> _errorHandlers;

        internal static bool ReThrowIfNoHandlers { get; set; }

        public GuardedOperations()
        {
            
        }

        public GuardedOperations(params IExtensionErrorHandler[] extensionErrorHandler)
        {
            _errorHandlers = new FrugalList<IExtensionErrorHandler>(extensionErrorHandler);
        }

        private IEnumerable<IExtensionErrorHandler> ErrorHandlers
        {
            get
            {
                if (_errorHandlers != null)
                    return _errorHandlers;
                _errorHandlers = new FrugalList<IExtensionErrorHandler>();
                if (_errorHandlerExports == null) return _errorHandlers;
                foreach (var export in _errorHandlerExports)
                {
                    try
                    {
                        var handler = export.Value;
                        if (handler != null)
                            _errorHandlers.Add(handler);
                    }
                    catch
                    {
                    }
                }
                return _errorHandlers;
            }
        }

        public void CallExtensionPoint(Action call)
        {
            CallExtensionPoint((object)null, call);
        }

        public void CallExtensionPoint(object errorSource, Action call)
        {
            try
            {
                BeforeCallingExtensionPoint(errorSource ?? call);
                call();
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
            }
            finally
            {
                AfterCallingExtensionPoint(errorSource ?? call);
            }
        }

        public T CallExtensionPoint<T>(Func<T> call, T valueOnThrow)
        {
            return CallExtensionPoint(null, call, valueOnThrow);
        }

        public T CallExtensionPoint<T>(object errorSource, Func<T> call, T valueOnThrow)
        {
            try
            {
                BeforeCallingExtensionPoint(errorSource ?? call);
                return call();
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
                return valueOnThrow;
            }
            finally
            {
                AfterCallingExtensionPoint(errorSource ?? call);
            }
        }

        public async Task CallExtensionPointAsync(Func<Task> asyncAction)
        {
            await CallExtensionPointAsync((object)null, asyncAction);
        }

        public async Task CallExtensionPointAsync(object errorSource, Func<Task> asyncAction)
        {
            try
            {
                await asyncAction();
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
            }
        }

        public async Task<T> CallExtensionPointAsync<T>(Func<Task<T>> asyncCall, T valueOnThrow)
        {
            return await CallExtensionPointAsync(null, asyncCall, valueOnThrow);
        }

        public async Task<T> CallExtensionPointAsync<T>(object errorSource, Func<Task<T>> asyncCall, T valueOnThrow)
        {
            try
            {
                return await asyncCall();
            }
            catch (Exception ex)
            {
                HandleException(errorSource, ex);
                return valueOnThrow;
            }
        }

        public IEnumerable<Lazy<TExtensionFactory, TMetadataView>> FindEligibleFactories<TExtensionFactory, TMetadataView>(IEnumerable<Lazy<TExtensionFactory, TMetadataView>> lazyFactories,
            IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService) where TExtensionFactory : class where TMetadataView : INamedContentTypeMetadata
        {
            Dictionary<string, List<Lazy<TExtensionFactory, TMetadataView>>> namedFactories = null;
            HashSet<string> replaced = null;
            TMetadataView metadata;
            foreach (var lazyFactory1 in lazyFactories)
            {
                var lazyFactory = lazyFactory1;
                var dataContentType1 = dataContentType;
                metadata = lazyFactory.Metadata;
                var contentTypes = metadata.ContentTypes;
                if (ExtensionSelector.ContentTypeMatch(dataContentType1, contentTypes))
                {
                    metadata = lazyFactory.Metadata;
                    if (string.IsNullOrEmpty(metadata.Name))
                    {
                        yield return lazyFactory;
                    }
                    else
                    {
                        if (namedFactories == null)
                            namedFactories = new Dictionary<string, List<Lazy<TExtensionFactory, TMetadataView>>>(StringComparer.OrdinalIgnoreCase);
                        var dictionary1 = namedFactories;
                        metadata = lazyFactory.Metadata;
                        var name1 = metadata.Name;
                        if (!dictionary1.TryGetValue(name1, out var lazyList1))
                        {
                            lazyList1 = new List<Lazy<TExtensionFactory, TMetadataView>>();
                            var dictionary2 = namedFactories;
                            metadata = lazyFactory.Metadata;
                            var name2 = metadata.Name;
                            var lazyList2 = lazyList1;
                            dictionary2.Add(name2, lazyList2);
                        }
                        lazyList1.Add(lazyFactory);
                        metadata = lazyFactory.Metadata;
                        if (metadata.Replaces != null)
                        {
                            if (replaced == null)
                                replaced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            metadata = lazyFactory.Metadata;
                            foreach (var replace in metadata.Replaces)
                                replaced.Add(replace);
                        }
                    }
                }
            }
            if (namedFactories != null)
            {
                foreach (var candidates in namedFactories.Values)
                {
                    var lazy = candidates[0];
                    if (replaced != null)
                    {
                        var stringSet = replaced;
                        metadata = lazy.Metadata;
                        var name = metadata.Name;
                        if (stringSet.Contains(name))
                            continue;
                    }
                    SortCandidates(candidates, dataContentType, contentTypeRegistryService);
                    yield return candidates[0];
                }
            }
        }

        public TExtensionInstance InvokeBestMatchingFactory<TExtensionFactory, TExtensionInstance, TMetadataView>(
            IList<Lazy<TExtensionFactory, TMetadataView>> providerHandles, IContentType dataContentType, Func<TExtensionFactory, TExtensionInstance> getter,
            IContentTypeRegistryService contentTypeRegistryService, object errorSource) where TExtensionFactory : class where TMetadataView : IContentTypeMetadata
        {
            var factory = InvokeBestMatchingFactory(providerHandles, dataContentType, contentTypeRegistryService, errorSource);
            if (factory == null)
                return default;
            TExtensionInstance extensionInstance = default;
            CallExtensionPoint(errorSource, () => extensionInstance = getter(factory));
            return extensionInstance;
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

        public TExtension InvokeBestMatchingFactory<TExtension, TMetadataView>(IList<Lazy<TExtension, TMetadataView>> providerHandles, IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService, object errorSource) where TMetadataView : IContentTypeMetadata
        {
            var candidates = new List<Lazy<TExtension, TMetadataView>>();
            foreach (var providerHandle in providerHandles)
            {
                foreach (var contentType in providerHandle.Metadata.ContentTypes)
                {
                    if (string.Compare(dataContentType.TypeName, contentType, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var extension = InstantiateExtension(errorSource, providerHandle);
                        if (extension != null)
                            return extension;
                    }
                    else if (dataContentType.IsOfType(contentType))
                    {
                        candidates.Add(providerHandle);
                        break;
                    }
                }
            }
            SortCandidates(candidates, dataContentType, contentTypeRegistryService);
            foreach (var candidate in candidates)
            {
                var extension = InstantiateExtension(errorSource, candidate);
                if (extension != null)
                    return extension;
            }
            return default;
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

        public void RaiseEvent(object sender, EventHandler eventHandlers)
        {
            if (eventHandlers == null)
                return;
            foreach (EventHandler invocation in eventHandlers.GetInvocationList())
            {
                try
                {
                    BeforeCallingEventHandler(invocation);
                    invocation(sender, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    HandleException(sender, ex);
                }
                finally
                {
                    AfterCallingEventHandler(invocation);
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
                    BeforeCallingEventHandler(invocation);
                    invocation(sender, args);
                }
                catch (Exception ex)
                {
                    HandleException(sender, ex);
                }
                finally
                {
                    AfterCallingEventHandler(invocation);
                }
            }
        }

        public void HandleException(object errorSource, Exception e)
        {
            var flag = false;
            foreach (var errorHandler in ErrorHandlers)
            {
                try
                {
                    errorHandler.HandleError(errorSource, e);
                    flag = true;
                }
                catch
                {
                }
            }
            if (!flag && ReThrowIfNoHandlers)
                throw new Exception("Unhandled exception.", e);
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

        public List<TExtensionInstance> InvokeEligibleFactories<TExtensionInstance, TExtensionFactory, TMetadataView>(IEnumerable<Lazy<TExtensionFactory, TMetadataView>> lazyFactories, Func<TExtensionFactory, TExtensionInstance> getter, IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService, object errorSource) where TExtensionInstance : class where TExtensionFactory : class where TMetadataView : INamedContentTypeMetadata
        {
            var extensionInstanceList = new List<TExtensionInstance>();
            foreach (var eligibleFactory in FindEligibleFactories(lazyFactories, dataContentType, contentTypeRegistryService))
            {
                try
                {
                    var extensionFactory = eligibleFactory.Value;
                    if (extensionFactory != null)
                    {
                        var extensionInstance = getter(extensionFactory);
                        if (extensionInstance != null)
                            extensionInstanceList.Add(extensionInstance);
                    }
                }
                catch (Exception ex)
                {
                    HandleException(errorSource, ex);
                }
            }
            return extensionInstanceList;
        }

        private void AfterCallingExtensionPoint(object extensionPoint)
        {
            //if (this.PerfTrackers.Count == 0)
            //    return;
            //for (int index = 0; index < this.PerfTrackers.Count; ++index)
            //{
            //    try
            //    {
            //        this.PerfTrackers[index].AfterCallingExtension(extensionPoint);
            //    }
            //    catch (Exception ex)
            //    {
            //        this.HandleException((object)this.PerfTrackers[index], ex);
            //    }
            //}
        }

        private void AfterCallingEventHandler(Delegate handler)
        {
            //if (this.PerfTrackers.Count == 0)
            //    return;
            //for (int index = 0; index < this.PerfTrackers.Count; ++index)
            //{
            //    try
            //    {
            //        this.PerfTrackers[index].AfterCallingEventHandler(handler);
            //    }
            //    catch (Exception ex)
            //    {
            //        this.HandleException((object)this.PerfTrackers[index], ex);
            //    }
            //}
        }

        private void BeforeCallingEventHandler(Delegate handler)
        {
            //if (this.PerfTrackers.Count == 0)
            //    return;
            //for (int index = 0; index < this.PerfTrackers.Count; ++index)
            //{
            //    try
            //    {
            //        this.PerfTrackers[index].BeforeCallingEventHandler(handler);
            //    }
            //    catch (Exception ex)
            //    {
            //        this.HandleException((object)this.PerfTrackers[index], ex);
            //    }
            //}
        }

        private void BeforeCallingExtensionPoint(object extensionPoint)
        {
            //if (this.PerfTrackers.Count == 0)
            //    return;
            //for (int index = 0; index < this.PerfTrackers.Count; ++index)
            //{
            //    try
            //    {
            //        this.PerfTrackers[index].BeforeCallingExtension(extensionPoint);
            //    }
            //    catch (Exception ex)
            //    {
            //        this.HandleException((object)this.PerfTrackers[index], ex);
            //    }
            //}
        }

        private static void SortCandidates<TExtension, TMetadataView>(List<Lazy<TExtension, TMetadataView>> candidates, IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService) where TMetadataView : IContentTypeMetadata
        {
            if (candidates.Count <= 1)
                return;
            var contentTypes = new List<IContentType>();
            foreach (var candidate in candidates)
            {
                foreach (var contentType1 in candidate.Metadata.ContentTypes)
                {
                    if (!dataContentType.IsOfType(contentType1)) continue;
                    var contentType2 = contentTypeRegistryService.GetContentType(contentType1);
                    if (!contentTypes.Contains(contentType2))
                        contentTypes.Add(contentType2);
                }
            }
            contentTypes.Sort(CompareContentTypes);
            candidates.Sort((left, right) =>
            {
                var metadata = left.Metadata;
                var num1 = BestContentTypeScore(metadata.ContentTypes, contentTypes);
                metadata = right.Metadata;
                var num2 = BestContentTypeScore(metadata.ContentTypes, contentTypes);
                return num1 - num2;
            });
        }

        private static int BestContentTypeScore(IEnumerable<string> contentTypes, List<IContentType> sortedContentTypes)
        {
            return contentTypes.Min(s => ContentTypeScore(s, sortedContentTypes));
        }

        private static int ContentTypeScore(string contentTypeName, IReadOnlyList<IContentType> sortedContentTypes)
        {
            for (var index = 0; index < sortedContentTypes.Count; ++index)
            {
                if (string.Compare(sortedContentTypes[index].TypeName, contentTypeName, StringComparison.OrdinalIgnoreCase) == 0)
                    return index;
            }
            return sortedContentTypes.Count;
        }

        private static int CompareContentTypes(IContentType left, IContentType right)
        {
            if (left == right)
                return 0;
            if (left.IsOfType(right.TypeName))
                return -1;
            if (right.IsOfType(left.TypeName))
                return 1;
            return string.Compare(left.TypeName, right.TypeName, StringComparison.OrdinalIgnoreCase);
        }
    }
}
