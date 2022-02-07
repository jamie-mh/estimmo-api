using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities.Json;
using Estimmo.Data;
using Fastenshtein;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public async Task<List<JsonPlace>> GetPlaces([Required] [FromQuery] string name)
        {
            name = name.Replace("-", " ");

            var places = await _context.Places
                .Where(p => EF.Functions.ILike(p.SearchName, $"%{name}%"))
                .Take(MaxResults)
                .ProjectTo<JsonPlace>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var levenshtein = new Levenshtein(name.ToLowerInvariant());

            return places
                .OrderBy(p => levenshtein.DistanceFrom(p.Name.ToLowerInvariant()))
                .ToList();
        }
    }
}
