using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StoryNotes.Models;
using StoryNotes.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StoryNotes.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OutlinePage : Page
    {
        public OutlinePage()
        {
            this.InitializeComponent();
        }
        public MainViewModel MainViewModel => App.MainViewModel;
        public void AddSubsection()
        {
            if (treeView.SelectedItem != null && treeView.SelectedNode != null)
            {
                if(treeView.SelectedNode.Depth < 10)
                {
                    (treeView.SelectedItem as StorySegment).Add(new StorySegment() { Title = "New Section" });
                    treeView.SelectedNode.IsExpanded = true;
                }
            }
            
        }
        private void AddSubsection_Click(object sender, RoutedEventArgs e)
        {
            AddSubsection();
        }
        private void DeleteSection()
        {
            var rootCollection = MainViewModel.SelectedStory.StorySegments;
            void FindAndDelete(ObservableCollection<StorySegment> segment, StorySegment target)
            {
                foreach (StorySegment child in segment)
                {
                    if (child == target)
                    {
                        segment.Remove(child);
                        return;
                    }
                    else if (child.StorySegments.Count > 0)
                    {
                        FindAndDelete(child.StorySegments, target);
                    }
                }
            }
            if (treeView.SelectedItem != null)
            {
                FindAndDelete(rootCollection, (StorySegment)treeView.SelectedItem);
                if (rootCollection.Count == 0)
                {
                    rootCollection.Add(new StorySegment() { Title = "Your Story" });
                }
            }
        }

        private void DeleteSection_Click(object sender, RoutedEventArgs e)
        {
            DeleteSection();
        }

        private void MenuFlyoutItem_Add(object sender, RoutedEventArgs e)
        {
            StorySegment clickedItem = (StorySegment)(e.OriginalSource as FrameworkElement)?.DataContext;
            if (clickedItem != null)
            {
                treeView.SelectedItem = clickedItem;
                AddSubsection();
            }
        }

        private void MenuFlyoutItem_Delete(object sender, RoutedEventArgs e)
        {
            StorySegment clickedItem = (StorySegment)(e.OriginalSource as FrameworkElement)?.DataContext;
            if (clickedItem != null)
            {
                treeView.SelectedItem = clickedItem;
                DeleteSection();
            }
        }
    }
}
