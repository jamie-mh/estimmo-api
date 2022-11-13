using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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
        [SwaggerOperation(
            Summary = "Get list of places matching criteria",
            OperationId = "GetPlaces",
            Tags = new[] { "Place" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Place list", typeof(IEnumerable<SimplePlace>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<ActionResult> GetPlaces(
            string name, double? latitude, double? longitude, [Range(1, 100)] int limit = 100)
        {
            if (name == null && (latitude == null || longitude == null))
            {
                ModelState.AddModelError("", "Name or coordinates must be specified");
                return ValidationProblem();
            }

            IQueryable<Place> queryable;

            if (name != null)
            {
                var postCodeMatch = Regex.Match(name, @"^(\d{5})$");

                if (postCodeMatch.Success)
                {
                    var postCode = postCodeMatch.Groups[1].Value;

                    queryable = _context.Places
                        .Where(p => p.Type == PlaceType.Town && p.PostCode == postCode);
                }
                else
                {
                    var simplifiedName = name
                        .ToLowerInvariant()
                        .Unaccent()
                        .Replace("-", "")
                        .Replace(",", "");

                    queryable = _context.Places
                        .Where(p => p.IsSearchable && EF.Functions.Like(p.SearchName, simplifiedName + "%"))
                        .OrderBy(p => p.Type);
                }
            }
            else
            {
                var point = new Point(longitude.Value, latitude.Value);

                queryable = _context.Places
                    .Where(p => p.Geometry.Covers(point))
                    .OrderBy(p => p.Type);
            }

            var places = await queryable
                .Take(limit)
                .ProjectTo<SimplePlace>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(places);
        }

        [HttpGet]
        [Route("/places/{id}")]
        [SwaggerOperation(
            Summary = "Get single place an its hierarchy",
            OperationId = "GetPlace",
            Tags = new[] { "Place" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Place", typeof(DetailedPlace))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<IActionResult> GetPlace(string id, [Required] PlaceType type)
        {
            var place = await _context.Places
                .Include(p => p.Parent)
                .ThenInclude(p => p.Parent)
                .ThenInclude(p => p.Parent)
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
