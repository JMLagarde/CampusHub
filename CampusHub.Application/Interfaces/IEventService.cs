using CampusHub.Application.DTOs;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IEventService
    {
        Task<Result<List<EventDto>>> GetAllEventsAsync();
        Task<Result<EventDto>> GetEventByIdAsync(int id);
        Task<Result<List<EventDto>>> GetEventsByCollegeAsync(string college);
        Task<Result<EventDto>> CreateEventAsync(EventDto eventDto);
        Task<Result<EventDto>> UpdateEventAsync(int id, EventDto eventDto);
        Task<Result> DeleteEventAsync(int id);
        Task<Result> ToggleBookmarkEventAsync(int eventId, int userId);
        Task<Result> RegisterForEventAsync(int eventId, int userId);
    }
}