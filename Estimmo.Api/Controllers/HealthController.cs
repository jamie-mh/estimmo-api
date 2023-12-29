// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    [Route("/health")]
    public class HealthController : ControllerBase
    {
        private readonly EstimmoContext _context;

        public HealthController(EstimmoContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Check API health",
            OperationId = "GetHealth",
            Tags = ["Health"]
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "API is available")]
        [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "API is unavailable")]
        public async Task<IActionResult> Default()
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return canConnect
                ? Ok()
                : StatusCode((int) HttpStatusCode.ServiceUnavailable);
        }
    }
}
