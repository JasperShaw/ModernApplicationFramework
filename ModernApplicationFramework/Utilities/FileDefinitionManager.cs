using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Interfaces.Utilities;

namespace ModernApplicationFramework.Utilities
{
    [Export(typeof(FileDefinitionManager))]
    public class FileDefinitionManager
    {
#pragma warning disable 649
        [ImportMany] private ISupportedFileDefinition[] _supportedFileDefinitions;
#pragma warning restore 649

        public IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions
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
            return SupportedFileDefinitions.FirstOrDefault(l => l.FileType.FileExtension.Contains(extension));
        }
    }
}