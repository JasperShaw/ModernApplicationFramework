using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Editor.Commanding;
using ModernApplicationFramework.Editor.Implementation;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Editor.KeyGestureScope
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    internal sealed class KeyGestureScopeState : IPartImportsSatisfiedNotification
    {
        [ImportMany] public List<Lazy<GestureScope>> Scopes;

        [ImportMany]
        public List<Lazy<IEdtiorGestureScopeProvider, IOrderableContentTypeAndTextViewRoleMetadata>> GestureScopeProviders
        {
            get;
            set;
        }

        public IList<Lazy<IEdtiorGestureScopeProvider, IOrderableContentTypeAndTextViewRoleMetadata>> OrderedGestureScopeProviders { get; private set; }

        public void OnImportsSatisfied()
        {
            OrderedGestureScopeProviders = Orderer.Order(GestureScopeProviders);
        }
    }
}