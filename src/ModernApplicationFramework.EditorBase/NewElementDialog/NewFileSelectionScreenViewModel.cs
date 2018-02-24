using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.Commands;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.NewElement;
using ModernApplicationFramework.EditorBase.NewElementDialog.ViewModels;

namespace ModernApplicationFramework.EditorBase.NewElementDialog
{
    [Export(typeof(INewFileSelectionModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewFileSelectionScreenViewModel : NewElementScreenViewModelBase<NewFileCommandArguments>, INewFileSelectionModel
    {
        public override bool UsesNameProperty => false;

        public override bool UsesPathProperty => false;

        public override bool CanOpenWith => true;

        public override string NoItemsMessage => "No file templates found";

        public override string NoItemSelectedMessage => "No item selected";

        public override ObservableCollection<INewElementExtensionsProvider> Providers => null;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SetupExtensions();
        }

        protected virtual void SetupExtensions()
        {
            Extensions = IoC.Get<IEditorProvider>().SupportedFileDefinitions;
        }

        public override NewFileCommandArguments CreateResult(string name, string path)
        {
            return !(SelectedExtension is ISupportedFileDefinition fileArgument)
                ? null
                : new NewFileCommandArguments(name, fileArgument.FileType.FileExtension, fileArgument.PreferredEditor);
        }
    }
}
