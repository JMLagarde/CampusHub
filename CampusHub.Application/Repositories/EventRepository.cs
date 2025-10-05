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
                .OrderBy(e => e.Date)
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
                .OrderBy(e => e.Date)
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
    }
}