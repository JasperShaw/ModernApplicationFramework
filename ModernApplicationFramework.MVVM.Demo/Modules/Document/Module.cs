using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using ModernApplicationFramework.MVVM.Core;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Document
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {

        public override IEnumerable<IDocument> DefaultDocuments
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
