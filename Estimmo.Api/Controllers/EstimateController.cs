// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using AutoMapper;
using Estimmo.Api.Models;
using Estimmo.Shared.Entities;
using Estimmo.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    [Route("/estimate")]
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
        [SwaggerOperation(
            Summary = "Get an estimated value for a property",
            OperationId = "GetEstimate",
            Tags = ["Estimation"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Estimation result", typeof(Estimate))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<IActionResult> GetEstimate([FromBody] EstimateModel model)
        {
            var request = _mapper.Map<EstimateRequest>(model);
            var estimate = await _estimationService.GetEstimateAsync(request, DateTime.Now);

            return Ok(estimate);
        }
    }
}
