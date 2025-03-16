// See https://aka.ms/new-console-template for more information

using ConsoleApp1.dto;
using ConsoleApp1.repository;
using ConsoleApp1.utils;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())  
    .AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true) 
    .Build();


string connectionString = configuration.GetSection("ConnectionStrings:MyDbConnection").Value;

var dbutils = new DbUtils(connectionString);

var cashierDbRepository = new CashierDBRepository(dbutils);

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

