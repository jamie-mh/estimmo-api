using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities.Json;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Fastenshtein;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
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
                    .Where(p => EF.Functions.Like(p.SearchName, EF.Functions.Unaccent($"%{lowerName}%")));
            }
            else
            {
                var point = new Point(latitude.Value, longitude.Value);

                queryable = _context.Places
                    .Where(p => p.Geometry.Covers(point));
            }

            var places = await queryable
                .OrderBy(p => p.Name)
                .Take(limit)
                .ProjectTo<JsonPlace>(_mapper.ConfigurationProvider)
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
    }
}
