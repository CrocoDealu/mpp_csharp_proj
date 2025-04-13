using System.Globalization;
using Avalonia.Data.Converters;

namespace frontend.utils;

public class CapacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int capacity)
        {
            if (capacity > 0)
                return $"Available Seats: {capacity}";
            else
                return "Sold out";
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}