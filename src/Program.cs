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
using Microsoft.AspNetCore.Builder;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;

namespace NemLoginSigningWebApp
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(new RenderedCompactJsonFormatter())
                    .CreateBootstrapLogger();

                WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

                // Configure DI and Services
                Startup startup = new(builder.Configuration);
                startup.ConfigureServices(builder.Services);

                // Read use serilog for logging and read configuration from appsettings.json: https://github.com/serilog/serilog-settings-configuration
                builder.Host.UseSerilog((context, services, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration)
                                 .ReadFrom.Services(services)
                                 .Enrich.FromLogContext()
                                 .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                                     .WithDefaultDestructurers())
                                 .Enrich.WithMachineName()
                                 .WriteTo.Console(new RenderedCompactJsonFormatter());
                }, preserveStaticLogger: false);

                // Build Web application, configure it, and finally run it.
                WebApplication app = builder.Build();
                startup.Configure(app, app.Environment);
                app.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Log.Fatal(e, "Host terminated unexpectedly");
                Log.CloseAndFlush();
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

            return 0;
        }
    }
}