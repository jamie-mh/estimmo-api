// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Estimmo.Shared.Extension;
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
    public partial class PlacesController : ControllerBase
    {
        private const double DistanceMargin = 50d; 
        
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public PlacesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [GeneratedRegex("^(\\d{5})$")]
        private static partial Regex PostCodeRegex();

        [GeneratedRegex("[(),]+")]
        private static partial Regex SimplifyRegex();
        
        private static string SimplifyForSearch(string input)
        {
            input = input.ToLowerInvariant().Unaccent();
            return SimplifyRegex().Replace(input, "");
        }

        [HttpGet]
        [Route("/places")]
        [SwaggerOperation(
            Summary = "Get list of places matching criteria",
            OperationId = "GetPlaces",
            Tags = ["Place"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Place list", typeof(IEnumerable<SimplePlace>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<ActionResult> GetPlaces(
            string name, double? latitude, double? longitude, PlaceType? type, [Range(1, 100)] int limit = 100)
        {
            if (name == null && (latitude == null || longitude == null))
            {
                ModelState.AddModelError("", "Name or coordinates must be specified");
                return ValidationProblem();
            }

            IQueryable<Place> queryable;

            if (name != null)
            {
                var postCodeMatch = PostCodeRegex().Match(name);

                if (postCodeMatch.Success)
                {
                    var postCode = postCodeMatch.Groups[1].Value;
                    queryable = _context.Places.Where(p => p.Type == PlaceType.Town && p.PostCode == postCode);
                }
                else
                {
                    var simplifiedName = SimplifyForSearch(name);
                    queryable = _context.Places
                        .Where(p => p.IsSearchable && EF.Functions.Like(p.SearchName, simplifiedName + "%"));
                }
            }
            else
            {
                var point = new Point(longitude.Value, latitude.Value);

                queryable = _context.Places
                    .Where(p => p.Geometry.Covers(point) || p.Geometry.IsWithinDistance(point, DistanceMargin))
                    .OrderBy(p => p.Type)
                    .ThenBy(p => p.Geometry.Distance(point));
            }

            if (type != null)
            {
                queryable = queryable.Where(p => p.Type == type);
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
            Tags = ["Place"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Place", typeof(RootPlace))]
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

            var detailed = _mapper.Map<RootPlace>(place);
            return Ok(detailed);
        }
    }
}
