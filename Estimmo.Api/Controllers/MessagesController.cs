using AutoMapper;
using AutoMapper.QueryableExtensions;
using Estimmo.Api.Entities;
using Estimmo.Api.Models;
using Estimmo.Data;
using Estimmo.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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
