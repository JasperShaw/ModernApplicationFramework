﻿using System.Collections.Generic;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Settings.Interfaces;

namespace ModernApplicationFramework.EditorBase.Interfaces.Settings.EditorAssociation
{
    public interface IEditorFileAssociationSettings : ISettingsDataModel
    {
        IEditor GetAssociatedEditor(ISupportedFileDefinition association);

        void CreateAssociation(IEditor editor, ISupportedFileDefinition association);

        void CreateAssociations(IEditor editor, IEnumerable<ISupportedFileDefinition> associations,
            bool ignoreDuplicateCheck);

        void CreateDefaultAssociations(IEditor editor);
    }
}