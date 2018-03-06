using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.WindowManagement.LayoutState
{
    internal class LayoutItemStatePersister : ILayoutItemStatePersister
    {
        private IDockingHostViewModel _dockingHostViewModel;
        private IDockingHost _dockingHost;
        private bool _initialized;

        public static LayoutItemStatePersister Instance { get; private set; }

        public LayoutItemStatePersister()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _dockingHostViewModel = IoC.Get<IDockingHostViewModel>();
            if (_dockingHostViewModel is ViewAware va)
                va.ViewAttached += Va_ViewAttached;
        }

        private void Va_ViewAttached(object sender, ViewAttachedEventArgs e)
        {
            _dockingHost = _dockingHostViewModel.DockingHostView;
            _initialized = true;
        }

        public void SaveToStream(Stream stream, ProcessStateOption option)
        {
            InternalSaveState(option, stream);
        }

        public void SaveToFile(string filePath, ProcessStateOption option)
        {
            var stream = CreateFileStream(FileHandleMode.Create, filePath);
            InternalSaveState(option, stream);
        }

        public void LoadFromStream(Stream stream, ProcessStateOption option)
        {
            InternalLoadState(option, stream);
        }

        public void LoadFromFile(string filePath, ProcessStateOption option)
        {
            var stream = CreateFileStream(FileHandleMode.Open, filePath);
            InternalLoadState(option, stream);
        }

        private void InternalLoadState(ProcessStateOption processOption, Stream inputStream)
        {
            if (!_initialized)
                throw new InvalidOperationException("Layout State Persister not initialized");

            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (processOption.HasFlag(ProcessStateOption.DocumentsOnly) && processOption.HasFlag(ProcessStateOption.ToolsOnly) ||
                processOption.HasFlag(ProcessStateOption.Complete) && (processOption.HasFlag(ProcessStateOption.ToolsOnly) ||
                                                                       processOption.HasFlag(ProcessStateOption.ToolsOnly)))
                throw new InvalidEnumArgumentException();

            var layoutItems = new Dictionary<string, ILayoutItemBase>();

            try
            {
                using (var reader = new BinaryReader(inputStream))
                {

                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        var typeName = reader.ReadString();
                        var contentId = reader.ReadString();
                        var stateEndPosition = reader.ReadInt64();
                        stateEndPosition += reader.BaseStream.Position;

                        var contentType = Type.GetType(typeName);
                        var skipStateData = true;

                        if (contentType != null)
                        {
                            if (IoC.GetInstance(contentType, null) is ILayoutItemBase contentInstance)
                            {
                                if (contentInstance is ITool && !processOption.HasFlag(ProcessStateOption.DocumentsOnly) ||
                                    contentInstance is ILayoutItem && !processOption.HasFlag(ProcessStateOption.ToolsOnly))
                                    layoutItems.Add(contentId, contentInstance);

                                try
                                {
                                    contentInstance.LoadState(reader);
                                    skipStateData = false;
                                }
                                catch
                                {
                                    skipStateData = true;
                                }
                            }
                        }

                        // Skip state data block if we couldn't read it.
                        if (skipStateData)
                            reader.BaseStream.Seek(stateEndPosition, SeekOrigin.Begin);
                    }

                    if (layoutItems.Count == 0)
                    {
                        if (processOption == ProcessStateOption.DocumentsOnly || processOption == ProcessStateOption.DocumentsOnly)
                            _dockingHostViewModel.LayoutItems.Clear();
                        if (processOption == ProcessStateOption.ToolsOnly ||
                            processOption == ProcessStateOption.Complete)
                            _dockingHostViewModel.Tools.Clear();
                        return;
                    }
                    var active = _dockingHostViewModel.ActiveItem; 
                    _dockingHost.LoadLayout(reader.BaseStream, _dockingHostViewModel.ShowTool, _dockingHostViewModel.OpenLayoutItem, layoutItems);
                    if (_dockingHostViewModel.LayoutItems.Contains(active))
                        _dockingHostViewModel.OpenLayoutItem(active);
                }
            }
            catch
            {
                inputStream.Close();
            }


        }

        private void InternalSaveState(ProcessStateOption processOption, Stream inputStream)
        {
            if (!_initialized)
                throw new InvalidOperationException();

            if (inputStream == null)
                throw new ArgumentNullException(nameof(inputStream));

            if (processOption.HasFlag(ProcessStateOption.DocumentsOnly) && processOption.HasFlag(ProcessStateOption.ToolsOnly) ||
                processOption.HasFlag(ProcessStateOption.Complete) && (processOption.HasFlag(ProcessStateOption.ToolsOnly) ||
                                                                       processOption.HasFlag(ProcessStateOption.ToolsOnly)))
                throw new InvalidEnumArgumentException();


            try
            {
                var writer = new BinaryWriter(inputStream);
                var itemStates = _dockingHostViewModel.LayoutItems.Concat(_dockingHostViewModel.Tools.Cast<ILayoutItemBase>());
                var itemCount = 0;
                // reserve some space for items count, it'll be updated later
                writer.Write(itemCount);

                foreach (var item in itemStates)
                {
                    if (item is ILayoutItem && processOption.HasFlag(ProcessStateOption.ToolsOnly))
                        continue;
                    if (item is ITool && processOption.HasFlag(ProcessStateOption.DocumentsOnly))
                        continue;
                    if (processOption.HasFlag(ProcessStateOption.UseShouldReopenOnStart) && !item.ShouldReopenOnStart)
                        continue;

                    var itemType = item.GetType();
                    var exportAttributes = itemType
                        .GetCustomAttributes(typeof(ExportAttribute), false)
                        .Cast<ExportAttribute>()
                        .ToList();

                    var layoutType = typeof(ILayoutItemBase);
                    // get exports with explicit types or names that inherit from ILayoutItemBase
                    var exportTypes = (from att in exportAttributes
                                           // select the contract type if it is of type ILayoutitem. else null
                                       let typeFromContract = att.ContractType != null
                                                              && layoutType.IsAssignableFrom(att.ContractType)
                                           ? att.ContractType
                                           : null
                                       // select the contract name if it is of type ILayoutItemBase. else null
                                       let typeFromQualifiedName = GetTypeFromContractNameAsILayoutItem(att)
                                       // select the viewmodel type if it is of type ILayoutItemBase. else null
                                       let typeFromViewModel =
                                       layoutType.IsAssignableFrom(itemType) ? itemType : null
                                       // att.ContractType overrides att.ContractName if both are set.
                                       // fall back to the ViewModel type of neither are defined.
                                       let type = typeFromContract ?? typeFromQualifiedName ?? typeFromViewModel
                                       where type != null
                                       select type).ToList();

                    // throw exceptions here, instead of failing silently. These are design time errors.
                    var firstExport = exportTypes.FirstOrDefault();
                    if (firstExport == null)
                        throw new InvalidOperationException(
                            $"A ViewModel that participates in LayoutItem.ShouldReopenOnStart must be decorated with an ExportAttribute who's ContractType that inherits from ILayoutItemBase, infringing type is {itemType}.");
                    if (exportTypes.Count > 1)
                        throw new InvalidOperationException(
                            $"A ViewModel that participates in LayoutItem.ShouldReopenOnStart can't be decorated with more than one ExportAttribute which inherits from ILayoutItemBase. infringing type is {itemType}.");

                    var selectedTypeName = firstExport.AssemblyQualifiedName;

                    if (string.IsNullOrEmpty(selectedTypeName))
                        throw new InvalidOperationException(
                            $"Could not retrieve the assembly qualified type name for {firstExport}, most likely because the type is generic.");
                    // TODO: it is possible to save generic types. It requires that every generic parameter is saved, along with its position in the generic tree... A lot of work.

                    writer.Write(selectedTypeName);
                    writer.Write(item.ContentId);

                    // Here's the tricky part. Because some items might fail to save their state, or they might be removed (a plug-in assembly deleted and etc.)
                    // we need to save the item's state size to be able to skip the data during deserialization.
                    // Save current stream position. We'll need it later.
                    var stateSizePosition = writer.BaseStream.Position;

                    // Reserve some space for item state size
                    writer.Write(0L);

                    long stateSize;

                    try
                    {
                        var stateStartPosition = writer.BaseStream.Position;
                        item.SaveState(writer);
                        stateSize = writer.BaseStream.Position - stateStartPosition;
                    }
                    catch
                    {
                        stateSize = 0;
                    }

                    // Go back to the position before item's state and write the actual value.
                    writer.BaseStream.Seek(stateSizePosition, SeekOrigin.Begin);
                    writer.Write(stateSize);

                    if (stateSize > 0)
                        writer.BaseStream.Seek(0, SeekOrigin.End);

                    itemCount++;
                }

                writer.BaseStream.Seek(0, SeekOrigin.Begin);
                writer.Write(itemCount);
                writer.BaseStream.Seek(0, SeekOrigin.End);

                _dockingHost.SaveLayout(writer.BaseStream);
            }
            catch
            {
                inputStream.Close();
            }
        }

        private static Type GetTypeFromContractNameAsILayoutItem(ExportAttribute attribute)
        {
            if (attribute == null)
                return null;

            string typeName;
            if ((typeName = attribute.ContractName) == null)
                return null;

            var type = Type.GetType(typeName);
            if (type == null || !typeof(ILayoutItemBase).IsInstanceOfType(type))
                return null;
            return type;
        }

        private Stream CreateFileStream(FileHandleMode mode, string filePath)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));
            if (mode == FileHandleMode.Open)
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException(nameof(filePath));
                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            var absolutePath = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(absolutePath))
                throw new ArgumentNullException(nameof(absolutePath));
            if (!Directory.Exists(absolutePath))
                Directory.CreateDirectory(absolutePath);
            return new FileStream(filePath, FileMode.Create, FileAccess.Write);
        }

        private enum FileHandleMode
        {
            Open,
            Create
        }
    }
}