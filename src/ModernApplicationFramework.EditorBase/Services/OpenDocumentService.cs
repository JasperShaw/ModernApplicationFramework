using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Packages;
using ModernApplicationFramework.EditorBase.Interfaces.Services;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Services
{
    [Export(typeof(IOpenFileService))]
    public class OpenFileService : IOpenFileService
    {
        private readonly IEditorProvider _editorProvider;
        private readonly IMruFilePackage _mruFilePackage;

        [ImportingConstructor]
        public OpenFileService(IEditorProvider editorProvider, IMruFilePackage mruFilePackage)
        {
            _editorProvider = editorProvider;
            _mruFilePackage = mruFilePackage;
        }

        public void OpenFile(OpenFileArguments args)
        {
            if (_editorProvider.Handles(args.Path))
                _editorProvider.Open(args);
            else
            {
                var fileType = NativeMethods.NativeMethods.GetExeType(args.Path);
                if (fileType == ShellFileType.Windows || fileType == ShellFileType.Dos ||
                    fileType == ShellFileType.Console)
                {
                    MessageBox.Show("Executables will not be opened", IoC.Get<IEnvironmentVariables>().ApplicationName,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (NativeMethods.NativeMethods.GetExeType(args.Path) == ShellFileType.Unknown)
                    Process.Start(args.Path);
            }
            _mruFilePackage.Manager.AddItem($"{args.Path}|{args.Editor:B}");
        }

        public bool TryOpenFile(OpenFileArguments args)
        {
            try
            {
                if (string.IsNullOrEmpty(args.Path))
                    return false;
                if (!File.Exists(args.Path))
                    return false;
                OpenFile(args);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool TryOpenFile(string path, Guid editorGuid)
        {
            var fileDefinitionManager = IoC.Get<IFileDefinitionManager>();
            var fileDefinition = fileDefinitionManager.GetDefinitionByFilePath(path);
            return TryOpenFile(new OpenFileArguments(fileDefinition, path, editorGuid));
        }

        public bool TryOpenFile(string path)
        {
            return TryOpenFile(path, Guid.Empty);
        }

        public void OpenStandardEditor(string path)
        {
            OpenSpecificEditor(path, Guid.Empty);
        }

        public void OpenSpecificEditor(string path, Guid editorGuid)
        {
            var fileDefinitionManager = IoC.Get<IFileDefinitionManager>();
            var fileDefinition = fileDefinitionManager.GetDefinitionByFilePath(path);
            OpenFile(new OpenFileArguments(fileDefinition, path, editorGuid));
        }
    }
}
