using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities.Json;
using Estimmo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimMetrics.Net.Metric;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class PlacesController : ControllerBase
    {
        private const int MaxResults = 100;

        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public PlacesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/places")]
        public async Task<List<JsonPlace>> GetPlaces([FromQuery] string name)
        {
            var places = await _context.Places
                .Where(p => EF.Functions.ILike(p.SearchName, $"%{name}%"))
                .Take(MaxResults)
                .ProjectTo<JsonPlace>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var levenstein = new Levenstein();
            var nameLower = name.ToLowerInvariant();

            return places
                .OrderByDescending(p => levenstein.GetSimilarity(nameLower, p.Name.ToLowerInvariant()))
                .ToList();
        }
    }
}
