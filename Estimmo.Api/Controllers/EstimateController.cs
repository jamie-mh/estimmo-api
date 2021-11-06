using AutoMapper;
using Estimmo.Api.Entities;
using Estimmo.Api.Models;
using Estimmo.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    [Route("/api/estimate")]
    public class EstimateController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IEstimationService _estimationService;

        public EstimateController(IMapper mapper, IEstimationService estimationService)
        {
            _mapper = mapper;
            _estimationService = estimationService;
        }

        [HttpPost]
        public async Task<IActionResult> GetEstimate([FromBody] EstimateModel model)
        {
            var request = _mapper.Map<EstimateRequest>(model);
            var estimate = await _estimationService.GetEstimateAsync(request);

            return Ok(estimate);
        }
    }
}
