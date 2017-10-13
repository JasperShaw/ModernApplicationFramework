using System;
using System.Collections.Generic;
using System.Xml;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsManager
{
    internal abstract class SettingsFile : DisposableObject, ISettingsFile
    {
        protected const string RootElement = "UserSettings";


        protected readonly ISettingsManager SettingsManager;

        public XmlDocument SettingsStorage { get; set; }

        protected SettingsFile(ISettingsManager settingsManager)
        {
            SettingsManager = settingsManager;
        }

        public abstract XmlNode GetSingleNode(string path, bool navigateAttributeWise = true);

        public abstract IEnumerable<XmlNode> GetChildNodes(string path, bool navigateAttributeWise = true);

        public abstract string GetPropertyValueData(string path, string propertyName,
            bool navigateAttributeWise = true);

        public abstract void AddPropertyValueElement(string path, string propertyName, string value,
            bool navigateAttributeWise = true);

        public abstract void AddOrChangePropertyValueElement(string path, string propertyName, string value,
            bool navigateAttributeWise = true,
            bool changeValueIfExists = true);


        public abstract void AddCategoryElement(ISettingsCategory settingsCategory);

        public abstract void AddToolsOptionsModelElement(string settingsModelName, string categoryName);

        public abstract string GetPropertyValueData(XmlNode node, string path, string propertyName,
            bool navigateAttributeWise = true);

        public abstract string GetPropertyValueData(XmlNode node, string propertyName);

        public abstract string GetAttributeValue(string path, string attribute, bool navigateAttributeWise = true);

        public abstract void SetPropertyValueData(string path, string propertyName, string value,
            bool navigateAttributeWise = true);
        
        public abstract void InsertDocument(string path, XmlDocument document, bool navigateAttributeWise);


        /// <summary>
        ///     Adds a new PropertyValue Element to the given node.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public abstract void AddPropertyValueElement(XmlNode parent, string propertyName, string value);

        /// <summary>
        ///     Adds a new PropertyValue Element to the given node. Overrides the value if property element already exists
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="changeValueIfExists"></param>
        public abstract void AddOrChangePropertyValueElement(XmlNode parent, string propertyName, string value,
            bool changeValueIfExists = true);

        /// <summary>
        ///     Creates and assinges a new SettingsStore
        /// </summary>
        /// <returns>returns the generated SettingsStore</returns>
        public abstract XmlDocument CreateNewSettingsStore();


        /// <summary>
        ///     Reads a settings file on disk and sets the SettingsStorage
        /// </summary>
        /// <param name="path">Path to the file</param>
        public abstract void TryRead(string path);

        public void Save()
        {
            lock (SettingsStorage)
            {
                var settings =
                    new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = @"    ",
                        NewLineChars = Environment.NewLine,
                        NewLineHandling = NewLineHandling.Replace
                    };
                using (var w = XmlWriter.Create(SettingsManager.EnvironmentVariables.SettingsFilePath, settings))
                {
                    SettingsStorage.Save(w);
                }
            }
        }

        protected override void DisposeManagedResources()
        {
            try
            {
                Save();
                SettingsStorage = null;
            }
            catch (Exception)
            {
                //Ignored
            }
        }


        /// <summary>
        ///     Provides a new XML-Document with declaration
        /// </summary>
        /// <returns></returns>
        protected XmlDocument CreateSettingsStoreBase()
        {
            var document = new XmlDocument();
            var xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = document.DocumentElement;
            document.InsertBefore(xmlDeclaration, root);
            return document;
        }
    }
}