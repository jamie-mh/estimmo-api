using AutoMapper;
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
        public async Task<FeatureCollection> GetRegions()
        {
            var regions = await _context.Regions.ToListAsync();
            return _mapper.Map<FeatureCollection>(regions);
        }

        [HttpGet]
        [Route("/regions/{regionId}/departments")]
        public async Task<FeatureCollection> GetDepartments(string regionId)
        {
            var departments = await _context.Departments.Where(d => d.RegionId == regionId).ToListAsync();
            return _mapper.Map<FeatureCollection>(departments);
        }

        [HttpGet]
        [Route("/departments/{departmentId}/towns")]
        public async Task<FeatureCollection> GetTowns(string departmentId)
        {
            var towns = await _context.Towns.Where(t => t.DepartmentId == departmentId).ToListAsync();
            return _mapper.Map<FeatureCollection>(towns);
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
        public async Task<FeatureCollection> GetSections(string townId)
        {
            var sections = await _context.Sections
                .Where(p => p.TownId == townId)
                .ToListAsync();

            return _mapper.Map<FeatureCollection>(sections);
        }
    }
}
