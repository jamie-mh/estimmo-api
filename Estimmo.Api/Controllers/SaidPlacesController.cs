using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class SaidPlacesController : ControllerBase
    {
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public SaidPlacesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("/said-places/{id}")]
        [SwaggerOperation(
            Summary = "Get single said place",
            OperationId = "GetSaidPlace",
            Tags = new[] { "SaidPlace" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Said place", typeof(AddressLike))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Said place not found")]
        public async Task<ActionResult> GetAddress(string id)
        {
            var place = await _context.Places
                .FirstOrDefaultAsync(p => p.Type == PlaceType.SaidPlace && p.Id == id);

            if (place == null)
            {
                return NotFound();
            }

            var address = _mapper.Map<AddressLike>(place);
            return Ok(address);
        }
    }
}
