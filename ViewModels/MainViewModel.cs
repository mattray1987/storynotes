using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StoryNotes.Models;
using StoryNotes.Utilities;
using StoryNotes.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StoryNotes.ViewModels
{
    public class MainViewModel : BindableBase
    {
        #region Fields
        private ObservableCollection<Character> filteredCharacters = new ObservableCollection<Character>();
        private string searchTerm;
        private Page mainPage;
        private Type selectedControl = typeof(GeneralInfoPage);
        private Story selectedStory;
        private Story unchangedStory;
        private Character selectedCharacter;
        private AppData appData = new AppData();
        private List<(string Tag, Type Page)> directory = new List<(string Tag, Type Page)>()
        {
            ("Characters", typeof(CharacterPage)),
            ("General", typeof(GeneralInfoPage)),
            ("Outline", typeof(OutlinePage)),
        };
        #endregion

        #region Properties
        public ObservableCollection<Story> AllStories { get; private set; } = new ObservableCollection<Story>();
        public ObservableCollection<Character> FilteredCharacters
        {
            get { return filteredCharacters; }
            set { SetProperty(ref filteredCharacters, value); }
        }
        public string SearchStoryCharacters
        {
            get { return searchTerm; }
            set
            {
                SetProperty(ref searchTerm, value);
                FilterCharacters();
            }
        }
        public Page MainPage
        {
            get { return mainPage; }
            set { SetProperty(ref mainPage, value); }
        }
        public Story SelectedStory
        {
            get { return selectedStory; }
            set
            {
                SetProperty(ref selectedStory, value);
                if (SelectedStory != null)
                {
                    unchangedStory = (Story)SelectedStory.Clone();
                    ((RelayCommand)EditStoryCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteStoryCommand).RaiseCanExecuteChanged();
                    FilterCharacters();

                    SelectedStory.Characters.CollectionChanged += MarkStoryChanged;
                    SelectedStory.StorySegments.CollectionChanged += MarkStoryChanged;
                    foreach(StorySegment storySegment in selectedStory.StorySegments)
                    {
                        storySegment.PropertyChanged += MarkStoryChanged;
                        storySegment.ChildChanged += MarkStoryChanged;
                    }
                    SelectedStory.IsChanged = false;
                }
            }
        }
        public Type SelectedControl
        {
            get => selectedControl;
            set
            {
                SetProperty(ref selectedControl, value);
            }
        }

        public Character SelectedCharacter
        {
            get => selectedCharacter;
            set
            {
                SetProperty(ref selectedCharacter, value);
                ((RelayCommand)DeleteCharacterCommand).RaiseCanExecuteChanged();
                if(SelectedCharacter != null)
                {
                    SelectedCharacter.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => SelectedStory.IsChanged = true;
                }
            }
        }
        public ICommand EditStoryCommand { get; set; }
        public ICommand DeleteStoryCommand { get; set; }
        public ICommand AddStoryCommand { get; set; }
        public ICommand SaveDataCommand { get; set; }
        public ICommand AddCharacterCommand { get; set; }
        public ICommand DeleteCharacterCommand { get; set; }
        #endregion

        #region Methods
        private void PopulateData()
        {
            LoadData();
            MainPage = new StorySelection();
        }
        private void LoadData()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string workbenchDataPath = Path.Combine(appDataFolder, "WriterWorkbench");

            string storyDataPath = Path.Combine(workbenchDataPath, "storyData.txt");
            if (File.Exists(storyDataPath))
            {
                StreamReader streamReader = new StreamReader(storyDataPath);
                string storyData = streamReader.ReadToEnd();
                streamReader.Close();
                if (storyData != null)
                {
                    AllStories = JsonSerializer.Deserialize<ObservableCollection<Story>>(storyData);
                }
            }

            string appDataPath = Path.Combine(workbenchDataPath, "appData.txt");
            if (File.Exists(appDataPath))
            {
                StreamReader streamReader = new StreamReader(appDataPath);
                string appData = streamReader.ReadToEnd();
                streamReader.Close();
                if (appData != null)
                {
                    this.appData = JsonSerializer.Deserialize<AppData>(appData);
                }
            }
        }
        public void SaveData()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string workbenchDataPath = Path.Combine(documentsFolder, "WriterWorkbench");
            Directory.CreateDirectory(workbenchDataPath);

            string storyDataPath = Path.Combine(workbenchDataPath, "storyData.txt");
            StreamWriter storyWriter = new StreamWriter(storyDataPath, false);
            string storyDataString = JsonSerializer.Serialize(AllStories);
            storyWriter.WriteLine(storyDataString);
            storyWriter.Close();

            string appDataPath = Path.Combine(workbenchDataPath, "appData.txt");
            StreamWriter appWriter = new StreamWriter(appDataPath, false);
            string appDataString = JsonSerializer.Serialize(appData);
            appWriter.WriteLine(appDataString);
            appWriter.Close();

            if(SelectedStory != null)
            {
                SelectedStory.IsChanged = false;
                unchangedStory = (Story)SelectedStory.Clone();
            }
        }
        private void FilterCharacters()
        {
            FilteredCharacters.Clear();
            if (SelectedStory != null)
            {
                foreach (Character character in SelectedStory.Characters)
                {
                    if (string.IsNullOrWhiteSpace(SearchStoryCharacters) ||
                        character.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    {
                        FilteredCharacters.Add(character);
                    }
                }
            }
        }
        public async Task NavigateHomeAsync(XamlRoot root)
        {
            if (SelectedStory.IsChanged)
            {
                ContentDialog backRequest = new()
                {
                    XamlRoot = root,
                    Title = "Save changes?",
                    Content = "Do you want to save your changes?",
                    PrimaryButtonText = "Save Changes",
                    SecondaryButtonText = "Discard Changes",
                    CloseButtonText = "Cancel"
                };
                ContentDialogResult contentDialogResult = await backRequest.ShowAsync();

                if (contentDialogResult == ContentDialogResult.Primary)
                {
                    SaveData();
                    MainPage = new StorySelection();
                }
                else if (contentDialogResult == ContentDialogResult.Secondary)
                {
                    var index = AllStories.IndexOf(SelectedStory);
                    AllStories[index] = unchangedStory;

                    MainPage = new StorySelection();
                }
                else
                {
                    return;
                }
            }
            MainPage = new StorySelection();
        }
        public void DiscardChanges()
        {
            SelectedStory = (Story)unchangedStory.Clone();
            SelectedStory.IsChanged = false;
        }
        public void NavigateContent(string tag)
        {
                var directoryEntry = directory.FirstOrDefault(p => p.Tag.ToString().Equals(tag));
                if (directoryEntry.Page != null)
                {
                    SelectedControl = directoryEntry.Page;
                }
        }
        private void NavigateStory()
        {
            MainPage = new StoryContents();
        }
        private void DeleteStory()
        {
            if (SelectedStory != null)
            {
                AllStories.Remove(SelectedStory);
                if (MainPage.GetType() == typeof(StoryContents))
                {
                    MainPage = new StorySelection();
                }
            }
        }
        private void AddStory()
        {
            Story story = new Story()
            {
                Title = "New Story",
                Id = appData.StoryID
            };
            appData.StoryID++;
            AllStories.Add(story);
            SelectedStory = story;
            MainPage = new StoryContents();
            SelectedStory.IsChanged = true;
        }
        private bool CanEditDeleteStory() => SelectedStory != null;
        private void AddCharacter()
        {
            if (SelectedStory != null)
            {
                Character character = new Character()
                {
                    Name = "New Character",
                    StoryId = SelectedStory.Id,
                };
                SelectedStory.Characters.Add(character);
                FilterCharacters();
                SelectedCharacter = character;
            }
        }
        
        private void DeleteCharacter()
        {
            if (SelectedStory != null)
            {
                SelectedStory.Characters.Remove(SelectedCharacter);
                FilterCharacters();
            }
        }
        private bool CanDeleteCharacter() => SelectedCharacter != null;
        private void MarkStoryChanged(object sender, EventArgs e)
        {
            SelectedStory.IsChanged = true;
        }
        public MainViewModel()
        {
            PopulateData();
            EditStoryCommand = new RelayCommand(NavigateStory, CanEditDeleteStory);
            DeleteStoryCommand = new RelayCommand(DeleteStory, CanEditDeleteStory);
            AddStoryCommand = new RelayCommand(AddStory);
            AddCharacterCommand = new RelayCommand(AddCharacter);
            DeleteCharacterCommand = new RelayCommand(DeleteCharacter, CanDeleteCharacter);
            SaveDataCommand = new RelayCommand(SaveData);
        }
        #endregion
    }
}
