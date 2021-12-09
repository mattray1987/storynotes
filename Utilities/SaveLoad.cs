using StoryNotes.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StoryNotes.Utilities
{
    internal static class SaveLoad
    {
        public static List<Character> LoadCharacters(string filePath)
        {
            List<Character> list = new List<Character>();
            if (filePath != null)
            {
                StreamReader sr = new StreamReader(filePath);
                string contents = sr.ReadToEnd();
                list = JsonSerializer.Deserialize<List<Character>>(contents);
            }
            return list;
        }

    }
}
