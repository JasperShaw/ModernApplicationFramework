using System;
using ModernApplicationFramework.Extended.UIContext;

namespace ModernApplicationFramework.Extended
{
    public static class GeneralUiContexts
    {
        private static readonly Lazy<UiContext> _shellInitializedContext = new Lazy<UiContext>(() =>
            UiContext.FromUiContextGuid(new Guid(UiContextGuids.ShellInitializedContextGuid)));

        private static readonly Lazy<UiContext> _shellInitializingContext = new Lazy<UiContext>(() =>
            UiContext.FromUiContextGuid(new Guid(UiContextGuids.ShellInitializingContextGuid)));

        public static UiContext ShellInitializedContext => _shellInitializedContext.Value;

        public static UiContext ShellInitializingContext => _shellInitializingContext.Value;
    }
}
