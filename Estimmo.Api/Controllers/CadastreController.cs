// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.Entities;
using Estimmo.Api.Mappers;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class CadastreController : ControllerBase
    {
        private readonly EstimmoContext _context;

        public CadastreController(EstimmoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("/regions")]
        [SwaggerOperation(
            Summary = "Get all regions",
            OperationId = "GetRegions",
            Tags = ["Cadastre"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Features with values payload", typeof(FeaturesWithValues))]
        public async Task<FeaturesWithValues> GetRegions(short? salesYear = null)
        {
            List<Region> regions;

            if (salesYear == null)
            {
                regions = await _context.Regions
                    .Include(r => r.ValueStats)
                    .ToListAsync();
            }
            else
            {
                regions = await _context.Regions
                    .Include(r => r.ValueStatsByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = FeatureCollectionMapper.MapToFeatureCollection(regions);

            IEnumerable<IValueStats> valueStats;

            if (salesYear == null)
            {
                valueStats = await _context.FranceValueStats.ToListAsync();
            }
            else
            {
                valueStats = await _context.FranceValueStatsByYear
                    .Where(v => v.Year == salesYear)
                    .ToListAsync();
            }

            var valueStatsByYear = await _context.FranceValueStatsByYear.ToListAsync();

            return new FeaturesWithValues
            {
                ValueStats = valueStats,
                ValueStatsByYear = valueStatsByYear,
                GeoJson = featureCollection
            };
        }

        [HttpGet]
        [Route("/regions/{regionId}/departments")]
        [SwaggerOperation(
            Summary = "Get all departments belonging to region",
            OperationId = "GetDepartments",
            Tags = ["Cadastre"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Features with values payload", typeof(FeaturesWithValues))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Region not found")]
        public async Task<FeaturesWithValues> GetDepartments(string regionId, short? salesYear = null)
        {
            var queryable = _context.Departments.Where(d => d.RegionId == regionId);
            List<Department> departments;

            if (salesYear == null)
            {
                departments = await queryable
                    .Include(d => d.ValueStats)
                    .ToListAsync();
            }
            else
            {
                departments = await queryable
                    .Include(d => d.ValueStatsByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = FeatureCollectionMapper.MapToFeatureCollection(departments);

            IEnumerable<IValueStats> valueStats;

            if (salesYear == null)
            {
                valueStats = await _context.RegionValueStats
                    .Where(v => v.Id == regionId)
                    .ToListAsync();
            }
            else
            {
                valueStats = await _context.RegionValueStatsByYear
                    .Where(v => v.Id == regionId && v.Year == salesYear)
                    .ToListAsync();
            }

            var valueStatsByYear = await _context.RegionValueStatsByYear
                .Where(r => r.Id == regionId)
                .ToListAsync();

            return new FeaturesWithValues
            {
                ValueStats = valueStats,
                ValueStatsByYear = valueStatsByYear,
                GeoJson = featureCollection
            };
        }

        [HttpGet]
        [Route("/departments/{departmentId}/towns")]
        [SwaggerOperation(
            Summary = "Get all towns belonging to department",
            OperationId = "GetTowns",
            Tags = ["Cadastre"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Features with values payload", typeof(FeaturesWithValues))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Department not found")]
        public async Task<FeaturesWithValues> GetTowns(string departmentId, short? salesYear = null)
        {
            var queryable = _context.Towns.Where(t => t.DepartmentId == departmentId);
            List<Town> towns;

            if (salesYear == null)
            {
                towns = await queryable
                    .Include(t => t.ValueStats)
                    .ToListAsync();
            }
            else
            {
                towns = await queryable
                    .Include(t => t.ValueStatsByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = FeatureCollectionMapper.MapToFeatureCollection(towns);

            IEnumerable<IValueStats> valueStats;

            if (salesYear == null)
            {
                valueStats = await _context.DepartmentValueStats
                    .Where(v => v.Id == departmentId)
                    .ToListAsync();
            }
            else
            {
                valueStats = await _context.DepartmentValueStatsByYear
                    .Where(v => v.Id == departmentId && v.Year == salesYear)
                    .ToListAsync();
            }

            var valueStatsByYear = await _context.DepartmentValueStatsByYear
                .Where(d => d.Id == departmentId)
                .ToListAsync();

            return new FeaturesWithValues
            {
                ValueStats = valueStats,
                ValueStatsByYear = valueStatsByYear,
                GeoJson = featureCollection
            };
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
        [SwaggerOperation(
            Summary = "Get all sections belonging to town",
            OperationId = "GetSections",
            Tags = ["Cadastre"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Features with values payload", typeof(FeaturesWithValues))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Town not found")]
        public async Task<FeaturesWithValues> GetSections(string townId, short? salesYear = null)
        {
            var queryable = _context.Sections.Where(s => s.TownId == townId);
            List<Section> sections;

            if (salesYear == null)
            {
                sections = await queryable
                    .Include(s => s.ValueStats)
                    .ToListAsync();
            }
            else
            {
                sections = await queryable
                    .Include(s => s.ValueStatsByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = FeatureCollectionMapper.MapToFeatureCollection(sections);

            IEnumerable<IValueStats> valueStats;

            if (salesYear == null)
            {
                valueStats = await _context.TownValueStats
                    .Where(v => v.Id == townId)
                    .ToListAsync();
            }
            else
            {
                valueStats = await _context.TownValueStatsByYear
                    .Where(v => v.Id == townId && v.Year == salesYear)
                    .ToListAsync();
            }

            var valueStatsByYear = await _context.TownValueStatsByYear
                .Where(t => t.Id == townId)
                .ToListAsync();

            return new FeaturesWithValues
            {
                ValueStats = valueStats,
                ValueStatsByYear = valueStatsByYear,
                GeoJson = featureCollection
            };
        }
    }
}
