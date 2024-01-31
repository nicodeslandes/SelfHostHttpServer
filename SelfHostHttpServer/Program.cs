using System;

namespace SelfHostHttpServer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Yo! I can process messages!");
        var server = new WebServer();
        server.Start();

        Console.ReadLine();
        server.Stop();
        Console.WriteLine("Bye");
    }
}
