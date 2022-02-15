using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Fastenshtein;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public PlacesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/places")]
        public async Task<ActionResult> GetPlaces(string name, double? latitude, double? longitude,
            [Range(1, 100)] int limit = 100)
        {
            if (name == null && (latitude == null || longitude == null))
            {
                return BadRequest("Name or coordinates must be specified");
            }

            IQueryable<Place> queryable;

            if (name != null)
            {
                var lowerName = name
                    .ToLowerInvariant()
                    .Replace("-", " ");

                queryable = _context.Places
                    .Where(p => EF.Functions.Like(p.SearchName, EF.Functions.Unaccent($"%{lowerName}%")) ||
                                EF.Functions.Like(p.PostCode, EF.Functions.Unaccent($"%{lowerName}%")));
            }
            else
            {
                var point = new Point(longitude.Value, latitude.Value);

                queryable = _context.Places
                    .Where(p => p.Geometry.Covers(point));
            }

            var places = await queryable
                .OrderBy(p => p.Name)
                .Take(limit)
                .ProjectTo<SimplePlace>(_mapper.ConfigurationProvider)
                .ToListAsync();

            if (name == null)
            {
                return Ok(places);
            }

            var levenshtein = new Levenshtein(name.ToLowerInvariant());

            places = places
                .OrderBy(p => levenshtein.DistanceFrom(p.Name.ToLowerInvariant()))
                .ToList();

            return Ok(places);
        }

        [HttpGet]
        [Route("/places/{id}")]
        public async Task<IActionResult> GetPlace(string id, [Required] PlaceType type)
        {
            var place = await _context.Places
                .ProjectTo<DetailedPlace>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id == id && p.Type == type);

            if (place == null)
            {
                return NotFound();
            }

            place.Hierarchy = new List<SimplePlace>();

            async Task AddPlaceToHierarchy(PlaceType placeType, string placeId)
            {
                var p =  await _context.Places
                    .ProjectTo<SimplePlace>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(p => p.Type == placeType && p.Id == placeId);

                if (p != null)
                {
                    place.Hierarchy.Add(p);
                }
            }

            if (place.Type is PlaceType.Department)
            {
                var department = await _context.Departments
                    .Select(d => new { d.Id, d.RegionId })
                    .FirstOrDefaultAsync(d => d.Id == id);

                await AddPlaceToHierarchy(PlaceType.Region, department.RegionId);
            }
            else if (place.Type == PlaceType.Town)
            {
                var town = await _context.Towns
                    .Select(t => new { t.Id, t.DepartmentId })
                    .FirstOrDefaultAsync(t => t.Id == place.Id);

                var department = await _context.Departments
                    .Select(d => new { d.Id, d.RegionId })
                    .FirstOrDefaultAsync(d => d.Id == town.DepartmentId);

                await AddPlaceToHierarchy(PlaceType.Region, department.RegionId);
                await AddPlaceToHierarchy(PlaceType.Department, town.DepartmentId);
            }

            return Ok(place);
        }
    }
}
