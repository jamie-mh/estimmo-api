using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Api.Models;
using Estimmo.Api.Services;
using Estimmo.Api.Services.Impl;
using Estimmo.Api.TypeConverters.FeatureCollection;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.Collections.Generic;
using System.Linq;

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
                    builder.WithMethods("GET", "POST");
                });
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(
                    new GeoJsonConverterFactory(GeometryFactory.Default, true, "featureId"));
            });

            services.AddSwaggerGen();
            services.AddAutoMapper(ConfigureMappers);

            // Services
            services.AddScoped<IEstimationService, EstimationService>();

            // Type Converters
            services.AddSingleton<RegionsTypeConverter>();
            services.AddSingleton<DepartmentsTypeConverter>();
            services.AddSingleton<TownsTypeConverter>();
            services.AddSingleton<SectionsTypeConverter>();
            services.AddSingleton<PropertySalesTypeConverter>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EstimmoContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                context.Database.Migrate();

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

        private static void ConfigureMappers(IMapperConfigurationExpression config)
        {
            config.CreateMap<EstimateModel, EstimateRequest>()
                .ForMember(d => d.Coordinates,
                    o => o.MapFrom(v => new Point(v.PropertyCoordinates.Longitude, v.PropertyCoordinates.Latitude)));

            config.CreateMap<IEnumerable<Region>, FeatureCollection>()
                .ConvertUsing<RegionsTypeConverter>();

            config.CreateMap<IEnumerable<Department>, FeatureCollection>()
                .ConvertUsing<DepartmentsTypeConverter>();

            config.CreateMap<IEnumerable<Town>, FeatureCollection>()
                .ConvertUsing<TownsTypeConverter>();

            config.CreateMap<IEnumerable<Section>, FeatureCollection>()
                .ConvertUsing<SectionsTypeConverter>();

            config.CreateMap<IEnumerable<PropertySale>, FeatureCollection>()
                .ConvertUsing<PropertySalesTypeConverter>();

            config.CreateMap<IAverageValue, KeyValuePair<short, double>>()
                .ConstructUsing(v => new KeyValuePair<short, double>((short) v.Type, v.Value));

            config.CreateMap<Place, SimplePlace>();

            config.CreateMap<Place, DetailedPlace>();

            config.CreateMap<MessageModel, Message>();
        }
    }
}
