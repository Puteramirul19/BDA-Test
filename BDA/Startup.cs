using System;
using System.Collections.Generic;
using System.Globalization;
using Autofac;
using BDA.ActiveDirectory;
using BDA.Data;
using BDA.FileStorage;
using BDA.Identity;
using BDA.Services;
using BDA.SI_VendorInvoiceCreate_Async_Out_Service;
using BDA.Web;
using CoreWCF.Configuration;
using DocumentFormat.OpenXml.InkML;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;

namespace BDA
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            //
            // Antiforgery policy
            //
            services.AddAntiforgery(options => { options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; });

            //
            // DatabaseContext
            // //
            // services.AddDbContext<BdaDBContext>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<BdaDBContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Autofac Configuration
            var builder = new ContainerBuilder();
            var container = builder.Build();
            services
                .AddMvc(options => { options.InputFormatters.Insert(0, new RawJsonBodyInputFormatter()); });

            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.Cookie.HttpOnly = true;
            //    options.ExpireTimeSpan = TimeSpan.FromSeconds(10);

            //    options.LoginPath = "/Account/Login";
            //    options.AccessDeniedPath = "/Home/Error505";
            //    options.SlidingExpiration = true;
            //});

            //Identity
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Stores.MaxLengthForKeys = 128;
                })
                .AddEntityFrameworkStores<BdaDBContext>()
                //.AddDefaultUI()
                .AddDefaultTokenProviders();

            //Permission & Authentication services

            services.AddSingleton<PermissionChecker>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddTransient<IAuthenticationStrategy, InternalDatabaseAuthenticationStrategy>();
            services.AddTransient<IAuthenticationStrategy, ActiveDirectoryAuthenticationStrategy>();
            services.AddTransient<IAuthenticator, Authenticator>(); //NOTE: Authenticator will receive collection of 
            //                                                         //      IAuthenticationStrategy in its constructor
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                });

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.ExpireTimeSpan = TimeSpan.FromMinutes(1);
            //});

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(40);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //
            // Hangfire
            //
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            //
            // Email services
            //
            var emailSettingsSection = Configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettingsSection);

            var emailSettings = new EmailSettings();
            emailSettingsSection.Bind(emailSettings);
            
            if (emailSettings.UseMock)
            {
                services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, MockEmailSender>();
                services.AddTransient<IEmailSender, MockEmailSender>();
            }
            else
            {
                services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();
                services.AddTransient<IEmailSender, EmailSender>();
            }

            //
            // File Store
            //
            switch (Configuration["FileStore:Mode"])
            {
                case "Disk":
                    services.AddSingleton<IFileStore, DiskFileStore>(x =>
                    {
                        var hostingEnv = x.GetService<IHostingEnvironment>();
                        return new DiskFileStore(System.IO.Path.Combine(hostingEnv.ContentRootPath,
                            Configuration["FileStore:DiskDirectory"]));
                    });
                    break;

                case "Db":
                    services.AddSingleton<IFileStore, DbFileStore>(x =>
                    {
                        return new DbFileStore(Configuration["FileStore:DbConnectionString"]);
                    });
                    break;
            }

            //
            // Active Directory
            //
            if (Configuration.GetValue<bool>("ActiveDirectory:UseMock") == true)
            {
                services.AddSingleton<IDirectoryService, MockDirectoryService>();
            }
            else
            {
                services.AddSingleton<IDirectoryService, ActiveDirectoryService>(x =>
                    new ActiveDirectoryService(
                        Configuration["ActiveDirectory:Path"],
                        Configuration["ActiveDirectory:UserDomain"],
                        Configuration["ActiveDirectory:AdminUsername"],
                        Configuration["ActiveDirectory:AdminPassword"]
                    )
                );
            }

            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
                opt =>
                {
                    //configure your other properties
                    opt.LoginPath = "/Account/Login";
                    opt.LogoutPath = "/account/logout";
                    opt.AccessDeniedPath = "/account/accessdenied";
                });

            //
            // Notifications
            //
            services.Configure<NotificationSettings>(Configuration.GetSection("NotificationSettings"));
            services.AddScoped<NotificationService>();


            // LogServices
            //
            services.AddScoped<ILogService, LogService>();

            //services.ConfigureApplicationCookie(options =>
            //{
            //    options.LoginPath = "/account/login";
            //    options.LogoutPath = "/account/logout";
            //    options.AccessDeniedPath = "/account/accessdenied";
            //});

            //add custom services
            //services.AddScoped<BankDraftApplicationService>();
            //services.AddScoped<IRunningNumber>();

            //
            // Inject Configuration
            //
            services.AddSingleton<IConfiguration>(Configuration);

            //
            // Register coreWCF
            //
            services.AddServiceModelServices();
            services.AddServiceModelMetadata();
            services.AddScoped<SI_VendorInvoiceCreate_Async_OutClient>();

            services.AddControllersWithViews().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IncludeFields = true;
                //options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
            
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHsts();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Lax
            });

            // security purposes
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-AspNet-Version");
                
                await next();
            });
            // add custom response header
            app.Use(async (context, next) =>
            {
                var csp = "default-src 'self'; " + 
                          "img-src 'self' http: https:; " +
                          "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://code.jquery.com https://cdn.datatables.net https://cdnjs.cloudflare.com  https://ajax.aspnetcdn.com; " +
                          "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                          "font-src 'self' https://fonts.gstatic.com; " +
                          "connect-src 'self' https://cdn.datatables.net;";
                
                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                context.Response.Headers["Content-Security-Policy"] = csp;
                context.Response.Headers["X-Frame-Options"] = "DENY";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff"; 
                context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains; preload";
                context.Response.Headers["Referrer-Policy"] = "no-referrer";
                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                
                var origin = context.Request.Headers["Origin"].ToString();
                if (!string.IsNullOrEmpty(origin))
                {
                    context.Response.Headers["Access-Control-Allow-Origin"] = origin;
                    context.Response.Headers["Vary"] = "Origin";
                }
                
                await next();
            });
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            //_appLay
            // Hangfire
            //
            app.UseHangfireServer();
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization = new[] { new MyHangfireAuthorizationFilter() }
            });

            //RecurringJob.AddOrUpdate<ReportingController>(x => x.GenerateReportforState(), Cron.Daily(7), TimeZoneInfo.Local);
            //RecurringJob.AddOrUpdate<Services.NotificationService>(x => x.RemindRequesterForBDAcceptance(null), Cron.Daily(int.Parse(Configuration["Scheduler:WeeklyReminderBDAcceptance"])), TimeZoneInfo.Local);

            ////RecurringJob.AddOrUpdate<ReminderJob>(x => x.RemindRequesterForBDAcceptance(), Cron.Daily(8), TimeZoneInfo.Local);
            ////RecurringJob.AddOrUpdate<ReminderJob>(x => x.RemindRequesterForFirstPartialRecoverySubmission(), Cron.Daily(8), TimeZoneInfo.Local);
            ////RecurringJob.AddOrUpdate<ReminderJob>(x => x.RemindRequesterForSecondPartialRecoverySubmission(), Cron.Daily(8), TimeZoneInfo.Local);
            ////RecurringJob.AddOrUpdate<ReminderJob>(x => x.RemindRequesterForRecoveryProcess(), Cron.Daily(8), TimeZoneInfo.Local);
            ////RecurringJob.AddOrUpdate<ReminderJob>(x => x.RemindRequesterForFullRecoverySubmission(), Cron.Daily(8), TimeZoneInfo.Local);

            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");

                endpoints.MapControllerRoute(
                    name: "DefaultApi",
                    pattern: "api/{controller}/{id?}");
            });
            ;

            RotativaConfiguration.Setup(env);
        }
    }
}