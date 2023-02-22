using HatTrick.API.Features;
using HatTrick.API.Middlewares;
using HatTrick.BLL;
using HatTrick.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.AspNetCore;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;

namespace HatTrick.API
{
    internal sealed class Startup
    {
        public CultureInfo Culture { get; }
        public IConfiguration Configuration { get; }

        public Startup(
            IConfiguration configuration,
            CultureInfo? culture = null
        )
        {
            Configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
            Culture = culture ?? CultureInfo.CurrentCulture;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(
            IServiceCollection services
        )
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // logging
            services.Configure<RequestLoggingOptions>(
                Configuration.GetSection("Serilog:RequestLoggingOptions")
            );
            services.AddLogging(logging => logging.AddSerilog(dispose: true));

            // cache
            services.Configure<MemoryCacheOptions>(
                Configuration.GetSection("Cache:Options")
            );
            services.AddMemoryCache();

            // culture
            CultureInfo.CurrentCulture = Culture;
            CultureInfo.CurrentUICulture = Culture;
            services.AddSingleton(Culture);
            services.AddSingleton<IFormatProvider, CultureInfo>(_ => Culture);
            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    options.AddSupportedCultures(Culture.Name);
                    options.AddSupportedUICultures(Culture.Name);
                    options.SetDefaultCulture(Culture.Name);
                    options.DefaultRequestCulture = new RequestCulture(Culture);
                    //options.ApplyCurrentCultureToResponseHeaders = true;
                }
            );

            // HTTP client
            services.AddSingleton<CookieContainer>();
            services.AddHttpClient();

            // DAL
            services.AddDbContext<Context>(
                o => o.UseSqlite(Configuration.GetConnectionString("HatTrick"))
            );

            // BLL
            services.AddScoped<Account>();
            services.AddScoped<Offer>();
            services.AddScoped<BettingShop>();
            
            // security
            services.Configure<IISOptions>(
                options =>
                {
                    options.AuthenticationDisplayName = IISDefaults.AuthenticationScheme;
                    options.AutomaticAuthentication = true;
                }
            );
            services.AddAuthentication();
            services.AddAuthentication(IISDefaults.AuthenticationScheme);

            // controllers & Swagger
#if NET7_0_OR_GREATER
            services.AddDatabaseDeveloperPageExceptionFilter();
#endif // NET7_0_OR_GREATER
            services.AddControllers();
            services.AddSwaggerGen(
                swagger =>
                {
                    swagger.SwaggerDoc(
                        $"v{(Assembly.GetEntryAssembly()?.GetName().Version?.Major ?? 1):D}",
                        new OpenApiInfo()
                        {
                            Title = "Hat-Trick Web API",
                            Description = "Web API for the Hat-Trick Online Betting Shop Simulator",
                            Contact = new OpenApiContact()
                            {
                                Name = "Davor Penzar",
                                Email = "davor.penzar@gmail.com"
                            },
                            Version = $"v. {Assembly.GetEntryAssembly()?.GetName().Version}"
                        }
                    );

                    //swagger.ResolveConflictingActions(Enumerable.FirstOrDefault);

                    swagger.IncludeXmlComments(
                        Path.Combine(
                            AppContext.BaseDirectory,
                            $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"
                        )
                    );
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory
        )
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (env is null)
            {
                throw new ArgumentNullException(nameof(env));
            }
            if (loggerFactory is null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSerilogRequestLogging();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();

            app.UseRequestTime<HttpRequestTimeFeature>();

            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint(
                        $"v{(Assembly.GetEntryAssembly()?.GetName().Version?.Major ?? 1):D}/swagger.json",
                        $"Hat-Trick WAPI v. {Assembly.GetEntryAssembly()?.GetName().Version}"
                    );
                }
            );

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
