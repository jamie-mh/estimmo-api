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
        [Route("/towns/{townId}/sections/{sectionId}/property-sales")]
        public async Task<FeatureCollection> GetPropertySales(string townId, string sectionId)
        {
            var sales = await _context.PropertySales
                .Include(p => p.Section)
                .Where(p => p.Section.TownId == townId && p.SectionId == sectionId)
                .ToListAsync();

            return _mapper.Map<FeatureCollection>(sales);
        }
    }
}
