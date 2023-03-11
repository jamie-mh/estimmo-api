using Estimmo.Data;
using Estimmo.Shared.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers())
                .CreateLogger();

            var services = new ServiceCollection();

            RegisterServices(services, configuration);
            var provider = services.BuildServiceProvider();

            ValueTuple<Type, Dictionary<string, string>> command;

            try
            {
                command = ParseCommandLine(args);
            }
            catch (ArgumentException e)
            {
                Log.Error("{Message}", e.Message);
                PrintAvailableModules();
                return 2;
            }

            var module = (IModule) ActivatorUtilities.CreateInstance(provider, command.Item1);

            try
            {
                await module.RunAsync(command.Item2);
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

            services.AddSingleton<AddressNormaliser>();
        }

        private static ValueTuple<Type, Dictionary<string, string>> ParseCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("No arguments provided");
            }

            var availableTypes = GetModuleTypes().ToList();
            var type = availableTypes.FirstOrDefault(t => t.Name == args[0]);

            if (type == null)
            {
                throw new ArgumentException("Module not found");
            }

            var parsedArgs = new Dictionary<string, string>();

            foreach (var arg in args.Skip(1))
            {
                var match = Regex.Match(arg, @"^(.*?)=(.*?)$");

                if (match.Success)
                {
                    parsedArgs.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
                else
                {
                    parsedArgs.Add(arg, null);
                }
            }

            return new ValueTuple<Type, Dictionary<string, string>>(type, parsedArgs);
        }

        private static IEnumerable<Type> GetModuleTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(IModule).IsAssignableFrom(t));
        }

        private static void PrintAvailableModules()
        {
            Console.WriteLine("Available modules:");

            foreach (var mod in GetModuleTypes())
            {
                Console.WriteLine($"- {mod.Name}");
            }
        }
    }
}
