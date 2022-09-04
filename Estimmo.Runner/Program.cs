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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .CreateLogger();

            var services = new ServiceCollection();

            RegisterServices(services, configuration);
            var provider = services.BuildServiceProvider();
            List<ModuleArg> moduleArgs;

            try
            {
                moduleArgs = ParseCommandLine(args);
            }
            catch (ArgumentException e)
            {
                Log.Error("{Message}", e.Message);
                PrintAvailableModules();
                return 2;
            }

            var tasks = new List<Task>();

            foreach (var arg in moduleArgs)
            {
                var module = (IModule) ActivatorUtilities.CreateInstance(provider, arg.Type);
                tasks.Add(module.RunAsync(arg.Args));
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occurred when executing a module");
                return 1;
            }

            return 0;
        }

        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(builder =>
            {
                builder.AddSerilog();
            });

            services.AddDbContext<EstimmoContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Main");
                options.UseNpgsql(connectionString);
            });
        }

        private static List<ModuleArg> ParseCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("No arguments provided");
            }

            var moduleTypes = GetModuleTypes().ToList();
            var result = new List<ModuleArg>();
            ModuleArg moduleArg = null;

            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var type = moduleTypes.FirstOrDefault(t => t.Name == arg);

                if (type != null)
                {
                    if (i > 0)
                    {
                        result.Add(moduleArg);
                    }

                    moduleArg = new ModuleArg { Type = type };
                }
                else
                {
                    if (i == 0)
                    {
                        throw new ArgumentException("No module specified");
                    }

                    moduleArg.Args.Add(arg);
                }

                if (i == args.Length - 1)
                {
                    result.Add(moduleArg);
                }
            }

            return result;
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
