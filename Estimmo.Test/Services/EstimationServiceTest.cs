// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Shared.Entities;
using Estimmo.Shared.Services;
using Estimmo.Shared.Services.Impl;
using Moq;
using Moq.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Estimmo.Test.Services
{
    public class EstimationServiceTest
    {
        private readonly Mock<IEstimmoContext> _context;
        private readonly IEstimationService _estimationService;

        public EstimationServiceTest()
        {
            _context = new Mock<IEstimmoContext>();
            _estimationService = new EstimationService(_context.Object);
        }

        [Fact]
        public async Task GetEstimateAsync_nullRequest()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _estimationService.GetEstimateAsync(null, DateTime.Now));
        }
        
        [Fact]
        public async Task GetEstimateAsync_nullCoordinates()
        {
            var request = new EstimateRequest();
            await Assert.ThrowsAsync<ArgumentException>(() => _estimationService.GetEstimateAsync(request, DateTime.Now));
        }
        
        [Fact]
        public async Task GetEstimateAsync_noSales()
        {
            var request = new EstimateRequest
            {
                PropertyType = PropertyType.House,
                Coordinates = new Point(1, 2),
                Rooms = 4,
                LandSurfaceArea = 1000,
                BuildingSurfaceArea = 100
            };

            _context.Setup(c => c.PropertySales).ReturnsDbSet(new List<PropertySale>());
            var result = await _estimationService.GetEstimateAsync(request, DateTime.Now);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetEstimateAsync_ok()
        {
            var now = DateTime.Now;
            
            var saleIgnored = new PropertySale { Type = PropertyType.Apartment };
            
            var saleA = new PropertySale
            {
                Type = PropertyType.House,
                Coordinates = new Point(48.85840612089741, 2.2944864222620662),
                RoomCount = 4,
                LandSurfaceArea = 1000,
                BuildingSurfaceArea = 100,
                Value = 100000,
                Date = now.AddDays(-10)
            };
            
            var saleB = new PropertySale
            {
                Type = PropertyType.House,
                Coordinates = new Point(48.86287849039163, 2.287350447146063),
                RoomCount = 2,
                LandSurfaceArea = 500,
                BuildingSurfaceArea = 50,
                Value = 50000,
                Date = now.AddDays(-100)
            };
            
            var saleC = new PropertySale
            {
                Type = PropertyType.House,
                Coordinates = new Point(48.864388080661165, 2.3014838450365773),
                RoomCount = 6,
                LandSurfaceArea = 5000,
                BuildingSurfaceArea = 150,
                Value = 200000,
                Date = now.AddDays(-200)
            };
            
            var saleD = new PropertySale
            {
                Type = PropertyType.House,
                Coordinates = new Point(48.85605652015742, 2.297810722782372),
                RoomCount = 8,
                LandSurfaceArea = 2000,
                BuildingSurfaceArea = 120,
                Value = 100000,
                Date = now.AddDays(-300)
            };

            var sales = new List<PropertySale> { saleIgnored, saleA, saleB, saleC, saleD };
            _context.Setup(c => c.PropertySales).ReturnsDbSet(sales);
            
            var request = new EstimateRequest
            {
                PropertyType = PropertyType.House,
                Coordinates = new Point(48.86158614603743, 2.297492896043603),
                Rooms = 4,
                LandSurfaceArea = 600,
                BuildingSurfaceArea = 100
            };
            
            var result = await _estimationService.GetEstimateAsync(request, now);
            Assert.Equal(4, result.DataPointCount);
            Assert.True(result.Value > 0);
        }
    }
}
