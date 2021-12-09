using StoryNotes.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace StoryNotes.Models
{
    public class Story : BindableBase, ICloneable
    {
        private string title;
        private string description;
        private string logline;
        private ObservableCollection<Character> characters = new();
        private ObservableCollection<StorySegment> storySegments = new();
        public int Id { get; set; }
        public bool IsChanged { get; set; } = false;
        public string Title
        {
            get { return title; }
            set 
            { 
                SetProperty(ref title, value); 
                IsChanged = true;
            }
        }
        public string Description
        {
            get { return description; }
            set 
            { 
                SetProperty(ref description, value); 
                IsChanged = true;
            }
        }
        public string Logline
        {
            get { return logline; }
            set 
            { 
                SetProperty(ref logline, value);
                IsChanged = true;
            }
        }
        public ObservableCollection<Character> Characters
        {
            get => characters;
            set { SetProperty(ref characters, value); }
        }
        public ObservableCollection<StorySegment> StorySegments
        {
            get => storySegments;
            set { SetProperty(ref storySegments, value); }
        }

        public Story()
        {
            StorySegments.Add(new StorySegment() { Title = "Your Story" });
        }

        public object Clone()
        {
            Story clone = new Story();
            clone.StorySegments.Clear();
            clone.Title = Title;
            clone.Description = Description;
            clone.Logline = Logline;
            if (Characters.Count > 0)
            {
                foreach(Character character in Characters)
                {
                    clone.Characters.Add((Character)character.Clone());
                }
            }
            if (StorySegments.Count > 0)
            {
                foreach (StorySegment segment in StorySegments)
                {
                    clone.StorySegments.Add((StorySegment)segment.Clone());
                }
            }
            return clone;
        }
    }
}
