using System;
using System.Net;
using System.IO;
using WebServer.Controllers;

namespace WebServer
{
    class Program
    {
        private static bool _appIsRunning = true;

        static void Main()
        {
            //var result = new MyORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;")
            var db = new MyORM(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SteamDB;Integrated Security=True;");
            //db.ExecuteNonQuery("update Accounts set Login='Vanya', Password='4321' where Id=2");
            //db.Update(new Account() { Id = 2, Login = "Vanya2", Password = "4321" });
            //db.Delete<Account>(new Account() { Login = "New2", Password = "10239"});
            db.Select<Account>().ForEach(i => Console.WriteLine(i.Id));
            /*using var server = new HttpServer();
            server.Start();

            while (_appIsRunning)
            {
                HandleCommand(Console.ReadLine()?.ToLower(), server);
            }*/
        }

        private static void HandleCommand(string? command, HttpServer server)
        {
            switch (command)
            {
                case "start":
                    server.Start();
                    break;

                case "restart":
                    server.Stop();
                    server.Start();
                    break;

                case "stop":
                    server.Stop();
                    break;

                case "status":
                    Console.WriteLine(server.Status.ToString());
                    break;

                case "exit":
                    _appIsRunning = false;
                    break;
            }
        }
    }
}