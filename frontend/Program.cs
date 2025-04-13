// See https://aka.ms/new-console-template for more information

using Avalonia;
using Avalonia.ReactiveUI;
using frontend.views;

namespace frontend
{
    class Program
    {
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}