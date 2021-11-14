using AutoMapper;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class PropertyValuesController : ControllerBase
    {
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public PropertyValuesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/region-property-values")]
        public async Task<Dictionary<string, int>> GetRegions([Required] PropertyType type)
        {
            var averageValues = await _context.RegionAverageValues.Where(v => v.Type == type).ToListAsync();
            return _mapper.Map<Dictionary<string, int>>(averageValues);
        }

        [HttpGet]
        [Route("/regions/{regionId}/department-property-values")]
        public async Task<Dictionary<string, int>> GetRegion(string regionId, [Required] PropertyType type)
        {
            var averageValues = await _context.DepartmentAverageValues
                .Where(d => d.Type == type && d.RegionId == regionId)
                .ToListAsync();

            return _mapper.Map<Dictionary<string, int>>(averageValues);
        }

        [HttpGet]
        [Route("/departments/{departmentId}/town-property-values")]
        public async Task<Dictionary<string, int>> GetDepartment(string departmentId, [Required] PropertyType type)
        {
            var averageValues = await _context.TownAverageValues
                .Where(t => t.Type == type && t.DepartmentId == departmentId)
                .ToListAsync();

            return _mapper.Map<Dictionary<string, int>>(averageValues);
        }

        [HttpGet]
        [Route("/towns/{townId}/section-property-values")]
        public async Task<Dictionary<string, int>> GetTown(string townId, [Required] PropertyType type)
        {
            var averageValues = await _context.SectionAverageValues
                .Where(s => s.Type == type && s.TownId == townId)
                .ToListAsync();

            return _mapper.Map<Dictionary<string, int>>(averageValues);
        }
    }
}
