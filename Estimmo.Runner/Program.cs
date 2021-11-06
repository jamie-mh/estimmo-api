using Estimmo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Estimmo.Runner
{
    public static class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            RegisterServices(services, configuration);
            var provider = services.BuildServiceProvider();

            if (args.Length == 0)
            {
                Log.Error("No module specified");
                PrintAvailableModules();
                return 2;
            }

            var name = args[0];
            var moduleArgs = args.Skip(1).ToArray();

            var moduleType = GetModuleTypes().FirstOrDefault(m => m.Name == name);

            if (moduleType == null)
            {
                Log.Error("Invalid module '{Name}'", name);
                PrintAvailableModules();
                return 1;
            }

            try
            {
                var module = (IModule) ActivatorUtilities.CreateInstance(provider, moduleType);
                await module.RunAsync(moduleArgs);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred when executing the module");
                return 1;
            }

            return 0;
        }

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EstimmoContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Main");
                options.UseNpgsql(connectionString);
            });
        }

        private static IEnumerable<Type> GetModuleTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(IModule).IsAssignableFrom(t));
        }

        private static void PrintAvailableModules()
        {
            Log.Information("Available modules:");

            foreach (var mod in GetModuleTypes())
            {
                Log.Information("> {Name}", mod.Name);
            }
        }
    }
}
