using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Settings;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;
using ModernApplicationFramework.Settings.SettingsManager;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Modules.Toolbox.State
{
    /// <summary>
    /// Settings for the toolbox module
    /// </summary>
    /// <seealso cref="ModernApplicationFramework.Settings.SettingDataModel.SettingsDataModel" />
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(ToolboxStateSettings))]
    public sealed class ToolboxStateSettings : SettingsDataModel
    {
        [Export]
        public static ISettingsCategory ToolboxStateCategoryCategory =
            new SettingsCategory(Guids.ToolboxStateCategoryId,
                SettingsCategoryType.Normal, "Environment_Toolbox", SettingsCategories.EnvironmentCategory);

        private readonly IToolboxStateSerializer _serializer;
        public override ISettingsCategory Category => ToolboxStateCategoryCategory;
        public override string Name => "Environment_Toolbox";
        public XmlNode Layout => GetSingleDataModel().FirstChild;

        [ImportingConstructor]
        public ToolboxStateSettings(ISettingsManager settingsManager, IToolboxStateSerializer serializer) : base(settingsManager)
        {
            _serializer = serializer;
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