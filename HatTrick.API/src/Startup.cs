using HatTrick.API.Formatters;
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
        public CultureInfo Culture { get; init; }
        public IConfiguration Configuration { get; init; }

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
                (options) =>
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
            services.AddScoped<BetShop>();
            /*
            services.AddSingleton<IParserFactory, SimpleParserFactory>(_ => new SimpleParserFactory(';'));
            services.AddScoped<IParser, SimpleParser>(
                sp => (SimpleParser)sp.GetRequiredService<IParserFactory>().Create()
            );
            services.AddScoped<ICommunicator, Communicator>();
            {
                services.AddHttpClient<ICommunicator, Communicator>(
                    (sp, client) =>
                    {
                        client.BaseAddress = new(Configuration["CustomBet:Authority"]);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.ParseAdd("application/xml");
                        client.DefaultRequestHeaders.Accept.ParseAdd("text/xml");
                        client.DefaultRequestHeaders.Accept.ParseAdd("text/plain");
                        client.DefaultRequestHeaders.Add(
                            "X-Access-Token",
                            sp.GetRequiredService<IEncryptor>().Decrypt(Configuration["CustomBet:AccessToken"])
                        );
                    }
                ).ConfigurePrimaryHttpMessageHandler(
                    sp => new HttpClientHandler()
                    {
                        Proxy = String.IsNullOrEmpty(Configuration["CustomBet:Proxy"]) ?
                            null :
                            new WebProxy(
                                sp.GetRequiredService<IEncryptor>().Decrypt(Configuration["CustomBet:Proxy"])
                            ),
                        DefaultProxyCredentials = null,
                        UseProxy = !String.IsNullOrEmpty(Configuration["CustomBet:Proxy"]),
                        CookieContainer = sp.GetRequiredService<CookieContainer>(),
                        UseCookies = true,
                        ClientCertificateOptions = ClientCertificateOption.Automatic,
                        PreAuthenticate = false,
                        UseDefaultCredentials = false,
                        Credentials = null
                    }
                );
            }
            services.AddSingleton<INamespacesPersisterFactory, NamespacesPersisterFactory>(
                sp => new(
                    sp.GetRequiredService<ILoggerFactory>(),
                    sp.GetRequiredService<ILogger<NamespacesPersisterFactory>>(),
                    sp.GetRequiredService<IEncryptor>().Decrypt(Configuration.GetConnectionString("CustomBet")),
                    LogDatabaseInformation,
                    true
                )
            );
            services.AddScoped<INamespacesPersister, NamespacesPersister>(
                sp => (NamespacesPersister)sp.GetRequiredService<INamespacesPersisterFactory>().Create()
            );
            services.AddSingleton<ISelectionsPersisterFactory, BLL.Db.SelectionsPersisterFactory>(
                sp => new(
                    sp.GetRequiredService<ILoggerFactory>(),
                    sp.GetRequiredService<ILogger<BLL.Db.SelectionsPersisterFactory>>(),
                    sp.GetRequiredService<IEncryptor>().Decrypt(Configuration.GetConnectionString("CustomBet")),
                    Configuration["Persistence:DefaultNamespace"],
                    LogDatabaseInformation,
                    true
                )
            );
            services.AddScoped<ISelectionsPersister, BLL.Db.SelectionsPersister>(
                sp => (BLL.Db.SelectionsPersister)sp.GetRequiredService<ISelectionsPersisterFactory>().Create()
            );
            services.AddSingleton<ICalculationResultsPersisterFactory, CalculationResultsPersisterFactory>(
                sp => new(
                    sp.GetRequiredService<ILoggerFactory>(),
                    sp.GetRequiredService<ILogger<CalculationResultsPersisterFactory>>(),
                    sp.GetRequiredService<IEncryptor>().Decrypt(Configuration.GetConnectionString("CustomBet")),
                    Configuration["Persistence:DefaultNamespace"],
                    LogDatabaseInformation,
                    true
                )
            );
            services.AddScoped<ICalculationResultsPersister, CalculationResultsPersister>(
                sp => (CalculationResultsPersister)sp.GetRequiredService<ICalculationResultsPersisterFactory>().Create()
            );
            */

            // managers
            /*
            services.AddScoped(
                sp => new SelectionsEvaluationManager(
                    sp,
                    sp.GetRequiredService<ILogger<SelectionsEvaluationManager>>(),
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(SelectionsEvaluationManager).FullName),
                    sp.GetRequiredService<ISelectionsPersisterFactory>(),
                    sp.GetRequiredService<ICalculationResultsPersisterFactory>(),
                    sp.GetRequiredService<INamespacesPersister>(),
                    sp.GetRequiredService<ISelectionsPersister>(),
                    Configuration.GetValue<Int32>("CustomBet:Requests:BatchSize", 5)
                )
            );
            {
                services.AddHttpClient<SelectionsEvaluationManager>(
                    (sp, client) =>
                    {
                        client.BaseAddress = new(Configuration["InternalApiEndpoints:Authority"]);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                        client.DefaultRequestHeaders.Accept.ParseAdd("text/json");
                        client.DefaultRequestHeaders.Accept.ParseAdd("text/plain");
                    }
                ).ConfigurePrimaryHttpMessageHandler(
                    sp => new HttpClientHandler()
                    {
                        Proxy = String.IsNullOrEmpty(Configuration["InternalApiEndpoints:Proxy"]) ?
                            null :
                            new WebProxy(
                                sp.GetRequiredService<IEncryptor>().Decrypt(Configuration["InternalApiEndpoints:Proxy"])
                            ),
                        DefaultProxyCredentials = null,
                        UseProxy = !String.IsNullOrEmpty(Configuration["InternalApiEndpoints:Proxy"]),
                        CookieContainer = sp.GetRequiredService<CookieContainer>(),
                        UseCookies = true,
                        ClientCertificateOptions = ClientCertificateOption.Automatic,
                        PreAuthenticate = true,
                        UseDefaultCredentials = true
                    }
                );
            }
            */

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
            //services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllers(
                options => options.InputFormatters.Add(new TextPlainInputFormatter())
            );
            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        $"v{Assembly.GetEntryAssembly()?.GetName().Version?.Major ?? 1:D}",
                        new()
                        {
                            Title = "",
                            Description = "",
                            Contact = new()
                            {
                                Name = "Davor Penzar",
                                Email = "davor.penzar@gmail.com"
                            },
                            Version = $"v. {Assembly.GetEntryAssembly()?.GetName().Version}"
                        }
                    );

                    //c.ResolveConflictingActions(Enumerable.FirstOrDefault);

                    c.IncludeXmlComments(
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

            app.UseRequestTime();

            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint(
                        $"v{Assembly.GetEntryAssembly()?.GetName().Version?.Major ?? 1:D}/swagger.json",
                        $"XXX v. {Assembly.GetEntryAssembly()?.GetName().Version}"
                    );
                }
            );

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
