using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace CampusHub.Application.Repositories
{
    public class AdminEventRepository : IAdminEventRepository
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AdminEventRepository> _logger;

        public AdminEventRepository(IApplicationDbContext context, ILogger<AdminEventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<Event>>> GetAllEventsAsync()
        {
            try
            {
                var events = await _context.Events
                    .Include(e => e.College)
                    .Include(e => e.Program)
                    .Include(e => e.Organizer)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync();

                return Result.Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all events from database");
                return Result.Fail<List<Event>>(new Error("Failed to retrieve events from database")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<Event>> GetEventByIdAsync(int id)
        {
            try
            {
                var evt = await _context.Events
                    .Include(e => e.College)
                    .Include(e => e.Program)
                    .Include(e => e.Organizer)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (evt == null)
                    return Result.Fail<Event>(new Error($"Event with ID {id} not found")
                        .WithMetadata("StatusCode", 404));

                return Result.Ok(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving event {EventId} from database", id);
                return Result.Fail<Event>(new Error($"Failed to retrieve event with ID {id}")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<Event>> CreateEventAsync(Event evt)
        {
            try
            {
                evt.CreatedAt = DateTime.UtcNow;
                _context.Events.Add(evt);
                await _context.SaveChangesAsync();

                return Result.Ok(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event in database");
                return Result.Fail<Event>(new Error("Failed to create event in database")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<Event>> UpdateEventAsync(Event evt)
        {
            try
            {
                evt.UpdatedAt = DateTime.UtcNow;
                _context.Events.Update(evt);
                await _context.SaveChangesAsync();

                return Result.Ok(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId} in database", evt.Id);
                return Result.Fail<Event>(new Error($"Failed to update event with ID {evt.Id}")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> DeleteEventAsync(int id)
        {
            try
            {
                var eventResult = await GetEventByIdAsync(id);
                if (eventResult.IsFailed)
                    return Result.Fail(eventResult.Errors);

                _context.Events.Remove(eventResult.Value);
                await _context.SaveChangesAsync();

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event {EventId} from database", id);
                return Result.Fail(new Error($"Failed to delete event with ID {id}")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<College>>> GetAllCollegesAsync()
        {
            try
            {
                var colleges = await _context.Colleges
                    .OrderBy(c => c.CollegeName)
                    .ToListAsync();

                return Result.Ok(colleges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving colleges from database");
                return Result.Fail<List<College>>(new Error("Failed to retrieve colleges from database")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<ProgramEntity>>> GetAllProgramsAsync()
        {
            try
            {
                var programs = await _context.Programs
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return Result.Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs from database");
                return Result.Fail<List<ProgramEntity>>(new Error("Failed to retrieve programs from database")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<ProgramEntity>>> GetProgramsByCollegeAsync(int collegeId)
        {
            try
            {
                var programs = await _context.Programs
                    .Where(p => p.CollegeId == collegeId)
                    .OrderBy(p => p.Name)
                    .ToListAsync();

                return Result.Ok(programs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving programs for college {CollegeId} from database", collegeId);
                return Result.Fail<List<ProgramEntity>>(new Error($"Failed to retrieve programs for college {collegeId}")
                    .CausedBy(ex));
            }
        }
    }
}