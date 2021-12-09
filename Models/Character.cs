using StoryNotes.Utilities;
using System;

namespace StoryNotes.Models
{
    public class Character : BindableBase, ICloneable
    {
        private string name;
        private string description;
        public string Name
        {
            get => name;
            set
            {
                SetProperty(ref name, value);
            }
        }
        public string Description
        {
            get => description;
            set { SetProperty(ref description, value); }
        }
        public int StoryId { get; set; }
        public Character()
        {
            Name = "";
            Description = "";
        }
        public Character(string newName, int storyId)
        {
            Name = newName;
            StoryId = storyId;
            Description = "";
        }
        public override string ToString()
        {
            return Name;
        }
        public bool Equals(Character character)
        {
            if(character.Name == Name && character.Description == Description)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object Clone()
        {
            Character clone = new Character();
            clone.Name = Name;
            clone.Description = Description;

            return clone;
        }
    }
}
