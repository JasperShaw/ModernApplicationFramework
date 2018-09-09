using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModernApplicationFramework.TextEditor;

namespace ModernApplicationFramework.Utilities.Core
{
    public interface IGuardedOperations
    {
        void CallExtensionPoint(Action call);

        void CallExtensionPoint(object errorSource, Action call);

        T CallExtensionPoint<T>(Func<T> call, T valueOnThrow);

        T CallExtensionPoint<T>(object errorSource, Func<T> call, T valueOnThrow);

        Task CallExtensionPointAsync(Func<Task> asyncAction);

        Task CallExtensionPointAsync(object errorSource, Func<Task> asyncAction);

        Task<T> CallExtensionPointAsync<T>(Func<Task<T>> asyncCall, T valueOnThrow);

        Task<T> CallExtensionPointAsync<T>(object errorSource, Func<Task<T>> asyncCall, T valueOnThrow);

        IEnumerable<Lazy<TExtensionFactory, TMetadataView>> FindEligibleFactories<TExtensionFactory, TMetadataView>(IEnumerable<Lazy<TExtensionFactory, TMetadataView>> lazyFactories, IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService) where TExtensionFactory : class where TMetadataView : INamedContentTypeMetadata;

        void HandleException(object errorSource, Exception e);

        TExtension InstantiateExtension<TExtension>(object errorSource, Lazy<TExtension> provider);

        TExtension InstantiateExtension<TExtension, TMetadata>(object errorSource, Lazy<TExtension, TMetadata> provider);

        TExtensionInstance InstantiateExtension<TExtension, TMetadata, TExtensionInstance>(object errorSource, Lazy<TExtension, TMetadata> provider, Func<TExtension, TExtensionInstance> getter);

        TExtension InvokeBestMatchingFactory<TExtension, TMetadataView>(IList<Lazy<TExtension, TMetadataView>> providerHandles, IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService, object errorSource) where TMetadataView : IContentTypeMetadata;

        TExtensionInstance InvokeBestMatchingFactory<TExtensionFactory, TExtensionInstance, TMetadataView>(IList<Lazy<TExtensionFactory, TMetadataView>> providerHandles, IContentType dataContentType, Func<TExtensionFactory, TExtensionInstance> getter, IContentTypeRegistryService contentTypeRegistryService, object errorSource) where TExtensionFactory : class where TMetadataView : IContentTypeMetadata;

        List<TExtensionInstance> InvokeEligibleFactories<TExtensionInstance, TExtensionFactory, TMetadataView>(IEnumerable<Lazy<TExtensionFactory, TMetadataView>> lazyFactories, Func<TExtensionFactory, TExtensionInstance> getter, IContentType dataContentType, IContentTypeRegistryService contentTypeRegistryService, object errorSource) where TExtensionInstance : class where TExtensionFactory : class where TMetadataView : INamedContentTypeMetadata;

        List<TExtensionInstance> InvokeMatchingFactories<TExtensionInstance, TExtensionFactory, TMetadataView>(IEnumerable<Lazy<TExtensionFactory, TMetadataView>> lazyFactories, Func<TExtensionFactory, TExtensionInstance> getter, IContentType dataContentType, object errorSource) where TExtensionInstance : class where TExtensionFactory : class where TMetadataView : IContentTypeMetadata;

        void RaiseEvent(object sender, EventHandler eventHandlers);

        void RaiseEvent<TArgs>(object sender, EventHandler<TArgs> eventHandlers, TArgs args) where TArgs : EventArgs;

        //Task RaiseEventOnBackgroundAsync<TArgs>(object sender, AsyncEventHandler<TArgs> eventHandlers, TArgs args) where TArgs : EventArgs;
    }
}
