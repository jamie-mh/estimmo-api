// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Estimmo.Runner.Fixtures;
using Estimmo.Shared.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Context;
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
    public static partial class Program
    {
        [GeneratedRegex("^(.*?)=(.*?)$")]
        private static partial Regex ArgumentRegex();

        private const string ArgumentSkip = ";";
        
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

            List<ModuleInvocation> invocations;

            try
            {
                invocations = ParseCommandLine(args);
            }
            catch (ArgumentException e)
            {
                Log.Error("{Message}", e.Message);
                PrintAvailableModules();
                return 2;
            }
            
            try
            {
                await Parallel.ForEachAsync(invocations, async (invocation, _) =>
                {
                    var module = (IModule) ActivatorUtilities.CreateInstance(provider, invocation.Type);
                    
                    using (LogContext.PushProperty("Invocation", invocation.Id))
                    {
                        await module.RunAsync(invocation.Arguments);
                    }
                });
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
            }, ServiceLifetime.Transient);

            services.AddSingleton<AddressNormaliser>();
            
            // Fixtures
            services.AddSingleton<TownIdsFixture>();
            services.AddSingleton<SectionIdsFixture>();
        }

        private static List<ModuleInvocation> ParseCommandLine(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("No arguments provided");
            }
            
            var availableTypes = GetModuleTypes().ToList();
            var invocations = new List<ModuleInvocation>();
            
            var id = 1;
            var invocation = new ModuleInvocation { Arguments = new Dictionary<string, string>(), Id = id };

            foreach (var arg in args)
            {
                if (arg == ArgumentSkip)
                {
                    invocations.Add(invocation);
                    invocation = new ModuleInvocation
                    {
                        Id = ++id,
                        Arguments = new Dictionary<string, string>()
                    };
                    
                    continue;
                }

                if (invocation.Type == null)
                {
                    invocation.Type = availableTypes.FirstOrDefault(t => t.Name == arg);

                    if (invocation.Type == null)
                    {
                        throw new ArgumentException($"Module '{arg}' not found");
                    }

                    continue;
                }

                var match = ArgumentRegex().Match(arg);

                if (match.Success)
                {
                    invocation.Arguments.Add(match.Groups[1].Value, match.Groups[2].Value);
                }
                else
                {
                    invocation.Arguments.Add(arg, null);
                }
            }

            if (invocation.Type != null)
            {
                invocations.Add(invocation);
            }
            
            return invocations;
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
