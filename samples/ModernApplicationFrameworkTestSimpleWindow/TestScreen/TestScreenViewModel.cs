using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace ModernApplicationFrameworkTestSimpleWindow.TestScreen
{
    [Export(typeof(TestScreenViewModel))]
    public class TestScreenViewModel : Screen
    {
    }
}
