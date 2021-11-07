using Estimmo.Api.Models;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    [Route("/towns")]
    public class TownsController : ControllerBase
    {
        private readonly EstimmoContext _context;

        public TownsController(EstimmoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<Town>> Default(NearbyModel model)
        {
            // TODO: swap on import
            var centerPoint = new Point(model.Longitude, model.Latitude);

            return await _context.Towns
                .Where(t => t.Geometry.IsWithinDistance(centerPoint, model.Radius))
                .ToListAsync();
        }
    }
}
