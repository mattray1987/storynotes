using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace StoryNotes.ViewModels
{
    internal class NullToHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
