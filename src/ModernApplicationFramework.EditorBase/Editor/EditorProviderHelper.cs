﻿namespace ModernApplicationFramework.EditorBase.Editor
{
    public static class EditorProviderHelper
    {
        //public static Task<IEditor> GetEditor(string path, Guid editorId)
        //{
        //    var provider = IoC.GetAllInstances(typeof(IEditorProvider))
        //        .Cast<IEditorProvider>()
        //        .FirstOrDefault(p => p.Handles(path));
        //    if (provider == null)
        //        return null;

        //    var editor = provider.Get(editorId);

        //    var viewAware = (IViewAware)editor;
        //    viewAware.ViewAttached += (sender, e) =>
        //    {
        //        var frameworkElement = (FrameworkElement)e.View;

        //        async void LoadedHandler(object sender2, RoutedEventArgs e2)
        //        {
        //            frameworkElement.Loaded -= LoadedHandler;
        //            await provider.Open((IStorableDocument)editor, path);
        //        }
        //        frameworkElement.Loaded += LoadedHandler;
        //    };

        //    return Task.FromResult(editor);
        //}
    }
}