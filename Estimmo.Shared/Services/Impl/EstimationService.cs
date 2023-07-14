// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Estimmo.Shared.Entities;
using Estimmo.Shared.Extension;
using MathNet.Numerics.LinearRegression;
using Microsoft.EntityFrameworkCore;

namespace Estimmo.Shared.Services.Impl
{
    public class EstimationService : IEstimationService
    {
        private const double SearchRadius = 1_000d;

        private readonly IEstimmoContext _context;

        public EstimationService(IEstimmoContext context)
        {
            _context = context;
        }

        public async Task<Estimate> GetEstimateAsync(EstimateRequest request, DateTime date)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Coordinates == null)
            {
                throw new ArgumentException("Coordinates cannot be null");
            }
            
            var nearbyPropertySales = await _context.PropertySales
                .Where(p => p.Type == request.PropertyType &&
                            p.Coordinates.IsWithinDistance(request.Coordinates, SearchRadius))
                .OrderBy(p => p.Date)
                .ToListAsync();

            if (!nearbyPropertySales.Any())
            {
                return null;
            }

            var maxDaysSince = (date - nearbyPropertySales.First().Date).TotalDays;
            var parameters = new double[nearbyPropertySales.Count][];
            var values = new double[nearbyPropertySales.Count];
            var weights = new double[nearbyPropertySales.Count];

            for (var i = 0; i < nearbyPropertySales.Count; ++i)
            {
                var sale = nearbyPropertySales[i];

                parameters[i] = new double[3];
                parameters[i][0] = sale.RoomCount;
                parameters[i][1] = sale.LandSurfaceArea;
                parameters[i][2] = sale.BuildingSurfaceArea;

                values[i] = (double) sale.Value;

                // Calculate the weight as the distance from the request point and the days since the latest record
                var daysSince = (date - sale.Date).TotalDays;
                var normalisedDistance = sale.Coordinates.GreatCircleDistance(request.Coordinates) / SearchRadius;
                var normalisedDaysSince = 1 - daysSince / maxDaysSince;

                weights[i] = (normalisedDistance + normalisedDaysSince) / 2d;
            }

            var model = WeightedRegression.Weighted(parameters, values, weights);
            var estimatedValue =
                (model[0] * request.Rooms) + (model[1] * request.LandSurfaceArea) +
                (model[2] * request.BuildingSurfaceArea);

            return new Estimate
            {
                Value = estimatedValue,
                DataPointCount = nearbyPropertySales.Count
            };
        }
    }
}
