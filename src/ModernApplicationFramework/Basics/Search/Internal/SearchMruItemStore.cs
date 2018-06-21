using System.ComponentModel.Composition;

namespace ModernApplicationFramework.Basics.Search.Internal
{
    [Export(typeof(ISearchMruItemStore))]
    internal class SearchMruItemStore
    {
    }

    internal interface ISearchMruItemStore
    {
        
    }
}
