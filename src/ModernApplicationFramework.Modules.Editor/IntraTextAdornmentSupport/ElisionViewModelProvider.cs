﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Logic;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities.Attributes;

namespace ModernApplicationFramework.Modules.Editor.IntraTextAdornmentSupport
{
    [Export(typeof(ITextViewModelProvider))]
    [TextViewRole("STRUCTURED")]
    [ContentType("text")]
    internal class ElisionViewModelProvider : ITextViewModelProvider
    {
        [Import] internal IProjectionBufferFactoryService ProjectionBufferFactory { get; set; }

        public ITextViewModel CreateTextViewModel(ITextDataModel dataModel, ITextViewRoleSet roles)
        {
            if (dataModel == null)
                throw new ArgumentNullException(nameof(dataModel));
            return new ElisionViewModel(dataModel, ProjectionBufferFactory);
        }
    }
}