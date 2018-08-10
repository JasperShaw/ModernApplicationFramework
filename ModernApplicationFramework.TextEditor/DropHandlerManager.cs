using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Text.Data;
using ModernApplicationFramework.Text.Data.Projection;
using ModernApplicationFramework.Text.Ui.Editor;
using ModernApplicationFramework.Text.Ui.Editor.DragDrop;
using ModernApplicationFramework.TextEditor.Utilities;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Core;

namespace ModernApplicationFramework.TextEditor
{
    internal class DropHandlerManager
    {
        internal static Dictionary<string, IList<Lazy<IDropHandlerProvider, IDropHandlerMetadata>>> DropHandlers;
        private readonly ITextView _textView;
        private readonly GuardedOperations _guardedOperations;

        public DropHandlerManager(IList<Lazy<IDropHandlerProvider, IDropHandlerMetadata>> dropHandlers, ITextView textView, GuardedOperations guardedOperations)
        {
            if (dropHandlers == null)
                throw new ArgumentNullException(nameof(dropHandlers));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _guardedOperations = guardedOperations;
            if (DropHandlers != null)
                return;
            DropHandlers = PopulateDropHandlers(dropHandlers);
        }

        public virtual IDropHandler GetSupportingHandler(DragDropInfo dragDropInfo)
        {
            return GetPrioritizedDropHandlers(dragDropInfo).FirstOrDefault(handler => handler.IsDropEnabled(dragDropInfo));
        }

        internal ICollection<IDropHandler> GetPrioritizedDropHandlers(DragDropInfo dragDropInfo)
        {
            var data = dragDropInfo.Data;
            var contentTypesAtPoint = GetContentTypesAtPoint(dragDropInfo.VirtualBufferPosition.Position);
            var prioritizedFormatList = GetPrioritizedFormatList(data);
            var dropHandlerList = new List<IDropHandler>();
            var dropHandlerSet = new HashSet<IDropHandler>();
            foreach (var key in prioritizedFormatList)
            {
                if (DropHandlers.ContainsKey(key))
                {
                    foreach (var provider in DropHandlers[key])
                    {
                        if (provider.Metadata.ContentTypes == null || ContentTypeMatch(contentTypesAtPoint, provider.Metadata.ContentTypes))
                        {
                            var dropHandler = _guardedOperations.InstantiateExtension(provider, provider, dropHandlerProvider => dropHandlerProvider.GetAssociatedDropHandler(_textView));
                            if (dropHandler != null && !dropHandlerSet.Contains(dropHandler))
                            {
                                dropHandlerList.Add(dropHandler);
                                dropHandlerSet.Add(dropHandler);
                            }
                        }
                    }
                }
            }
            return dropHandlerList;
        }

        private static bool ContentTypeMatch(List<IContentType> targetContentTypes, IEnumerable<string> extensionContentTypes)
        {
            foreach (var targetContentType in targetContentTypes)
            {
                if (ExtensionSelector.ContentTypeMatch(targetContentType, extensionContentTypes))
                    return true;
            }
            return false;
        }

        private static List<IContentType> GetContentTypesAtPoint(SnapshotPoint point)
        {
            var contentTypes = new List<IContentType>();
            GetContentTypesAtPoint(point, contentTypes);
            return contentTypes;
        }

        private static void GetContentTypesAtPoint(SnapshotPoint point, ICollection<IContentType> contentTypes)
        {
            if (!contentTypes.Contains(point.Snapshot.ContentType))
                contentTypes.Add(point.Snapshot.ContentType);
            if (!(point.Snapshot is IProjectionSnapshot snapshot))
                return;
            foreach (var sourceSnapshot in snapshot.MapToSourceSnapshots(point.Position))
                GetContentTypesAtPoint(sourceSnapshot, contentTypes);
        }

        private static List<string> GetPrioritizedFormatList(IDataObject data)
        {
            var stringList = new List<string>(data.GetFormats());
            stringList.Sort(new FormatSorter());
            return stringList;
        }

        private static Dictionary<string, IList<Lazy<IDropHandlerProvider, IDropHandlerMetadata>>> PopulateDropHandlers(IList<Lazy<IDropHandlerProvider, IDropHandlerMetadata>> dropHandlers)
        {
            var dictionary = new Dictionary<string, IList<Lazy<IDropHandlerProvider, IDropHandlerMetadata>>>();
            foreach (var lazy in Orderer.Order(dropHandlers))
            {
                foreach (var dropFormat in lazy.Metadata.DropFormats)
                {
                    if (dictionary.ContainsKey(dropFormat))
                        dictionary[dropFormat].Add(lazy);
                    else
                        dictionary.Add(dropFormat, new List<Lazy<IDropHandlerProvider, IDropHandlerMetadata>>(1)
                        {
                            lazy
                        });
                }
            }
            return dictionary;
        }

        internal class FormatSorter : IComparer<string>
        {
            internal Dictionary<string, int> FormatMap;

            internal FormatSorter()
            {
                FormatMap = new Dictionary<string, int>(23)
                {
                    {DataFormats.FileDrop, 23},
                    {DataFormats.EnhancedMetafile, 22},
                    {DataFormats.WaveAudio, 21},
                    {DataFormats.Riff, 20},
                    {DataFormats.Dif, 19},
                    {DataFormats.Locale, 18},
                    {DataFormats.Palette, 17},
                    {DataFormats.PenData, 16},
                    {DataFormats.Serializable, 15},
                    {DataFormats.SymbolicLink, 14},
                    {DataFormats.Xaml, 13},
                    {DataFormats.XamlPackage, 12},
                    {DataFormats.Tiff, 11},
                    {DataFormats.Bitmap, 10},
                    {DataFormats.Dib, 9},
                    {DataFormats.MetafilePicture, 8},
                    {DataFormats.CommaSeparatedValue, 7},
                    {DataFormats.StringFormat, 6},
                    {DataFormats.Html, 5},
                    {DataFormats.Rtf, 4},
                    {DataFormats.UnicodeText, 3},
                    {DataFormats.OemText, 2},
                    {DataFormats.Text, 1}
                };
            }

            public int Compare(string x, string y)
            {
                if (x != null && y != null && x.Equals(y, StringComparison.OrdinalIgnoreCase))
                    return 0;
                if (!FormatMap.ContainsKey(x))
                    return !FormatMap.ContainsKey(y) ? 0 : -1;
                return !FormatMap.ContainsKey(y) || FormatMap[x] <= FormatMap[y] ? 1 : -1;
            }
        }
    }
}