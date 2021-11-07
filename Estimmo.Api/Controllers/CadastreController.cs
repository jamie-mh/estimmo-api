using AutoMapper;
using Estimmo.Api.Models;
using Estimmo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class CadastreController : ControllerBase
    {
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public CadastreController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/towns")]
        public async Task<FeatureCollection> GetTowns([FromQuery] NearbyModel model)
        {
            var centerPoint = new Point(model.Longitude, model.Latitude);

            var towns = await _context.Towns
                .Where(t => t.Geometry.IsWithinDistance(centerPoint, model.Radius))
                .ToListAsync();

            return _mapper.Map<FeatureCollection>(towns);
        }

        [HttpGet]
        [Route("/towns/{townId}/sections")]
        public async Task<FeatureCollection> GetSections(string townId)
        {
            var sections = await _context.Sections
                .Where(p => p.TownId == townId)
                .ToListAsync();

            return _mapper.Map<FeatureCollection>(sections);
        }

        [HttpGet]
        [Route("/towns/{townId}/sections/{sectionId}/parcels")]
        public async Task<FeatureCollection> GetParcels(string townId, string sectionId)
        {
            var parcels = await _context.Parcels
                .Where(p => p.TownId == townId && p.SectionId == sectionId)
                .ToListAsync();

            return _mapper.Map<FeatureCollection>(parcels);
        }
    }
}
