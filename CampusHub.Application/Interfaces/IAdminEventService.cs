using CampusHub.Application.DTO;
using CampusHub.Application.DTOs;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace CampusHub.Application.Interfaces
{
    public interface IAdminEventService
    {
        Task<Result<List<EventDto>>> GetAllEventsAsync();
        Task<Result<EventDto>> GetEventByIdAsync(int id);
        Task<Result<EventDto>> CreateEventAsync(EventDto eventDto);
        Task<Result<EventDto>> UpdateEventAsync(int id, EventDto eventDto);
        Task<Result> DeleteEventAsync(int id);
        Task<Result<List<CollegeDto>>> GetAllCollegesAsync();
        Task<Result<List<AdminProgramDto>>> GetAllProgramsAsync();
        Task<Result<List<AdminProgramDto>>> GetProgramsByCollegeAsync(int collegeId);
        Task<Result<string>> UploadEventImageAsync(IFormFile file);
    }
}