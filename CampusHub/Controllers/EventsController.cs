using CampusHub.Application.DTOs;
using CampusHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CampusHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var result = await _eventService.GetAllEventsAsync();

            if (result.IsFailed)
            {
                _logger.LogError("Error retrieving events: {Errors}", string.Join(", ", result.Errors));
                return StatusCode(500, result.Errors);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _eventService.GetEventByIdAsync(id);

            if (result.IsFailed)
            {
                _logger.LogError("Error retrieving event {EventId}: {Errors}", id, string.Join(", ", result.Errors));
                return NotFound(result.Errors);
            }

            return Ok(result.Value);
        }

        [HttpGet("college/{college}")]
        public async Task<IActionResult> GetEventsByCollege(string college)
        {
            var result = await _eventService.GetEventsByCollegeAsync(college);

            if (result.IsFailed)
            {
                _logger.LogError("Error retrieving events for college {College}: {Errors}", college, string.Join(", ", result.Errors));
                return StatusCode(500, result.Errors);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            var result = await _eventService.CreateEventAsync(eventDto);

            if (result.IsFailed)
            {
                _logger.LogError("Error creating event: {Errors}", string.Join(", ", result.Errors));
                return BadRequest(result.Errors);
            }

            return CreatedAtAction(nameof(GetEventById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto eventDto)
        {
            var result = await _eventService.UpdateEventAsync(id, eventDto);

            if (result.IsFailed)
            {
                _logger.LogError("Error updating event {EventId}: {Errors}", id, string.Join(", ", result.Errors));
                return BadRequest(result.Errors);
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _eventService.DeleteEventAsync(id);

            if (result.IsFailed)
            {
                _logger.LogError("Error deleting event {EventId}: {Errors}", id, string.Join(", ", result.Errors));
                return BadRequest(result.Errors);
            }

            return NoContent();
        }

        [HttpPost("{id}/toggle-bookmark")]
        public async Task<IActionResult> ToggleBookmarkEvent(int id, [FromBody] BookmarkRequest request)
        {
            var result = await _eventService.ToggleBookmarkEventAsync(id, request.UserId);

            if (result.IsFailed)
            {
                _logger.LogError("Error toggling bookmark for event {EventId}: {Errors}", id, string.Join(", ", result.Errors));
                return BadRequest(result.Errors);
            }

            return Ok();
        }

        [HttpPost("{id}/register")]
        public async Task<IActionResult> RegisterForEvent(int id, [FromBody] RegisterRequest request)
        {
            var result = await _eventService.RegisterForEventAsync(id, request.UserId);

            if (result.IsFailed)
            {
                _logger.LogError("Error registering for event {EventId}: {Errors}", id, string.Join(", ", result.Errors));
                return BadRequest(result.Errors);
            }

            return Ok();
        }
    }

    public class BookmarkRequest
    {
        public int UserId { get; set; }
    }

    public class RegisterRequest
    {
        public int UserId { get; set; }
    }
}