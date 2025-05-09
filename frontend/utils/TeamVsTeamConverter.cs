using System.Globalization;
using Avalonia.Data.Converters;
using frontend.model;

namespace frontend.utils;

public class TeamVsTeamConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Game game)
            return $"{game.Team1} vs {game.Team2}";
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}