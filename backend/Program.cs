// See https://aka.ms/new-console-template for more information

using System.Reflection;
using backend.network;
using ConsoleApp1.repository;
using ConsoleApp1.service;
using ConsoleApp1.utils;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;


namespace backend
{
    class Program
    {
        static void Main(string[] args)
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
        
            ClientManager clientManager = new ClientManager();
            SportsTicketManagementService service = new SportsTicketManagementService(cashierRepo, gameRepo, ticketRepo, clientManager);
            RequestHandler requestHandler = new RequestHandler(service);
            JSONServer server = new JSONServer(service, requestHandler);
            server.Run();
        }
    }

}