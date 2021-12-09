using Microsoft.UI.Xaml.Controls;
using StoryNotes.ViewModels;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StoryNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StoryContents : Page
    {
        public StoryContents()
        {
            this.InitializeComponent();
        }
        public MainViewModel MainViewModel => App.MainViewModel;

        private async void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            await MainViewModel.NavigateHomeAsync(this.XamlRoot);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedTag = args.SelectedItemContainer.Tag.ToString();
            if (selectedTag != null)
            {
                MainViewModel.NavigateContent(selectedTag);
            }
        }
    }
}
