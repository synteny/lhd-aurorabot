using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Telegram.Bot.Examples.DotNetCoreWebHook
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://localhost:{Environment.GetEnvironmentVariable("PORT") ?? "5000"}")
                .Build();
    }
}
