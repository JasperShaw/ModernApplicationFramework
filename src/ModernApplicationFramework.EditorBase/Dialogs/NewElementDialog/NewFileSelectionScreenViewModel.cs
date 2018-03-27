using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Dialogs.EditorSelectorDialog;
using ModernApplicationFramework.EditorBase.Dialogs.NewElementDialog.ViewModels;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.Interfaces.Services;

namespace ModernApplicationFramework.EditorBase.Dialogs.NewElementDialog
{
    [Export(typeof(INewFileSelectionModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewFileSelectionScreenViewModel : NewElementScreenViewModelBase<NewFileArguments>, INewFileSelectionModel
    {
        public override bool UsesNameProperty => false;

        public override bool UsesPathProperty => false;

        public override bool CanOpenWith => true;

        public override string NoItemsMessage => NewElementDialogResources.NewFileExtensionMessageNoItems;

        public override string NoItemSelectedMessage => NewElementDialogResources.NewFileExtensionMessageNoSelectedItem;

        public override ObservableCollection<INewElementExtensionsProvider> Providers => null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SetupExtensions();
        }

        protected virtual void SetupExtensions()
        {
            Extensions = IoC.Get<FileDefinitionManager>().SupportedFileDefinitions
                .Where(x => x.SupportedFileOperation.HasFlag(SupportedFileOperation.Create));
        }

        public override NewFileArguments CreateResult(string name, string path)
        {
            return !(SelectedExtension is ISupportedFileDefinition fileArgument)
                ? null
                : new NewFileArguments(fileArgument,
                    IoC.Get<IUniqueNameCreator<ISupportedFileDefinition>>().GetUniqueName(fileArgument));
        }

        public override NewFileArguments CreateResultOpenWith(string name, string path)
        {
            var selectorModel = IoC.Get<NewFileEditorSelectorViewModel>();
            selectorModel.TargetExtension = SelectedExtension;
            if (IoC.Get<IWindowManager>().ShowDialog(selectorModel) != true)
                return null;
            if (!(SelectedExtension is ISupportedFileDefinition fileArgument))
                return null;
            return new NewFileArguments(fileArgument, IoC.Get<IUniqueNameCreator<ISupportedFileDefinition>>().GetUniqueName(fileArgument), selectorModel.Result.EditorId);
        }
    }
}
