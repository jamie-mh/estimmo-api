using AutoMapper;
using Estimmo.Api.Models.Features;
using Estimmo.Data;
using Estimmo.Data.Entities;
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

        private async Task<FeatureCollection> QueryableToFeatureCollection<T>(IQueryable<T> queryable)
        {
            var features = await queryable.ToListAsync();
            return _mapper.Map<FeatureCollection>(features);
        }

        [HttpGet]
        [Route("/regions")]
        public async Task<FeatureCollection> GetRegions(PropertyType propertyType = PropertyType.House)
        {
            var query = from region in _context.Regions
                join avgValue in _context.RegionAverageValues on region.Id equals avgValue.Id into grouping
                from avgValue in grouping.DefaultIfEmpty()
                where avgValue.Type == propertyType
                select new RegionFeature { Region = region, AverageValue = avgValue };

            return await QueryableToFeatureCollection(query);
        }

        [HttpGet]
        [Route("/regions/{regionId}/departments")]
        public async Task<FeatureCollection> GetDepartments(string regionId, PropertyType propertyType = PropertyType.House)
        {
            var query = from department in _context.Departments
                join avgValue in _context.DepartmentAverageValues on department.Id equals avgValue.Id into grouping
                from avgValue in grouping.DefaultIfEmpty()
                where department.RegionId == regionId && avgValue.Type == propertyType
                select new DepartmentFeature { Department = department, AverageValue = avgValue };

            return await QueryableToFeatureCollection(query);
        }

        [HttpGet]
        [Route("/departments/{departmentId}/towns")]
        public async Task<FeatureCollection> GetTowns(string departmentId, PropertyType propertyType = PropertyType.House)
        {
            var query = from town in _context.Towns
                join avgValue in _context.TownAverageValues on town.Id equals avgValue.Id into grouping
                from avgValue in grouping.DefaultIfEmpty()
                where town.DepartmentId == departmentId && avgValue.Type == propertyType
                select new TownFeature { Town = town, AverageValue = avgValue };

            return await QueryableToFeatureCollection(query);
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
        public async Task<FeatureCollection> GetSections(string townId, PropertyType propertyType = PropertyType.House)
        {
            var query = from section in _context.Sections
                join avgValue in _context.SectionAverageValues on section.Id equals avgValue.Id into grouping
                from avgValue in grouping.DefaultIfEmpty()
                where section.TownId == townId && avgValue.Type == propertyType
                select new SectionFeature { Section = section, AverageValue = avgValue };

            return await QueryableToFeatureCollection(query);
        }
    }
}
