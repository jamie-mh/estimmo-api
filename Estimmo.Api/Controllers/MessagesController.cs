using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities;
using Estimmo.Api.Models;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Estimmo.Api.Controllers
{
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly EstimmoContext _context;
        private readonly IMapper _mapper;

        public MessagesController(EstimmoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [Route("/messages")]
        [SwaggerOperation(
            Summary = "Get messages",
            OperationId = "GetMessages",
            Tags = new[] { "Message" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Message list", typeof(IEnumerable<Message>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "No authentication token provided")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Invalid authentication token or role")]
        public async Task<IActionResult> GetMessages(
            [Range(0, int.MaxValue)] int offset = 0, [Range(1, 100)] int limit = 100, bool isArchived = false)
        {
            var messages = await _context.Messages
                .Where(m => m.IsArchived == isArchived)
                .ProjectTo<SimpleMessage>(_mapper.ConfigurationProvider)
                .OrderByDescending(m => m.SentOn)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpGet]
        [Authorize]
        [Route("/messages/{id:int}")]
        [SwaggerOperation(
            Summary = "Get single message",
            OperationId = "GetMessage",
            Tags = new[] { "Message" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Message", typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "No authentication token provided")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Invalid authentication token or role")]
        public async Task<IActionResult> GetMessage(int id)
        {
            var message = await _context.Messages
                .Where(m => m.Id == id)
                .ProjectTo<DetailedMessage>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            return Ok(message);
        }

        [HttpPatch]
        [Authorize]
        [Route("/messages/{id:int}")]
        [SwaggerOperation(
            Summary = "Set message as archived",
            OperationId = "UpdateMessage",
            Tags = new[] { "Message" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK, "Message", typeof(Message))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "No authentication token provided")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Invalid authentication token or role")]
        public async Task<IActionResult> SetArchived(int id, MessagePatchModel model)
        {
            var message = await _context.Messages
                .Where(m => m.Id == id)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            message.IsArchived = model.IsArchived;
            await _context.SaveChangesAsync();
            return Ok(message);
        }

        [HttpPost]
        [Route("/messages")]
        [SwaggerOperation(
            Summary = "Send message",
            OperationId = "CreateMessage",
            Tags = new[] { "Message" }
        )]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed")]
        public async Task<IActionResult> SendMessage([FromBody] MessageModel model)
        {
            var message = _mapper.Map<Message>(model);
            message.SentOn = DateTime.UtcNow;

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
