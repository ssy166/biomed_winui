using Microsoft.UI.Xaml.Data;
using System;

namespace biomed.Converters
{
    public class GreaterThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return false;

            if (double.TryParse(value.ToString(), out double val) && 
                double.TryParse(parameter.ToString(), out double param))
            {
                return val > param;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
} 