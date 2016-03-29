using System.ComponentModel.Composition;

namespace CaliBurnTest.ViewModels
{
    [Export(typeof(IViewModel))]
    public class AppViewModel : IViewModel
    {
    }
}
