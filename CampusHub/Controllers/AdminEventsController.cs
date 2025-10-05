using CampusHub.Application.DTOs;
using CampusHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminEventsController : ControllerBase
    {
        private readonly IAdminEventService _adminEventService;
        private readonly IApplicationDbContext _context;

        public AdminEventsController(
            IAdminEventService adminEventService,
            IApplicationDbContext context)
        {
            _adminEventService = adminEventService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var result = await _adminEventService.GetAllEventsAsync();

            if (result.IsFailed)
                return StatusCode(500, result.Errors);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var result = await _adminEventService.GetEventByIdAsync(id);

            if (result.IsFailed)
                return NotFound(result.Errors);

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminEventService.CreateEventAsync(eventDto);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetEventById), new { id = result.Value.Id }, result.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventDto eventDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminEventService.UpdateEventAsync(id, eventDto);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var result = await _adminEventService.DeleteEventAsync(id);

            if (result.IsFailed)
                return BadRequest(result.Errors);

            return NoContent();
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var result = await _adminEventService.UploadEventImageAsync(file);

            if (result.IsFailed)
                return BadRequest(new { message = result.Errors.FirstOrDefault()?.Message ?? "Upload failed" });

            return Ok(new { ImagePath = result.Value });
        }

        [HttpGet("colleges")]
        public async Task<IActionResult> GetColleges()
        {
            var colleges = await _context.Colleges
                .Select(c => new CollegeDto
                {
                    CollegeId = c.CollegeId,
                    CollegeName = c.CollegeName
                })
                .ToListAsync();

            return Ok(colleges);
        }

        [HttpGet("programs")]
        public async Task<IActionResult> GetPrograms()
        {
            var programs = await _context.Programs
                .Select(p => new ProgramDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CollegeId = p.CollegeId
                })
                .ToListAsync();

            return Ok(programs);
        }

        [HttpGet("programs/college/{collegeId}")]
        public async Task<IActionResult> GetProgramsByCollege(int collegeId)
        {
            var programs = await _context.Programs
                .Where(p => p.CollegeId == collegeId)
                .Select(p => new ProgramDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CollegeId = p.CollegeId
                })
                .ToListAsync();

            return Ok(programs);
        }
    }

    public class CollegeDto
    {
        public int CollegeId { get; set; }
        public string CollegeName { get; set; } = string.Empty;
    }

    public class ProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CollegeId { get; set; }
    }
}