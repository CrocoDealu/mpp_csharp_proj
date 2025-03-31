
using ConsoleApp1.dto;
using ConsoleApp1.repository;
using ConsoleApp1.utils;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using System;
using ConsoleApp1.service;
using ConsoleApp1.views;
using log4net;
using log4net.Config;



//


namespace ConsoleApp1
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

