﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutUtilities
{
    [Export(typeof(ILayoutItemStatePersister))]
    public class LayoutItemStatePersister : ILayoutItemStatePersister
    {
        public void LoadState(IDockingHostViewModel shell, IDockingHost shellView, string fileName)
        {
            var layoutItems = new Dictionary<string, ILayoutItemBase>();

            if (!File.Exists(fileName))
                return;

            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                using (var reader = new BinaryReader(stream))
                {
                    stream = null;

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
                            var contentInstance = IoC.GetInstance(contentType, null) as ILayoutItemBase;

                            if (contentInstance != null)
                            {
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

                    shellView.LoadLayout(reader.BaseStream, shell.ShowTool, shell.OpenDocument, layoutItems);
                }
            }
            catch
            {
                stream?.Close();
            }
        }

        public void SaveState(IDockingHostViewModel shell, IDockingHost shellView, string fileName)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

                using (var writer = new BinaryWriter(stream))
                {
                    stream = null;

                    var itemStates = shell.Documents.Concat(shell.Tools.Cast<ILayoutItemBase>());

                    var itemCount = 0;
                    // reserve some space for items count, it'll be updated later
                    writer.Write(itemCount);

                    foreach (var item in itemStates)
                    {
                        if (!item.ShouldReopenOnStart)
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
                            // select the viewmodel tpye if it is of type ILayoutItemBase. else null
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

                    shellView.SaveLayout(writer.BaseStream);
                }
            }
            catch
            {
                stream?.Dispose();
            }
        }

        private Type GetTypeFromContractNameAsILayoutItem(ExportAttribute attribute)
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
    }
}