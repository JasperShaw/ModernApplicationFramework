using System;
using System.Collections.Generic;
using System.IO;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Docking.Layout.Serialization;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutManagement
{
    internal static class LayoutUtilities
    {
        public static void LoadLayout(DockingManager manager, Stream stream, Action<ILayoutItem> addDocumentCallback,
            Action<ITool> addToolCallback, Dictionary<string, ILayoutItemBase> items)
        {
            var layoutSerializer = new XmlLayoutSerializer(manager);
            layoutSerializer.LayoutSerializationCallback += (s, e) =>
            {
                ILayoutItemBase itemBase;
                if (items.TryGetValue(e.Model.ContentId, out itemBase))
                {
                    e.Content = itemBase;

                    var tool = itemBase as ITool;
                    var anchorable = e.Model as LayoutAnchorable;

                    var document = itemBase as ILayoutItem;
                    var layoutDocument = e.Model as LayoutDocument;

                    if (tool != null && anchorable != null)
                    {
                        addToolCallback(tool);
                        tool.IsVisible = anchorable.IsVisible;

                        if (anchorable.IsActive)
                            tool.Activate();

                        tool.IsSelected = e.Model.IsSelected;

                        return;
                    }

                    if (document != null && layoutDocument != null)
                    {
                        addDocumentCallback(document);

                        // Nasty hack to get around issue that occurs if documents are loaded from state,
                        // and more documents are opened programmatically.
                        layoutDocument.GetType()
                            .GetProperty("IsLastFocusedDocument")
                            .SetValue(layoutDocument, false, null);

                        document.IsSelected = layoutDocument.IsSelected;
                        return;
                    }
                }

                // Don't create any panels if something went wrong.
                e.Cancel = true;
            };
            try
            {
                layoutSerializer.Deserialize(stream);
            }
            catch
            {
                // ignored
            }
        }

        public static void SaveLayout(DockingManager manager, Stream stream)
        {
            var layoutSerializer = new XmlLayoutSerializer(manager);
            layoutSerializer.Serialize(stream);
        }
    }
}