using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
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
        public async Task<CadastreItem> GetRegions()
        {
            var regions = await _context.Regions
                .Include(r => r.AverageValues)
                .ToListAsync();

            var featureCollection = _mapper.Map<FeatureCollection>(regions);

            var averageValues = await _context.FranceAverageValues
                .ToDictionaryAsync(v => (int) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }

        [HttpGet]
        [Route("/regions/{regionId}/departments")]
        public async Task<CadastreItem> GetDepartments(string regionId)
        {
            var departments = await _context.Departments
                .Where(d => d.RegionId == regionId)
                .Include(d => d.AverageValues)
                .ToListAsync();

            var featureCollection = _mapper.Map<FeatureCollection>(departments);

            var averageValues = await _context.RegionAverageValues
                .Where(r => r.Id == regionId)
                .ToDictionaryAsync(v => (int) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }

        [HttpGet]
        [Route("/departments/{departmentId}/towns")]
        public async Task<CadastreItem> GetTowns(string departmentId)
        {
            var towns = await _context.Towns
                .Where(t => t.DepartmentId == departmentId)
                .Include(t => t.AverageValues)
                .ToListAsync();

            var featureCollection = _mapper.Map<FeatureCollection>(towns);

            var averageValues = await _context.DepartmentAverageValues
                .Where(d => d.Id == departmentId)
                .ToDictionaryAsync(v => (int) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
        public async Task<CadastreItem> GetSections(string townId)
        {
            var sections = await _context.Sections
                .Where(s => s.TownId == townId)
                .Include(s => s.AverageValues)
                .ToListAsync();

            var featureCollection = _mapper.Map<FeatureCollection>(sections);

            var averageValues = await _context.TownAverageValues
                .Where(t => t.Id == townId)
                .ToDictionaryAsync(v => (int) v.Type, v => v.Value);

            return new CadastreItem { AverageValues = averageValues, GeoJson = featureCollection };
        }
    }
}
