using MediatR;
using Microsoft.AspNetCore.Mvc;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Requests;

namespace IncidentReporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IncidentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // --------------------------------------------------------
        // GET: api/incidents
        // --------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllIncidentsQuery());
            return Ok(result);
        }

        // --------------------------------------------------------
        // GET: api/incidents/{id}
        // --------------------------------------------------------
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetIncidentByIdQuery(id));

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
            var result = await _mediator.Send(new CreateIncidentCommand(dto));

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // --------------------------------------------------------
        // PUT: api/incidents/{id}
        // --------------------------------------------------------
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] IncidentUpdateDto dto)
        {
            var result = await _mediator.Send(new UpdateIncidentCommand(id, dto));

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
            var deleted = await _mediator.Send(new DeleteIncidentCommand(id));

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
