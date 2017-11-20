using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.WindowManagement.CommandBarLayout
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(CommandBarLayoutSettings))]
    public sealed class CommandBarLayoutSettings : SettingsDataModel
    {
        private readonly ICommandBarSerializer _serializer;
        public override ISettingsCategory Category => Settings.CommandBarLayoutCategory;
        public override string Name => "Environment_CommandBars";
        public XmlNode Layout => GetSingleDataModel().FirstChild;

        [ImportingConstructor]
        public CommandBarLayoutSettings(ISettingsManager settingsManager, ICommandBarSerializer serializer)
        {
            _serializer = serializer;
            SettingsManager = settingsManager;
        }

        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
            var document = new XmlDocument();
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(stream);
                stream.Seek(0L, SeekOrigin.Begin);     
                document.Load(stream);
            }
            if (document.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                document.RemoveChild(document.FirstChild);
            SetSettingsModel(document, true);
        }
    }
}