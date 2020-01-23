using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BiblioMit.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BiblioMit.Models;
using BiblioMit.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using BiblioMit.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using System.IO;
using AspNetCore.IServiceCollection.AddIUrlHelper;
//using PaulMiami.AspNetCore.Mvc.Recaptcha;
using Microsoft.AspNetCore.Routing;

namespace BiblioMit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public string os = Environment.OSVersion.Platform.ToString();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString(os + "Connection")));

            services.AddHostedService<SeedBackground>();
            services.AddScoped<ISeed, SeedService>();

            //services.AddRecaptcha(new RecaptchaOptions
            //{
            //    SiteKey = Configuration["6Ld3SGcUAAAAANRVkFCci9QZMf5fSbRzROLXEyZk"],
            //    SecretKey = Configuration["6Ld3SGcUAAAAAJdww1OzASUrSve3O8oZfLpfmGjy"]
            //});
            services.AddIdentity<AppUser, AppRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<SpanishIdentityErrorDescriber>();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = new PathString("/login");
                    o.AccessDeniedPath = new PathString("/Account/AccessDenied");
                    o.LogoutPath = new PathString("/logout");
                });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IForum, ForumService>();
            services.AddScoped<IPost, PostService>();
            services.AddScoped<IUpload, UploadService>();
            services.AddScoped<IAppUser, AppUserService>();

            //services.AddScoped<ILibs, LibService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("bibliomit.cl");
                options.ExcludedHosts.Add("www.bibliomit.cl");
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });

            services.AddNodeServices(o =>
            {
                o.ProjectPath = "./";
            });

            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("es"),
                        new CultureInfo("en")
                    };

                    opts.DefaultRequestCulture = new RequestCulture("es");
                    // Formatting numbers, dates, etc.
                    opts.SupportedCultures = supportedCultures;
                    // UI strings that we have localized.
                    opts.SupportedUICultures = supportedCultures;
                });

            services.AddAuthorization(options =>
            {
                foreach (var item in ClaimData.UserClaims)
                {
                    options.AddPolicy(item, policy => policy.RequireClaim(item, item));
                }
            });

            services.AddUrlHelper();

            // Authorization handlers.
            services.AddScoped<IAuthorizationHandler,
                                  ContactIsOwnerAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                                  ContactAdministratorsAuthorizationHandler>();

            services.AddScoped<IAuthorizationHandler,
                                  ContactManagerAuthorizationHandler>();

            services.AddScoped<IViewRenderService, ViewRenderService>();

            //services.AddCors();

            services.AddSignalR(options => {
                options.EnableDetailedErrors = true;
            });

            //services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            //services.AddWebOptimizer(pipeline =>
            //{
            //    pipeline.MinifyJsFiles();
            //    pipeline.MinifyCssFiles();
            //    pipeline.MinifyHtmlFiles();
            //});

            //services.AddWebMarkupMin(
            //    options =>
            //    {
            //        options.AllowMinificationInDevelopmentEnvironment = false;
            //        options.AllowCompressionInDevelopmentEnvironment = false;
            //    })
            //    .AddHtmlMinification(
            //        options =>
            //        {
            //            options.MinificationSettings.RemoveRedundantAttributes = true;
            //            options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
            //            options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
            //        })
            //    .AddHttpCompression();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSitemapMiddleware();
            //app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseDefaultFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
                routes.MapHub<EntryHub>("/entryHub");
                routes.MapHub<ProgressHub>("/progressHub");
            });

            if (env.IsDevelopment())
            {
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                //{
                //    ProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "ClientApp"),
                //    HotModuleReplacement = true
                //});
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var path = new List<string> { "lib", "cldr-data", "main" };

            var ch = os == "Win32NT" ? @"\" : "/";

            var di = new DirectoryInfo(Path.Combine(env.WebRootPath, string.Join(ch, path)));
            var supportedCultures = di.GetDirectories().Where(x => x.Name != "root").Select(x => new CultureInfo(x.Name)).ToList();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.FirstOrDefault(x => x.Name == "es")),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();

            //app.UseWebOptimizer();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            
            //app.UseWebMarkupMin();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
