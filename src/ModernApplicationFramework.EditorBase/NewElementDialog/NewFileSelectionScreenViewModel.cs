using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Commands;
using ModernApplicationFramework.EditorBase.Controls.EditorSelectorDialog;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.NewElementDialog.ViewModels;

namespace ModernApplicationFramework.EditorBase.NewElementDialog
{
    [Export(typeof(INewFileSelectionModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewFileSelectionScreenViewModel : NewElementScreenViewModelBase<NewFileCommandArguments>, INewFileSelectionModel
    {
        public override bool UsesNameProperty => false;

        public override bool UsesPathProperty => true;

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

        public override NewFileCommandArguments CreateResult(string name, string path)
        {
            return !(SelectedExtension is ISupportedFileDefinition fileArgument)
                ? null
                : new NewFileCommandArguments(fileArgument, "UniqueName");
        }

        public override NewFileCommandArguments CreateResultOpenWith(string name, string path)
        {
            var selectorModel = IoC.Get<IEditorSelectorViewModel>();
            selectorModel.TargetExtension = SelectedExtension;
            if (IoC.Get<IWindowManager>().ShowDialog(selectorModel) != true)
                return null;
            if (!(SelectedExtension is ISupportedFileDefinition fileArgument))
                return null;
            return new NewFileCommandArguments(fileArgument, "UniqueName", selectorModel.Result.EditorId);
        }
    }
}
