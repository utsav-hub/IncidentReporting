using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Requests;

namespace IncidentReporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncidentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IncidentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Helper method to get current user ID from JWT claims
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        // --------------------------------------------------------
        // GET: api/incidents
        // --------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            var result = await _mediator.Send(new GetAllIncidentsQuery(userId));
            return Ok(result);
        }

        // --------------------------------------------------------
        // GET: api/incidents/{id}
        // --------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _mediator.Send(new GetIncidentByIdQuery(id, userId));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // --------------------------------------------------------
        // POST: api/incidents
        // --------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] IncidentCreateDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _mediator.Send(new CreateIncidentCommand(dto, userId));

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // --------------------------------------------------------
        // PUT: api/incidents/{id}
        // --------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] IncidentUpdateDto dto)
        {
            var userId = GetCurrentUserId();
            var result = await _mediator.Send(new UpdateIncidentCommand(id, dto, userId));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // --------------------------------------------------------
        // DELETE: api/incidents/{id}
        // --------------------------------------------------------
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var deleted = await _mediator.Send(new DeleteIncidentCommand(id, userId));

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
