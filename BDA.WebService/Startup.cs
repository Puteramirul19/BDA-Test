using System;
using System.Collections.Generic;
using System.Globalization;
//using System.ServiceModel;
using BDA.Data;
using BDA.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SoapCore;

namespace BDA.WebService
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

            services.Configure<RequestLocalizationOptions>(options =>
            {
                //    options.DefaultRequestCulture = new RequestCulture("ms-MY");

                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ms-MY");
                //By default the below will be set to whatever the server culture is. 
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("ms-MY") };
            });

            services.AddSoapCore();
            services.AddScoped<SI_TransactionStatus_Sync_In, TransactionStatus>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<BdaDBContext>(Configuration);
            services.AddDbContext<BdaDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                          sqlServerOptionsAction: sqlOptions =>
                          {
                              sqlOptions.EnableRetryOnFailure();
                          });
            });

            // Hangfire
            //
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            // Notifications
            //
            var emailSettingsSection = Configuration.GetSection("EmailSettings");
            var useMock = Convert.ToBoolean(emailSettingsSection["UseMock"]);
            if (!useMock)
            {
                services.Configure<NotificationSettings>(Configuration.GetSection("NotificationSettings"));
                services.AddScoped<NotificationService>();
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
          

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var settings = Configuration.GetSection("FileWSDL").Get<WsdlFileOptions>();
            settings.AppPath = env.ContentRootPath; // The hosting environment root path
            app.UseRouting();
            app.UseSoapEndpoint<SI_TransactionStatus_Sync_In>("/TransactionStatus.svc", new SoapEncoderOptions(), SoapSerializer.XmlSerializer, false, null, settings);
    
            // Does not comply .Net8 Standard
            // app.UseMvc();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // Use this for controllers
            });
        }
    }
}
