using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace frontend.utils;

public class CapacityColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int capacity && capacity == 0)
            return new SolidColorBrush(Colors.Red);
        return new SolidColorBrush(Colors.Black);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}