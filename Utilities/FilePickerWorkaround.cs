using Microsoft.UI.Xaml;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace StoryNotes.Utilities
{
    internal class FilePickerWorkaround
    {
        private async void HandleButtonClick()
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle((Application.Current as App).MainWindow);
            var folderPicker = new FolderPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
        }
    }
}
