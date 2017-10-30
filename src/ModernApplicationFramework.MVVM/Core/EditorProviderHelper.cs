using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    public static class EditorProviderHelper
    {
        public static Task<IDocument> GetEditor(string path, Type editorType)
        {
            var provider = IoC.GetAllInstances(typeof(IEditorProvider))
                              .Cast<IEditorProvider>()
                              .FirstOrDefault(p => p.Handles(path));
            if (provider == null)
                return null;

            var editor = provider.Create(editorType);

            var viewAware = (IViewAware)editor;
            viewAware.ViewAttached += (sender, e) =>
            {
                var frameworkElement = (FrameworkElement)e.View;

                async void LoadedHandler(object sender2, RoutedEventArgs e2)
                {
                    frameworkElement.Loaded -= LoadedHandler;
                    await provider.Open((IStorableDocument) editor, path);
                }

                frameworkElement.Loaded += LoadedHandler;
            };

            return Task.FromResult(editor);
        }
    }
}
