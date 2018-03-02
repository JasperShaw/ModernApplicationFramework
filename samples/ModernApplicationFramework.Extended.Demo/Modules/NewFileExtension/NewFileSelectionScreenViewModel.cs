using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;

namespace ModernApplicationFramework.Extended.Demo.Modules.NewFileExtension
{
    [Export(typeof(INewFileSelectionModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class NewFileSelectionScreenViewModel : EditorBase.NewElementDialog.NewFileSelectionScreenViewModel
    {
        public override ObservableCollection<INewElementExtensionsProvider> Providers => new ObservableCollection<INewElementExtensionsProvider>
        {
            IoC.Get<InstalledFilesExtensionProvider>()
        };

        protected override void SetupExtensions()
        {
        }
    }
}
