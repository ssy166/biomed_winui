using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using System;

namespace biomed.Converters
{
    public class StringUrlToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string url && !string.IsNullOrEmpty(url))
            {
                try
                {
                    return new BitmapImage(new Uri(url));
                }
                catch (UriFormatException)
                {
                    // Handle malformed URLs gracefully
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 