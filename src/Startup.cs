using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NemLoginSigningService.Services;
using NemLoginSignatureValidationService.Service;
using NemLoginSigningWebApp.Logic;
using NemLoginSigningCore.Logging;
using NemLoginSigningCore.Configuration;

namespace NemLoginSigningWebApp
{
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
            services.AddControllersWithViews();

            // Configuration dependencies
            var configurationSection = Configuration.GetSection("NemloginConfiguration");
            services.Configure<NemloginConfiguration>(configurationSection);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddTransient<ISignersDocumentLoader, SignersDocumentLoader>();
            services.AddTransient<ISigningPayloadService, SigningPayloadService>();
            services.AddTransient<ITransformationPropertiesService, TransformationPropertiesService>();
            services.AddTransient<IDocumentSigningService, DocumentSigningService>();
            services.AddTransient<ISigningResultService, SigningResultService>();
            services.AddTransient<ISigningValidationService, SigningValidationService>();

            var nemloginConfiguration = configurationSection.Get<NemloginConfiguration>();

            services.AddHttpClient("ValidationServiceClient", c => c.BaseAddress = new System.Uri(nemloginConfiguration.ValidationServiceURL));
            services.AddTransient<ISigningValidationService, SigningValidationService>();

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider()
                };

                options.SupportedCultures.Add(new CultureInfo("dk-DK"));
                options.SupportedCultures.Add(new CultureInfo("en-US"));
                options.SetDefaultCulture("da-DK");
                options.DefaultRequestCulture = new RequestCulture("da-DK");
            });

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            LoadAssemblies();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRequestLocalization();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            LoggerCreator.LoggerFactory = loggerFactory;

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void LoadAssemblies()
        {
            // Load assembly to be able to do reflection later on
            Assembly.Load("NemLoginSigningXades"); // , Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
            Assembly.Load("NemLoginSigningPades"); // , Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
        }
    }
}