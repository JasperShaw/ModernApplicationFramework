using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.TestWindow.Document
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase.ModuleBase
    {

        public override IEnumerable<ILayoutItem> DefaultDocuments
        {
            get
            {
                yield return IoC.Get<SampleViewModel>();
            }
        }

        public override void PostInitialize()
        {
            DockingHostViewModel.OpenDocument(IoC.Get<SampleViewModel>());
        }
    }
}
