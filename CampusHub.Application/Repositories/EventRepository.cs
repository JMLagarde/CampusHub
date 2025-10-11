using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Application.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IApplicationDbContext _context;

        public EventRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Event>>> GetAllAsync()
        {
            var events = await _context.Events
                .Include(e => e.College)
                .Include(e => e.Program)
                .Include(e => e.Organizer)
                .Include(e => e.EventBookmarks)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return Result.Ok(events);
        }

        public async Task<Result<Event>> GetByIdAsync(int id)
        {
            var evt = await _context.Events
                .Include(e => e.College)
                .Include(e => e.Program)
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evt == null)
                return Result.Fail<Event>("Event not found");

            return Result.Ok(evt);
        }

        public async Task<Result<List<Event>>> GetByCollegeAsync(string collegeName)
        {
            var events = await _context.Events
                .Include(e => e.College)
                .Include(e => e.Program)
                .Include(e => e.Organizer)
                .Where(e => e.College != null && e.College.CollegeName == collegeName)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return Result.Ok(events);
        }

        public async Task<Result<Event>> CreateAsync(Event evt)
        {
            evt.CreatedAt = DateTime.UtcNow;
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            return Result.Ok(evt);
        }

        public async Task<Result<Event>> UpdateAsync(Event evt)
        {
            evt.UpdatedAt = DateTime.UtcNow;
            _context.Events.Update(evt);
            await _context.SaveChangesAsync();

            return Result.Ok(evt);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var result = await GetByIdAsync(id);
            if (result.IsFailed)
                return Result.Fail(result.Errors);

            _context.Events.Remove(result.Value);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<List<int>> GetBookmarkedEventIdsAsync(int userId)
        {
            return await _context.EventBookmarks
                .Where(eb => eb.UserId == userId)
                .Select(eb => eb.EventId)
                .ToListAsync();
        }

        public async Task<bool> ToggleBookmarkAsync(int eventId, int userId)
        {
            var existing = await _context.EventBookmarks
                .FirstOrDefaultAsync(eb => eb.EventId == eventId && eb.UserId == userId);

            if (existing != null)
            {
                _context.EventBookmarks.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // not bookmarked
            }
            else
            {
                _context.EventBookmarks.Add(new EventBookmark
                {
                    EventId = eventId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                return true; // bookmarked
            }
        }

        public async Task<Result<List<Event>>> GetBookmarkedEventsAsync(int userId)
        {
            var events = await _context.EventBookmarks
                .Where(eb => eb.UserId == userId)
                .Include(eb => eb.Event)
                    .ThenInclude(e => e.College)
                .Include(eb => eb.Event)
                    .ThenInclude(e => e.Program)
                .Include(eb => eb.Event)
                    .ThenInclude(e => e.Organizer)
                .Select(eb => eb.Event)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return Result.Ok(events);
        }
    }
}
