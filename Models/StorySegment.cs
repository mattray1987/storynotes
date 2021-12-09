using StoryNotes.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StoryNotes.Models
{
    public class StorySegment : BindableBase, ICloneable
    {
        private string purpose;
        private string title;
        private string description;
        private ObservableCollection<StorySegment> subSegments = new();
        private ObservableCollection<Character> characters = new();
        public event EventHandler ChildChanged;

        public string Purpose
        {
            get { return purpose; }
            set { SetProperty(ref purpose, value); }
        }
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
        public ObservableCollection<StorySegment> StorySegments
        {
            get { return subSegments; }
            set { SetProperty(ref subSegments, value); }
        }
        public ObservableCollection<Character> Characters
        {
            get { return characters; }
            set { SetProperty(ref characters, value); }
        }

        public StorySegment()
        {
            StorySegments.CollectionChanged += StorySegments_CollectionChanged;
            foreach (StorySegment storySegment in StorySegments)
            {
                storySegment.ChildChanged += OnChildChanged;
                storySegment.PropertyChanged += OnChildChanged;
            }
        }

        private void StorySegments_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnChildChanged(this, null);
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach(StorySegment storySegment in e.NewItems)
                {
                    storySegment.ChildChanged += OnChildChanged;
                    storySegment.PropertyChanged += OnChildChanged;
                }
            }
        }

        public object Clone()
        {
            StorySegment clone = new StorySegment();
            clone.purpose = Purpose;
            clone.title = Title;
            clone.description = Description;
            if (StorySegments.Count > 0)
            {
                foreach (StorySegment child in StorySegments)
                {
                    StorySegment childClone = (StorySegment)child.Clone();
                    clone.subSegments.Add(childClone);
                }
            }
            return clone;
        }
        protected virtual void OnChildChanged(object sender, EventArgs e)
        {
            ChildChanged?.Invoke(this, e);
        }
        public void Add(StorySegment subSegment)
        {
            subSegment.PropertyChanged += OnChildChanged;
            StorySegments.Add(subSegment);
        }

    }
}
