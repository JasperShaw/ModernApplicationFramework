using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.FileSupport.Exceptions;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.Native.Platform.Enums;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public static class OpenFileHelper
    {
        internal static void OpenSupportedFiles(IReadOnlyCollection<OpenFileArguments> files)
        {
            var editorProvider = IoC.Get<IEditorProvider>();
            try
            {
                foreach (var file in files)
                    editorProvider.Open(file);
            }
            catch (FileNotSupportedException exception)
            {
                MessageBox.Show(exception.Message, IoC.Get<IEnvironmentVariables>().ApplicationName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void OpenUnsupportedFiles(IReadOnlyCollection<OpenFileArguments> files)
        {
            var flag = false;
            foreach (var file in files)
            {
                var fileType = NativeMethods.NativeMethods.GetExeType(file.Path);
                if (fileType == ShellFileType.Unknown)
                    continue;
                if (fileType == ShellFileType.Windows || fileType == ShellFileType.Dos ||
                    fileType == ShellFileType.Console)
                {
                    flag = true;
                    continue;
                }
                if (NativeMethods.NativeMethods.GetExeType(file.Path) == ShellFileType.Unknown)
                    Process.Start(file.Path);
            }

            if (flag)
            {
                MessageBox.Show("Executables will not be opened", IoC.Get<IEnvironmentVariables>().ApplicationName,
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
