
using ConsoleApp1.dto;
using ConsoleApp1.repository;
using ConsoleApp1.utils;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;




var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())  
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
    .Build();

string connectionString = configuration.GetSection("ConnectionStrings:MyDbConnection").Value;

var dbutils = new DbUtils(connectionString);

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly()!);
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var cashierDbRepository = new CashierDBRepository(dbutils, configuration);
// Cashier c = new Cashier(2, "David", "parola", "yoda");
// cashierDbRepository.Update(c);

foreach (var cashier in cashierDbRepository.FindAll())
{
    Console.WriteLine(cashier);
}

var gameDbRepository = new GameDBRepository(dbutils);


// gameDbRepository.Save(new Game(0, "Steaua", "Rapid", 10, 0, "Europa League", 50000, "Finals"));
foreach (var game in gameDbRepository.FindAll())
{
    Console.WriteLine(game);
}

var ticketDbRepository = new TicketDBRepository(dbutils, gameDbRepository, cashierDbRepository);

foreach (var ticket in ticketDbRepository.GetTicketsForClient(new ClientFilterDTO("", "Margaretelor")))
{
    Console.WriteLine(ticket);
}

