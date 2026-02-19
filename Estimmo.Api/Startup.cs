// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Estimmo.Shared.Services;
using Estimmo.Shared.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;

namespace Estimmo.Api
{
    public class Startup
    {
        private const string CorsPolicyName = "CorsPolicy";

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<EstimmoContext>(options =>
            {
                var connectionString = _configuration.GetConnectionString("Main");
                options.UseNpgsql(connectionString);
            });

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.WithMethods("GET", "POST", "PATCH");
                });
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new GeoJsonConverterFactory(GeometryFactory.Default, true, "featureId"));
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Estimmo API",
                    Description = "Backend for Estimmo application"
                });

                options.EnableAnnotations();
            });

            // Services
            services.AddScoped<IEstimationService, EstimationService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EstimmoContext context)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                });

                app.UseExceptionHandler("/error/500");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseRouting();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(CorsPolicyName);
            });
        }
    }
}
