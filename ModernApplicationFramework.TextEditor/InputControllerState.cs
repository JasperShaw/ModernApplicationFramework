﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Text.Ui.Operations;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.TextEditor
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    internal sealed class InputControllerState : IPartImportsSatisfiedNotification
    {
        [Import]
        public IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        [ImportMany]
        public List<Lazy<IMouseProcessorProvider, IOrderableContentTypeAndTextViewRoleMetadata>> MouseProcessorProviders { get; set; }

        [ImportMany]
        public List<Lazy<IKeyProcessorProvider, IOrderableContentTypeAndTextViewRoleMetadata>> KeyProcessorProviders { get; set; } 

        public IList<Lazy<IMouseProcessorProvider, IOrderableContentTypeAndTextViewRoleMetadata>> OrderedMouseProcessorProviders { get; private set; }

        public IList<Lazy<IKeyProcessorProvider, IOrderableContentTypeAndTextViewRoleMetadata>> OrderedKeyProcessorProviders { get; private set; }

        public void OnImportsSatisfied()
        {
            OrderedMouseProcessorProviders = Orderer.Order(MouseProcessorProviders);
            OrderedKeyProcessorProviders = Orderer.Order(KeyProcessorProviders);
        }
    }
}