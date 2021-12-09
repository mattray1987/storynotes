using Microsoft.UI.Xaml.Controls;
using StoryNotes.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StoryNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StorySelection : Page
    {
        public StorySelection()
        {
            this.InitializeComponent();
        }
        public MainViewModel MainViewModel => App.MainViewModel;

        private void ListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            MainViewModel.EditStoryCommand.Execute(null);
        }
    }
}
