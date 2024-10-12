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

using NemLoginSigningWebApp.Config;
using NemLoginSigningWebApp.Logic;
using NemLoginSigningWebApp.Utils;
using Nemlogin.QualifiedSigning.SDK.Core.Services;
using Nemlogin.QualifiedSigning.SDK.Core.Validations.PlainTextValidation;
using Nemlogin.QualifiedSigning.SDK.Core.Validations.XMLValidation;
using Nemlogin.QualifiedSigning.SDK.Core.Validations;
using Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;
using Nota.SystemTest;

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
        services.Configure<NemloginConfig>(configurationSection);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        services.AddTransient<IValidator, XMLValidator>();
        services.AddTransient<IValidator, PlainTextValidator>();
        services.AddTransient<IValidator, HTMLValidator>();
        services.AddTransient<IValidator, PdfValidatorV2>();
        services.AddTransient<IValidationFactory, ValidatorFactory>(x => new ValidatorFactory(x.GetServices(typeof(IValidator)).Cast<IValidator>()));

        services.AddTransient<ISignersDocumentLoader, SignersDocumentLoader>();
        services.AddTransient<ISigningPayloadService, SigningPayloadService>();
        services.AddTransient<ITransformationPropertiesService, TransformationPropertiesService>();
        services.AddTransient<IDocumentSigningService, DocumentSigningService>();
        services.AddTransient<ISigningValidationService, SigningValidationService>();

        services.AddTransient<ISystemTester, SystemTester>();

        NemloginConfig nemloginConfiguration = configurationSection.Get<NemloginConfig>();

        X509Certificate2 ocesCertificate = new(nemloginConfiguration.SignatureKeysConfiguration.KeystorePath,
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
        Assembly.Load("Nemlogin.QualifiedSigning.SDK.Xades"); // Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
        Assembly.Load("Nemlogin.QualifiedSigning.SDK.Pades"); // Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    }
}
