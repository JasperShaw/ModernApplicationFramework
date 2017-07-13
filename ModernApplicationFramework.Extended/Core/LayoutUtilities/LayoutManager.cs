using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutUtilities
{
    [Export(typeof(ILayoutManager))]
    public class LayoutManager : ILayoutManager
    {
        private readonly ILayoutItemStatePersister _layoutItemStatePersister;

        [ImportingConstructor]
        public LayoutManager(ILayoutItemStatePersister layoutItemStatePersister)
        {
            _layoutItemStatePersister = layoutItemStatePersister;
        }

        public void Load()
        {       
        }

        public void Save()
        {
        }

        public void Apply()
        {
        }
    }

    public interface ILayoutManager
    {
        void Load();

        void Save();

        void Apply();
    }
}
