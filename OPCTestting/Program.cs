// See https://aka.ms/new-console-template for more information
using Middleware.controller;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

/*Console.WriteLine("Hello, World!");


OpcController OpcController = new OpcController();
Console.WriteLine("hello");
OpcController.Progress();

//// 保持控制台应用程序运行
Console.WriteLine("按任意键停止服务...");
Console.ReadKey();*/
public class Program
{

    public static bool run = false;
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>().UseUrls("http://0.0.0.0:8080");
            });

}