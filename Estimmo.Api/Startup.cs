using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Api.Models;
using Estimmo.Api.TypeConverters.FeatureCollection;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Shared.Entities;
using Estimmo.Shared.Services;
using Estimmo.Shared.Services.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.Collections.Generic;
using Coordinates = Estimmo.Api.Entities.Coordinates;

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
            app.UseSwagger();
            app.UseSwaggerUI();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            config.CreateMap<IValueStats, KeyValuePair<short, double>>()
                .ConstructUsing(v => new KeyValuePair<short, double>((short) v.Type, v.Average));

            config.CreateMap<Place, AddressLike>()
                .ForMember(a => a.Coordinates,
                    p => p.MapFrom(c =>
                        new Coordinates { Latitude = c.Geometry.Coordinate.Y, Longitude = c.Geometry.Coordinate.X }));

            config.CreateMap<Place, SimplePlace>();

            config.CreateMap<Place, DetailedPlace>();
        }
    }
}
