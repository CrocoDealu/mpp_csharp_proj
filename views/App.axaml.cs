using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ConsoleApp1.repository;
using ConsoleApp1.service;
using ConsoleApp1.utils;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp1.views;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
            .Build();

        string connectionString = configuration.GetSection("ConnectionStrings:MyDbConnection").Value;
        var dbutils = new DbUtils(connectionString);
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));


        ICashierRepository cashierRepo = new CashierDBRepository(dbutils, configuration);
        IGameRepository gameRepo = new GameDBRepository(dbutils);
        ITicketRepository ticketRepo = new TicketDBRepository(dbutils, gameRepo, cashierRepo);
        
        SportsTicketManagementService service = new SportsTicketManagementService(cashierRepo, gameRepo, ticketRepo);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new LoginWindow(service);
        }

        base.OnFrameworkInitializationCompleted();
    }
}