// Copyright (C) 2023 jmh
// SPDX-License-Identifier: AGPL-3.0-or-later

using Estimmo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error/404")]
        public IActionResult NotFoundError()
        {
            return NotFound(new Error { Code = 404, Message = "Not Found" });
        }

        [Route("/error/{code:int}")]
        public IActionResult Error(int code)
        {
            return StatusCode(code, new Error { Code = code, Message = "Error" });
        }
    }
}
