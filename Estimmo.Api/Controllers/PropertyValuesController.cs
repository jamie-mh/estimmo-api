using AutoMapper;
using Estimmo.Api.Models;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        private static PropertyValuesByType GetPropertyValuesByType(IEnumerable<IAverageValue> averageValues)
        {
            var result = new PropertyValuesByType();

            foreach (var value in averageValues)
            {
                if (!result.ContainsKey(value.Id))
                {
                    result.Add(value.Id, new Dictionary<int, double>());
                }

                result[value.Id].Add((int) value.Type, value.Value);
            }

            return result;
        }

        [HttpGet]
        [Route("/region-property-values")]
        public async Task<PropertyValuesByType> GetRegions()
        {
            var averageValues = await _context.RegionAverageValues.ToListAsync();
            return GetPropertyValuesByType(averageValues);
        }

        [HttpGet]
        [Route("/regions/{regionId}/department-property-values")]
        public async Task<PropertyValuesByType> GetRegion(string regionId)
        {
            var averageValues = await _context.DepartmentAverageValues
                .Where(d => d.RegionId == regionId)
                .ToListAsync();

            return GetPropertyValuesByType(averageValues);
        }

        [HttpGet]
        [Route("/departments/{departmentId}/town-property-values")]
        public async Task<PropertyValuesByType> GetDepartment(string departmentId)
        {
            var averageValues = await _context.TownAverageValues
                .Where(t => t.DepartmentId == departmentId)
                .ToListAsync();

            return GetPropertyValuesByType(averageValues);
        }

        [HttpGet]
        [Route("/towns/{townId}/section-property-values")]
        public async Task<PropertyValuesByType> GetTown(string townId)
        {
            var averageValues = await _context.SectionAverageValues
                .Where(s => s.TownId == townId)
                .ToListAsync();

            return GetPropertyValuesByType(averageValues);
        }
    }
}
