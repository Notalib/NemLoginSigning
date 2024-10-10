using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

using NemLoginSigningService.Services;
using NemLoginSignatureValidationService.Service;
using NemLoginSigningCore.Configuration;

using NemLoginSigningWebApp.Config;
using NemLoginSigningWebApp.Logic;
using NemLoginSigningWebApp.Utils;

namespace NemLoginSigningWebApp;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // Implementation needs a httpcontext accessor, Add method does a try add.
        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

        // Configuration dependencies
        IConfigurationSection configurationSection = Configuration.GetSection("NemloginConfiguration");
        services.Configure<NemloginConfiguration>(configurationSection);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        services.AddTransient<ISignersDocumentLoader, SignersDocumentLoader>();
        services.AddTransient<ISigningPayloadService, SigningPayloadService>();
        services.AddTransient<ITransformationPropertiesService, TransformationPropertiesService>();
        services.AddTransient<IDocumentSigningService, DocumentSigningService>();
        services.AddTransient<ISigningValidationService, SigningValidationService>();

        NemloginConfiguration nemloginConfiguration = configurationSection.Get<NemloginConfiguration>();

        X509Certificate2 ocesCertificate = new X509Certificate2(nemloginConfiguration.SignatureKeysConfiguration.KeystorePath,
            nemloginConfiguration.SignatureKeysConfiguration.PrivateKeyPassword);

        Log.Information("Loaded OCES Certificate {SubjectName}, has {PrivateKeySize} bit key.", ocesCertificate.Subject, ocesCertificate.GetRSAPrivateKey().KeySize);

        // Configure HTTPClients
        services.AddHttpClient("ValidationServiceClient", c => c.BaseAddress = new System.Uri(nemloginConfiguration.ValidationServiceURL));
        services.AddHttpClient<IUUIDMatchClient, UUIDMatchClient>(c =>
        {
            c.BaseAddress = new Uri(nemloginConfiguration.UUIDMatchServiceURL);
        }).ConfigurePrimaryHttpMessageHandler(() =>
        {
            // Requires authenticating with the NemLog-In registered VOCES/FOCES cert as the TLS client certificate
            HttpClientHandler handler = new();
            handler.ClientCertificates.Add(ocesCertificate);

            return handler;
        });

        services.AddTransient<ISigningValidationService, SigningValidationService>();

        CorsConfig cors = Configuration.GetSection("CORS").Get<CorsConfig>();

        if (cors?.AllowedOrigins is not null && cors.AllowedOrigins.Any())
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins(cors.AllowedOrigins);
                    });
            });
        }

        LoadAssemblies();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSerilogRequestLogging();

        app.UseRouting();

        app.UseCors();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private void LoadAssemblies()
    {
        // Load assembly to be able to do reflection later on
        Assembly.Load("NemLoginSigningXades"); // , Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
        Assembly.Load("NemLoginSigningPades"); // , Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    }
}
