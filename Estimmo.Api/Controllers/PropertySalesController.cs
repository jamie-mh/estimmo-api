using AutoMapper;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
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
        public async Task<IActionResult> GetPropertySales(string sectionId, PropertyType? type = null)
        {
            var section = await _context.Sections.SingleOrDefaultAsync(s => s.Id == sectionId);

            if (section == null)
            {
                return NotFound();
            }

            var queryable = _context.PropertySales
                .Include(p => p.Section)
                .Where(p => p.SectionId == sectionId);

            List<PropertySale> sales;

            if (type == null)
            {
                sales = await queryable.ToListAsync();
            }
            else
            {
                sales = await queryable.Where(s => s.Type == type).ToListAsync();
            }

            var collection = _mapper.Map<FeatureCollection>(sales);
            collection.BoundingBox = section.Geometry.EnvelopeInternal;

            return Ok(collection);
        }
    }
}
