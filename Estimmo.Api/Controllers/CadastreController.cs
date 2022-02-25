using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
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

            var averageValues = await _context.FranceAverageValues
                .ToDictionaryAsync(v => (short) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }

        [HttpGet]
        [Route("/regions/{regionId}/departments")]
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

            var averageValues = await _context.RegionAverageValues
                .Where(r => r.Id == regionId)
                .ToDictionaryAsync(v => (short) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }

        [HttpGet]
        [Route("/departments/{departmentId}/towns")]
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

            var averageValues = await _context.DepartmentAverageValues
                .Where(d => d.Id == departmentId)
                .ToDictionaryAsync(v => (short) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
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

            var averageValues = await _context.TownAverageValues
                .Where(t => t.Id == townId)
                .ToDictionaryAsync(v => (short) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }
    }
}
