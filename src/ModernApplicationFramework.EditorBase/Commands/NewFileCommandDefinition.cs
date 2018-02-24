using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.Controls.NewElementDialog;
using ModernApplicationFramework.EditorBase.Controls.NewFileSectionExtension;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Layout;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public sealed class NewFileCommandDefinition : CommandDefinition
    {

        private IEditorProvider _editorProvider;

        private IEditorProvider EditorProvider => _editorProvider ?? (_editorProvider = IoC.Get<IEditorProvider>());


        public NewFileCommandDefinition()
        {
            var command = new UICommand(CreateNewFile, CanCreateNewFile);
            Command = command;
            DefaultKeyGesture = new MultiKeyGesture(Key.N, ModifierKeys.Control);
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture { get; }
        public override GestureScope DefaultGestureScope { get; }

        //public override string IconId => "NewFileIcon";
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;

        public override Guid Id => new Guid("{B33B7AA8-2FB6-4F80-88A2-3F97878273F3}");

        public override Uri IconSource => null; 

        //public override Uri IconSource
        //    =>
        //        new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/NewFile_16x.xaml",
        //            UriKind.RelativeOrAbsolute);

        public override string Name => "New File";
        public override string NameUnlocalized => "New File";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanCreateNewFile()
        {
            return EditorProvider.SupportedFileDefinitions.Any();
        }

        private void CreateNewFile()
        {
            var vm = new NewElementDialogViewModel<NewFileCommandArguments>();

            var presenter = IoC.Get<NewFileSelectionScreenViewModel>();
            //presenter.ItemSource = EditorProvider.SupportedFileDefinitions;

            vm.ItemPresenter = presenter;
            vm.DisplayName = "New File";

            var windowManager = IoC.Get<IWindowManager>();
            if (windowManager.ShowDialog(vm) != true)
                return;

            var result = vm.ResultData;

            var editor = EditorProvider?.Create(result.PreferredEditor);
            var viewAware = (IViewAware)editor;
            if (viewAware != null)
                viewAware.ViewAttached += (sender, e) =>
                {
                    var frameworkElement = (FrameworkElement)e.View;

                    async void LoadedHandler(object sender2, RoutedEventArgs e2)
                    {
                        frameworkElement.Loaded -= LoadedHandler;
                        await EditorProvider.New((IStorableDocument)editor, result.FileName + result.FileExtension);
                    }

                    frameworkElement.Loaded += LoadedHandler;
                };
            IoC.Get<IDockingMainWindowViewModel>().DockingHost.OpenDocument(editor);
        }
    }

    [Export(typeof(INewElementExtensionsProvider))]
    [Export(typeof(InstalledFilesExtensionProvider))]
    public class InstalledFilesExtensionProvider : NewElementExtensionProvider
    {
        public override string Text => "Installed";
        public override uint SortOrder => 0;

        protected override NewElementExtesionRootTreeNode ConstructTree()
        {
            var fileTypes = IoC.Get<IEditorProvider>().SupportedFileDefinitions;
            var rootNode = new NewElementExtesionRootTreeNode();
            var node = new NewElementExtensionTreeNode(rootNode, "All Files", fileTypes);
            rootNode.AddNode(node);
            var nodeEmpty = new NewElementExtensionTreeNode(rootNode, "Empty", new List<IExtensionDefinition>());
            rootNode.AddNode(nodeEmpty);
            return rootNode;
        }
    }

    public interface INewElementExtensionsProvider
    {
        string Text { get; }

        uint SortOrder { get; }

        INewElementExtensionTreeNode ExtensionsTree { get; }
    }

    public abstract class NewElementExtensionProvider : INewElementExtensionsProvider
    {
        private readonly Lazy<NewElementExtesionRootTreeNode> _rootNode;

        public abstract string Text { get; }

        public abstract uint SortOrder { get; }

        public INewElementExtensionTreeNode ExtensionsTree => _rootNode.Value;

        protected NewElementExtensionProvider()
        {
            _rootNode = new Lazy<NewElementExtesionRootTreeNode>(ConstructTree);
        }

        protected abstract NewElementExtesionRootTreeNode ConstructTree();
    }



    public interface INewElementExtensionTreeNode
    {
        string Text { get; }

        IList<INewElementExtensionTreeNode> Nodes { get; }

        INewElementExtensionTreeNode Parent { get; }

        IList<IExtensionDefinition> Extensions { get; }

        bool IsSelected { get; set; }

        bool IsExpanded { get; set; }
    }

    public class NewElementExtesionRootTreeNode : INewElementExtensionTreeNode
    {
        private readonly List<INewElementExtensionTreeNode> _cildNodes;
        public string Text => string.Empty;
        public IList<INewElementExtensionTreeNode> Nodes => _cildNodes;
        public INewElementExtensionTreeNode Parent => null;
        public IList<IExtensionDefinition> Extensions => null;
        public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }

        public NewElementExtesionRootTreeNode()
        {
            _cildNodes = new List<INewElementExtensionTreeNode>();
        }

        public void AddNode(INewElementExtensionTreeNode node)
        {
            _cildNodes.Add(node);
        }
    }

    public class NewElementExtensionTreeNode : INewElementExtensionTreeNode, INotifyPropertyChanged
    {
        private readonly ObservableCollection<IExtensionDefinition> _extensions;

        public string Text { get; }

        public IList<INewElementExtensionTreeNode> Nodes => null;

        public INewElementExtensionTreeNode Parent { get; }
        public IList<IExtensionDefinition> Extensions => _extensions;

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NewElementExtensionTreeNode(INewElementExtensionTreeNode parent, string text, IEnumerable<IExtensionDefinition> extensions)
        {
            Parent = parent;
            Text = text;
            if (extensions == null)
                throw new ArgumentNullException("Extensions cannot be null");
            _extensions = new ObservableCollection<IExtensionDefinition>();
            foreach (var extension in extensions)
                AddItemToExtensionsList(extension);
        }

        private void AddItemToExtensionsList(IExtensionDefinition extension)
        {
            if (extension == null)
                return;
            _extensions.Add(extension);
        }
    }

    public class ExtensionsCollection : ObservableCollection<IExtensionDefinition>
    {
    }

    internal class ExtensionsProviderCollection : ObservableCollection<INewElementExtensionsProvider>
    {
    }
}