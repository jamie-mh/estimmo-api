using AutoMapper;
using Estimmo.Api.Entities;
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
        private readonly IMapper _mapper;

        public CadastreController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/regions")]
        [SwaggerOperation(
            Summary = "Get all regions",
            OperationId = "GetRegions",
            Tags = new[] { "Cadastre" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cadastre item payload", typeof(CadastreItem))]
        public async Task<CadastreItem> GetRegions(short? salesYear = null)
        {
            List<Region> regions;

            if (salesYear == null)
            {
                regions = await _context.Regions
                    .Include(r => r.AverageValues)
                    .ToListAsync();
            }
            else
            {
                regions = await _context.Regions
                    .Include(r => r.AverageValuesByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = _mapper.Map<FeatureCollection>(regions);

            IEnumerable<IAverageValue> averageValues;

            if (salesYear == null)
            {
                averageValues = await _context.FranceAverageValues.ToListAsync();
            }
            else
            {
                averageValues = await _context.FranceAverageValuesByYear
                    .Where(v => v.Year == salesYear)
                    .ToListAsync();
            }

            var averageValuesByYear = await _context.FranceAverageValuesByYear.ToListAsync();

            return new CadastreItem
            {
                AverageValues = averageValues,
                AverageValuesByYear = averageValuesByYear,
                GeoJson = featureCollection
            };
        }

        [HttpGet]
        [Route("/regions/{regionId}/departments")]
        [SwaggerOperation(
            Summary = "Get all departments belonging to region",
            OperationId = "GetDepartments",
            Tags = new[] { "Cadastre" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cadastre item payload", typeof(CadastreItem))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Region not found")]
        public async Task<CadastreItem> GetDepartments(string regionId, short? salesYear = null)
        {
            var queryable = _context.Departments.Where(d => d.RegionId == regionId);
            List<Department> departments;

            if (salesYear == null)
            {
                departments = await queryable
                    .Include(d => d.AverageValues)
                    .ToListAsync();
            }
            else
            {
                departments = await queryable
                    .Include(d => d.AverageValuesByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = _mapper.Map<FeatureCollection>(departments);

            IEnumerable<IAverageValue> averageValues;

            if (salesYear == null)
            {
                averageValues = await _context.RegionAverageValues
                    .Where(v => v.Id == regionId)
                    .ToListAsync();
            }
            else
            {
                averageValues = await _context.RegionAverageValuesByYear
                    .Where(v => v.Id == regionId && v.Year == salesYear)
                    .ToListAsync();
            }

            var averageValuesByYear = await _context.RegionAverageValuesByYear
                .Where(r => r.Id == regionId)
                .ToListAsync();

            return new CadastreItem
            {
                AverageValues = averageValues,
                AverageValuesByYear = averageValuesByYear,
                GeoJson = featureCollection
            };
        }

        [HttpGet]
        [Route("/departments/{departmentId}/towns")]
        [SwaggerOperation(
            Summary = "Get all towns belonging to department",
            OperationId = "GetTowns",
            Tags = new[] { "Cadastre" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cadastre item payload", typeof(CadastreItem))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Department not found")]
        public async Task<CadastreItem> GetTowns(string departmentId, short? salesYear = null)
        {
            var queryable = _context.Towns.Where(t => t.DepartmentId == departmentId);
            List<Town> towns;

            if (salesYear == null)
            {
                towns = await queryable
                    .Include(t => t.AverageValues)
                    .ToListAsync();
            }
            else
            {
                towns = await queryable
                    .Include(t => t.AverageValuesByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = _mapper.Map<FeatureCollection>(towns);

            IEnumerable<IAverageValue> averageValues;

            if (salesYear == null)
            {
                averageValues = await _context.DepartmentAverageValues
                    .Where(v => v.Id == departmentId)
                    .ToListAsync();
            }
            else
            {
                averageValues = await _context.DepartmentAverageValuesByYear
                    .Where(v => v.Id == departmentId && v.Year == salesYear)
                    .ToListAsync();
            }

            var averageValuesByYear = await _context.DepartmentAverageValuesByYear
                .Where(d => d.Id == departmentId)
                .ToListAsync();

            return new CadastreItem
            {
                AverageValues = averageValues,
                AverageValuesByYear = averageValuesByYear,
                GeoJson = featureCollection
            };
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
        [SwaggerOperation(
            Summary = "Get all sections belonging to town",
            OperationId = "GetSections",
            Tags = new[] { "Cadastre" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Cadastre item payload", typeof(CadastreItem))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Town not found")]
        public async Task<CadastreItem> GetSections(string townId, short? salesYear = null)
        {
            var queryable = _context.Sections.Where(s => s.TownId == townId);
            List<Section> sections;

            if (salesYear == null)
            {
                sections = await queryable
                    .Include(s => s.AverageValues)
                    .ToListAsync();
            }
            else
            {
                sections = await queryable
                    .Include(s => s.AverageValuesByYear.Where(v => v.Year == salesYear))
                    .ToListAsync();
            }

            var featureCollection = _mapper.Map<FeatureCollection>(sections);

            IEnumerable<IAverageValue> averageValues;

            if (salesYear == null)
            {
                averageValues = await _context.TownAverageValues
                    .Where(v => v.Id == townId)
                    .ToListAsync();
            }
            else
            {
                averageValues = await _context.TownAverageValuesByYear
                    .Where(v => v.Id == townId && v.Year == salesYear)
                    .ToListAsync();
            }

            var averageValuesByYear = await _context.TownAverageValuesByYear
                .Where(t => t.Id == townId)
                .ToListAsync();

            return new CadastreItem
            {
                AverageValues = averageValues,
                AverageValuesByYear = averageValuesByYear,
                GeoJson = featureCollection
            };
        }
    }
}
