using System.Windows;

namespace ModernApplicationFramework.Editor.Implementation
{

    //TODO: move to other project
    public static class EmbeddedObjectHelper
    {
        public static readonly DependencyProperty OleCommandTargetProperty =
            DependencyProperty.RegisterAttached("CommandTarget", typeof(ICommandTarget), typeof(UIElement),
                new PropertyMetadata(null));
        //public static readonly DependencyProperty UserContextProviderProperty = DependencyProperty.RegisterAttached("UserContextProvider", typeof(IVsProvideUserContext), typeof(UIElement), new PropertyMetadata((PropertyChangedCallback)null));

        public static ICommandTarget GetOleCommandTarget(UIElement element)
        {
            return (ICommandTarget)element.GetValue(OleCommandTargetProperty);
        }

        public static void SetOleCommandTarget(UIElement element, ICommandTarget value)
        {
            element.SetValue(OleCommandTargetProperty, value);
        }

        //public static IVsProvideUserContext GetUserContextProvider(UIElement element)
        //{
        //    return (IVsProvideUserContext)element.GetValue(EmbeddedObjectHelper.UserContextProviderProperty);
        //}

        //public static void SetUserContextProvider(UIElement element, IVsProvideUserContext value)
        //{
        //    element.SetValue(UserContextProviderProperty, (object)value);
        //}
    }
}