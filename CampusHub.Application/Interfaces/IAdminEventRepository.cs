using CampusHub.Domain.Entities;
using FluentResults;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminEventRepository
    {
        Task<Result<List<Event>>> GetAllEventsAsync();
        Task<Result<Event>> GetEventByIdAsync(int id);
        Task<Result<Event>> CreateEventAsync(Event evt);
        Task<Result<Event>> UpdateEventAsync(Event evt);
        Task<Result> DeleteEventAsync(int id);
        Task<Result<List<College>>> GetAllCollegesAsync();
        Task<Result<List<ProgramEntity>>> GetAllProgramsAsync();
        Task<Result<List<ProgramEntity>>> GetProgramsByCollegeAsync(int collegeId);
    }
}