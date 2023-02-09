using HatTrick.API.DatabaseInitialisation;
using HatTrick.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Debugging;
using System;
using System.Threading.Tasks;

namespace HatTrick.API
{
    public static class Program
    {
        public const int ExitSuccess = 0;
        public const int ExitFailure = -1;

        private static IHostBuilder CreateHostBuilder(
            string[] args
        ) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureLogging(logging => logging.AddSerilog(dispose: true))
                .UseSerilog(
                    (hostingContext, services, loggerConfiguration) =>
                        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                )
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        private static async Task EnsureDatabaseExistsAsync(
            IHost host
        )
        {
            using var scope = host.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<Context>();

            await Initialiser.InitialiseAsync(
                dbContext,
                scope.ServiceProvider
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger(typeof(Initialiser))
            );
        }

        public static async Task<int> Main(
            string[] args
        )
        {
            SelfLog.Enable(Console.Error);

            var exitValue = ExitSuccess;

            try
            {
                Log.Debug("Initialising the application...");

                var host = CreateHostBuilder(args).Build();

                Log.Debug("Ensuring the database is initialised...");

                await EnsureDatabaseExistsAsync(host);

                Log.Debug("Starting the application...");

                await host.RunAsync();
            }
            catch (Exception exception)
            {
                Log.Fatal(
                    exception,
                    "An error has occured while running the application."
                );

                exitValue = ExitFailure;
            }
            finally
            {
                Log.Information(
                    "The application is closing. Exit value: {exitValue}",
                        exitValue
                );

                SelfLog.Disable();

                Log.CloseAndFlush();
            }

            return exitValue;
        }
    }
}