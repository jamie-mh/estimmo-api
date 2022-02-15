using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities;
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
                ModelState.AddModelError("", "Name or coordinates must be specified");
                return ValidationProblem();
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
                .Include(p => p.Parent)
                .ThenInclude(p => p.Parent)
                .SingleOrDefaultAsync(p => p.Id == id && p.Type == type);

            if (place == null)
            {
                return NotFound();
            }

            var detailed = _mapper.Map<DetailedPlace>(place);
            return Ok(detailed);
        }
    }
}
