using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    [Export(typeof(ILayoutManager))]
    public class LayoutManager : ILayoutManager
    {
        private readonly ILayoutItemStatePersister _layoutItemStatePersister;

        public ObservableCollection<Layout> Layouts { get; }

        [ImportingConstructor]
        public LayoutManager(ILayoutItemStatePersister layoutItemStatePersister)
        {
            _layoutItemStatePersister = layoutItemStatePersister;
            Layouts = new ObservableCollection<Layout>();
        }

        public void Load()
        {       
        }

        public void Save(string name)
        {
        }

        public void Apply(string name)
        {
        }

        public string GetUniqueLayoutName()
        {
            return GetUniqueLayoutName(1);
        }

        private string GetUniqueLayoutName(int index)
        {
            var i = index;
            var uniqueName = string.Format(CultureInfo.CurrentUICulture, Commands_Resources.SaveLayoutCommandDefinitionMessageBox_Default, i);
            return Layouts.Any(layout => uniqueName.Equals(layout.Name))
                ? GetUniqueLayoutName(++i)
                : uniqueName;
        }
    }

    public interface ILayoutManager
    {
        ObservableCollection<Layout> Layouts { get; }

        void Load();

        void Save(string name);

        void Apply(string name);

        string GetUniqueLayoutName();
    }

    public class Layout
    {
        public string Name { get; set; }
    }
}
