using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

using NemLoginSigningWebApp.Utils;

namespace NemLoginSigningWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateBootstrapLogger();

            Log.Information("Host created - Starting up NemLoginSigningWebApp WebHost");

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((context, services, configuration) =>
                {
                    // Read serilog configuration from appsettings.json: https://github.com/serilog/serilog-settings-configuration
                    configuration.ReadFrom.Configuration(context.Configuration)
                                 .ReadFrom.Services(services)
                                 .Enrich.FromLogContext()
                                 .Enrich.WithCorrelationId()
                                 .Enrich.WithExceptionDetails()
                                 .Enrich.WithMachineName()
                                 .WriteTo.Console(new RenderedCompactJsonFormatter());
                });
    }
}