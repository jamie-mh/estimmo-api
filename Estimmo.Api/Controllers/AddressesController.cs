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
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private const double DistanceMargin = 50.0;

        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public AddressesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/addresses")]
        [SwaggerOperation(
            Summary = "Get list of addresses matching criteria",
            OperationId = "GetAddresses",
            Tags = new[] { "Address" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Address list", typeof(IEnumerable<AddressLike>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<ActionResult> GetAddresses(
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
                var simplifiedName = name
                    .ToLowerInvariant()
                    .Unaccent()
                    .Replace("-", "")
                    .Replace(",", "")
                    .Replace("(", "")
                    .Replace(")", "");

                queryable = _context.Places
                    .Where(p => p.IsSearchable && p.Type == PlaceType.Address &&
                                EF.Functions.Like(p.SearchName, simplifiedName + "%"));
            }
            else
            {
                var point = new Point(longitude.Value, latitude.Value);

                queryable = _context.Places
                    .Where(p => p.Type == PlaceType.Address && p.Geometry.IsWithinDistance(point, DistanceMargin))
                    .OrderBy(p => p.Geometry.Distance(point));
            }

            var addresses = await queryable
                .Take(limit)
                .ProjectTo<AddressLike>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(addresses);
        }

        [HttpGet]
        [Route("/addresses/{id}")]
        [SwaggerOperation(
            Summary = "Get single address",
            OperationId = "GetAddress",
            Tags = new[] { "Address" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Address", typeof(AddressLike))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        public async Task<ActionResult> GetAddress(string id)
        {
            var place = await _context.Places
                .FirstOrDefaultAsync(p => p.Type == PlaceType.Address && p.Id == id);

            if (place == null)
            {
                return NotFound();
            }

            var address = _mapper.Map<AddressLike>(place);
            return Ok(address);
        }
    }
}
