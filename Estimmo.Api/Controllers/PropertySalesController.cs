using AutoMapper;
using Estimmo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
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
        public async Task<IActionResult> GetPropertySales(string sectionId)
        {
            var section = await _context.Sections.SingleOrDefaultAsync(s => s.Id == sectionId);

            if (section == null)
            {
                return NotFound();
            }

            var sales = await _context.PropertySales
                .Include(p => p.Section)
                .Where(p => p.SectionId == sectionId)
                .ToListAsync();

            var collection = _mapper.Map<FeatureCollection>(sales);
            collection.BoundingBox = section.Geometry.EnvelopeInternal;

            return Ok(collection);
        }
    }
}
