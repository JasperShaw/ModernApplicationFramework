using System.Collections.Generic;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.MVVM.ViewModels;

namespace ModernApplicationFramework.MVVM.Views
{
    public partial class NewFileItemPresenter : IElementDialogItemPresenter
    {
        public NewFileItemPresenter()
        {
            InitializeComponent();
        }

        public IEnumerable<object> ItemSource { get; set; }
        public bool UsesNameProperty => true;
        public bool UsesPathProperty => false;
        public object CreateResult(string name, string path)
        {
            return new NewFileCommandArguments(name, ".txt", typeof(SimpleTextEditorViewModel));
        }
    }
}
