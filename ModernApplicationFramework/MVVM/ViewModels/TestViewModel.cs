using System.ComponentModel.Composition;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.ViewModels
{
    [Export(typeof(ITest))]
    public class TestViewModel : ITest
    {
    }
}
