using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    [Export(typeof(IFileDefinitionManager))]
    public class FileDefinitionManager : IFileDefinitionManager
    {
#pragma warning disable 649
        [ImportMany] private ISupportedFileDefinition[] _supportedFileDefinitions;
#pragma warning restore 649

        public IReadOnlyCollection<ISupportedFileDefinition> SupportedFileDefinitions
        {
            get
            {
                if (_supportedFileDefinitions == null)
                    throw new NullReferenceException();
                return _supportedFileDefinitions;
            }
        }

        public ISupportedFileDefinition GetDefinitionByExtension(string extension)
        {
            return SupportedFileDefinitions.FirstOrDefault(l => l.FileExtension.Contains(extension));
        }
    }
}