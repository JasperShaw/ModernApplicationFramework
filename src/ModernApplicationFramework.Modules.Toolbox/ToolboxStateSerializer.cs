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
        protected override string RootNode => "ToolboxLayoutState";

        protected override Stream ValidationScheme => Stream.Null;

        protected override void ClearCurrentLayout()
        {
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
        }

        protected override void EnsureInitialized()
        {
        }

        protected override XmlNode GetBackupNode(in XmlDocument backup, IToolboxNode item)
        {
            return null;
        }

        protected override XmlNode GetCurrentNode(in XmlDocument currentLayout, IToolboxNode item)
        {
            return null;
        }

        protected override void HandleBackupNodeNull(IToolboxNode item)
        {
        }

        protected override void PrepareDeserialize()
        {
        }

        protected override void Serialize(ref XmlDocument xmlDocument)
        {
        }
    }
}
