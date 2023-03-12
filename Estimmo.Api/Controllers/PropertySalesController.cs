using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class PropertySalesController : ControllerBase
    {
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public PropertySalesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/sections/{sectionId}/property-sales")]
        [SwaggerOperation(
            Summary = "Get property sales that occurred in a section",
            OperationId = "GetPropertySales",
            Tags = new[] { "PropertySale" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Features with values payload", typeof(FeaturesWithValues))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Section not found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<IActionResult> GetPropertySales(string sectionId, PropertyType? type = null, short? year = null)
        {
            if (type == PropertyType.All)
            {
                type = null;
            }

            Section section;

            if (year == null)
            {
                section = await _context.Sections
                    .Include(t => t.ValueStats)
                    .FirstOrDefaultAsync(s => s.Id == sectionId);
            }
            else
            {
                section = await _context.Sections
                    .Include(t => t.ValueStatsByYear.Where(v => v.Year == year))
                    .FirstOrDefaultAsync(s => s.Id == sectionId);
            }

            if (section == null)
            {
                return NotFound();
            }

            var queryable = _context.PropertySales
                .Include(p => p.Section)
                .Where(p => p.SectionId == sectionId);

            if (type != null)
            {
                queryable = queryable.Where(s => s.Type == type);
            }

            if (year != null)
            {
                queryable = queryable.Where(s => s.Date.Year == year);
            }

            var sales = await queryable
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            var featureCollection = _mapper.Map<FeatureCollection>(sales);
            featureCollection.BoundingBox = section.Geometry.EnvelopeInternal;

            IEnumerable<IValueStats> valueStats;

            if (year == null)
            {
                valueStats = await _context.SectionValueStats
                    .Where(v => v.Id == sectionId)
                    .ToListAsync();
            }
            else
            {
                valueStats = await _context.SectionValueStatsByYear
                    .Where(v => v.Id == sectionId && v.Year == year)
                    .ToListAsync();
            }

            var valueStatsByYear = await _context.SectionValueStatsByYear
                .Where(d => d.Id == sectionId)
                .ToListAsync();

            return Ok(new FeaturesWithValues
            {
                ValueStats = valueStats,
                ValueStatsByYear = valueStatsByYear,
                GeoJson = featureCollection
            });
        }
    }
}
