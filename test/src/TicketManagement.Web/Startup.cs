using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Serilog;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.DataAccess.EFRepositories;
using TicketManagement.DataAccess.Interfaces;
using TicketManagement.Web.Clients;
using TicketManagement.Web.Infrastructure.Helpers;
using TicketManagement.Web.Infrastructure.MapperProfile;
using TicketManagement.Web.JwtTokenAuth;

namespace TicketManagement.Web
{
#pragma warning disable S1200 // Classes should not be coupled to too many other classes (Single Responsibility Principle)
    public class Startup
#pragma warning restore S1200 // Classes should not be coupled to too many other classes (Single Responsibility Principle)
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(JwtTokenConstants.SchemeName)
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(JwtTokenConstants.SchemeName, null);
            services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ru"),
                    new CultureInfo("be"),
                };

                options.DefaultRequestCulture = new RequestCulture(CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(context =>
                {
                    var lang = (string)context.Request.Query["lang"] ?? context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
                    var defaultLang = string.IsNullOrEmpty(lang) ? CultureInfo.CurrentCulture.TwoLetterISOLanguageName : lang;
                    return Task.FromResult(new ProviderCultureResult(defaultLang, defaultLang));
                }));
            });

            services.AddDbContext<TicketManagementContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:TicketManagementDb"]));

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(IRepository<>))
                .AddClasses(classes => classes.InNamespaces("TicketManagement.DataAccess.EFRepositories"))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(BaseService<>))
                .AddClasses(classes => classes.InNamespaces("TicketManagement.BusinessLogic.Services"))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddAutoMapper(typeof(ModelProfile));
            services.AddScoped<TimeZoneHelper>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            Action<IServiceProvider, HttpClient> userApiSettings = (provider, client) =>
            {
                var userApiAddress = Configuration["UserApiAddress"];
                client.BaseAddress = new Uri(userApiAddress ?? string.Empty);
            };
            services.AddHttpClient<IUserClient, UserClient>(userApiSettings);
            services.AddHttpClient<IRolesClient, RolesClient>(userApiSettings);
            services.AddHttpClient<ILoginClient, LoginClient>(userApiSettings);

            services.AddHttpClient<IEventManagerClient, EventManagerClient>((provider, client) =>
            {
                var userApiAddress = Configuration["EventManagerApiAddress"];
                client.BaseAddress = new Uri(userApiAddress ?? string.Empty);
            });

            services.AddFeatureManagement(Configuration.GetSection("FeatureFlags"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseRequestLocalization();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.Use(async (context, next) =>
            {
                var lang = (string)context.Request.Query["lang"];
                if (!string.IsNullOrEmpty(lang))
                {
                    context.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
                }

                var token = (string)context.Request.Query["token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Response.Cookies.Append(JwtTokenConstants.JwtCookieKey, token, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        MaxAge = TimeSpan.FromMinutes(60),
                    });
                }

                token ??= context.Request.Cookies[JwtTokenConstants.JwtCookieKey];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }

                await next();
            });
            app.UseAuthentication();

            app.UseStatusCodePages(context =>
            {
                var response = context.HttpContext.Response;
                if (response.StatusCode == (int)HttpStatusCode.Unauthorized ||
                    response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    response.Redirect("/Account/Login");
                }

                return Task.CompletedTask;
            });

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}");
            });
        }
    }
}
