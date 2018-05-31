using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Modules.Toolbox
{
    [Export(typeof(IToolboxStateSerializer))]
    public class ToolboxStateSerializer : LayoutSerializer<IToolboxNode>, IToolboxStateSerializer
    {
        protected override string RootNode => throw new NotImplementedException();

        protected override Stream ValidationScheme => throw new NotImplementedException();

        protected override void ClearCurrentLayout()
        {
            throw new NotImplementedException();
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
            throw new NotImplementedException();
        }

        protected override void EnsureInitialized()
        {
            throw new NotImplementedException();
        }

        protected override XmlNode GetBackupNode(in XmlDocument backup, IToolboxNode item)
        {
            throw new NotImplementedException();
        }

        protected override XmlNode GetCurrentNode(in XmlDocument currentLayout, IToolboxNode item)
        {
            throw new NotImplementedException();
        }

        protected override void HandleBackupNodeNull(IToolboxNode item)
        {
            throw new NotImplementedException();
        }

        protected override void PrepareDeserialize()
        {
            throw new NotImplementedException();
        }

        protected override void Serialize(ref XmlDocument xmlDocument)
        {
            throw new NotImplementedException();
        }
    }
}
